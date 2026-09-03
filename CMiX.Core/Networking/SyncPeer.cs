// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VL.Serialization.MessagePack;

namespace CMiX.Core.Networking
{
    // One process in the sync: the Studio, an Engine, or a console. It appends its own edits to
    // the store, applies the edits of the other peers, and joins from the snapshot.
    public partial class SyncPeer : ObservableObject, IMessageSender, IDisposable
    {
        private const int ReplayBatch = 256;

        private readonly ISyncTarget _target;
        private readonly ControlMessenger _messenger;
        private readonly Func<SyncOptions, ISyncStore> _storeFactory;
        private readonly OutgoingQueue _outgoing = new();
        private readonly SemaphoreSlim _joinLock = new(1, 1);
        private readonly string _peerId;

        private ISyncStore _store;
        private SnapshotCompactor _compactor;
        private bool _compactionEnabled;
        private CancellationTokenSource _cts;
        private CancellationTokenSource _followerCts;
        private Task _followerTask;
        private Action<Action> _dispatcher;
        private bool _autoJoin;
        private bool _started;
        // Counts the starts. A join, push or check of an earlier start must change nothing.
        private int _generation;
        private bool _wasConnected;
        private bool _afterFirstConnect;
        private string _activity;

        public SyncPeer(ISyncTarget target, ControlMessenger messenger, Func<SyncOptions, ISyncStore> storeFactory)
        {
            _target = target;
            _messenger = messenger;
            _storeFactory = storeFactory;
            _peerId = PeerId.ToString();
            _messenger.IsSendingBlocked = true;
            Peers = new ObservableCollection<PeerInfo>();
            PushCommand = new AsyncRelayCommand(() => PushAsync());
            PullCommand = new AsyncRelayCommand(() => JoinAsync());
        }

        public Guid PeerId { get; } = Guid.NewGuid();
        public SyncOptions Options { get; private set; }
        public string Name => Options?.PeerName ?? string.Empty;
        public string Role => Options?.Role ?? string.Empty;

        public ICommand PushCommand { get; }
        public ICommand PullCommand { get; }
        public ObservableCollection<PeerInfo> Peers { get; }

        // The Studio sets both. Engines neither compact nor list the other peers.
        public bool CompactionEnabled
        {
            get => _compactionEnabled;
            set
            {
                _compactionEnabled = value;
                if (value)
                    CreateCompactor();
                else
                    DisposeCompactor();
            }
        }

        public bool ListPeersEnabled { get; set; }

        // Only a writer appends entries and pushes. Engines read and apply.
        public bool IsWriter { get; set; }

        // How long a value change waits before the compactor writes a new snapshot. Tests shorten it.
        public TimeSpan CompactionDelay { get; set; } = SyncTimings.CompactionDelay;

        public int PendingMessages => _outgoing.PendingCount;

        public event Action<StreamEntry, IMessage> MessageApplied;
        public event Action<IMessage> MessageSent;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInSync), nameof(Status))]
        private bool _isConnected;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInSync), nameof(Status))]
        private bool _isJoined;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInSync))]
        private StreamPosition _lastAppliedId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInSync))]
        private StreamPosition _tailId;

        // The ID of the last own entry the store confirmed.
        [ObservableProperty]
        private StreamPosition _lastSentId;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private long _sentMessages;

        [ObservableProperty]
        private long _appliedMessages;

        partial void OnIsJoinedChanged(bool value) => _messenger.IsSendingBlocked = !value;

        public bool IsInSync => IsConnected && IsJoined && LastAppliedId == TailId;

        public string Status =>
            !_started ? "Not started" :
            !IsConnected ? (_wasConnected ? "Reconnecting" : "Connecting") :
            _activity != null ? _activity :
            !IsJoined ? "Not in sync" : "Connected";

        public void SetDispatcher(Action<Action> dispatcher) => _dispatcher = dispatcher;

        // Returns at once. A background task connects, joins when asked, and starts the loops.
        public void Start(SyncOptions options, bool autoJoin)
        {
            Stop();

            Interlocked.Increment(ref _generation);
            Options = options.WithFallbacks();
            _autoJoin = autoJoin;
            _store = _storeFactory(Options);
            CreateCompactor();
            _cts = new CancellationTokenSource();
            _started = true;
            _wasConnected = false;
            _afterFirstConnect = false;
            OnPropertyChanged(nameof(Options));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Role));
            OnPropertyChanged(nameof(Status));

            var store = _store;
            var generation = Volatile.Read(ref _generation);
            var ct = _cts.Token;
            _ = Task.Run(() => RunAsync(store, generation, ct), ct);
        }

        // Returns at once. The store is disposed on a background task. Queued messages stay queued.
        public void Stop()
        {
            if (!_started) return;

            _started = false;
            DisposeCompactor();
            var cts = _cts;
            var followerCts = _followerCts;
            var store = _store;
            _cts = null;
            _followerCts = null;
            _followerTask = null;
            _store = null;

            Cancel(followerCts);
            Cancel(cts);

            if (store != null)
            {
                store.ConnectionChanged -= OnConnectionChanged;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await store.DisposeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });
            }

            _activity = null;
            _wasConnected = false;
            _afterFirstConnect = false;
            // A new store can restart its IDs. A kept position would look like an entry that never
            // arrives.
            LastSentId = StreamPosition.Zero;
            IsConnected = false;
            IsJoined = false;
            OnPropertyChanged(nameof(Status));
        }

        public void Dispose() => Stop();

        public void SendMessage(IMessage message)
        {
            if (message == null || !IsWriter) return;

            var envelope = new MessageEnvelope
            {
                SenderID = _peerId,
                MessageID = Guid.NewGuid(),
                Payload = message
            };
            _outgoing.Enqueue(MessagePackSerialization.Serialize(envelope));
            OnPropertyChanged(nameof(PendingMessages));
            MessageSent?.Invoke(message);
        }

        // Pull: adopt the store state, then follow the stream.
        public Task JoinAsync() => JoinAsync(_cts?.Token ?? CancellationToken.None, false);

        // Push: publish the local state as the new store state, then follow the stream.
        public Task PushAsync() => PushAsync(_cts?.Token ?? CancellationToken.None);

        // Compares the local state with the store state. It reads only, and changes nothing.
        public async Task<StartCheck> CheckStartAsync()
        {
            var store = _store;
            if (store == null) return StartCheck.NotInSync;

            var result = await EvaluateStartAsync(store).ConfigureAwait(false);
            return result.Check;
        }

        private async Task RunAsync(ISyncStore store, int generation, CancellationToken ct)
        {
            try
            {
                store.ConnectionChanged += OnConnectionChanged;
                await WaitForConnectionAsync(store, ct).ConfigureAwait(false);
                if (!IsCurrent(store, generation)) return;

                _afterFirstConnect = true;
                await DispatchCurrentAsync(store, generation, () =>
                {
                    _wasConnected = true;
                    IsConnected = true;
                }).ConfigureAwait(false);

                _ = Task.Run(() => _outgoing.RunAsync(store, OnSentAsync, ct), ct);
                var presence = new Presence(store, _peerId, HeartbeatFields, () => ListPeersEnabled,
                    tick => OnPresenceTickAsync(store, generation, tick));
                _ = Task.Run(() => presence.RunAsync(ct), ct);

                await OnConnectedAsync(ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DispatchCurrentAsync(store, generation, () => ErrorMessage = ex.Message).ConfigureAwait(false);
            }
        }

        private async Task WaitForConnectionAsync(ISyncStore store, CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                await store.ConnectAsync(ct).ConfigureAwait(false);
                if (store.IsConnected) return;

                await WaitForConnectionEventAsync(store, SyncTimings.MaxBackoff, ct).ConfigureAwait(false);
                if (store.IsConnected) return;
            }
        }

        private static async Task WaitForConnectionEventAsync(ISyncStore store, TimeSpan timeout, CancellationToken ct)
        {
            var connected = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            void OnChanged(bool value)
            {
                if (value) connected.TrySetResult(true);
            }

            store.ConnectionChanged += OnChanged;
            try
            {
                await Task.WhenAny(connected.Task, Task.Delay(timeout, ct)).ConfigureAwait(false);
            }
            finally
            {
                store.ConnectionChanged -= OnChanged;
            }
        }

        protected virtual async Task OnConnectedAsync(CancellationToken ct)
        {
            if (!_autoJoin)
            {
                await RunStartCheckAsync(ct).ConfigureAwait(false);
                return;
            }

            var store = _store;
            var generation = Volatile.Read(ref _generation);
            if (store == null) return;

            // A store error makes the join fail. An engine has no user who repeats it, so it tries
            // again until it is in the sync.
            var backoff = SyncTimings.MinBackoff;
            while (!ct.IsCancellationRequested && !IsJoined)
            {
                if (!IsCurrent(store, generation)) return;

                if (store.IsConnected)
                {
                    await JoinAsync(ct, true).ConfigureAwait(false);
                    if (!IsCurrent(store, generation)) return;
                    if (IsJoined) return;

                    await Task.Delay(backoff, ct).ConfigureAwait(false);
                }
                else
                {
                    await WaitForConnectionEventAsync(store, backoff, ct).ConfigureAwait(false);
                }

                backoff = TimeSpan.FromTicks(Math.Min(backoff.Ticks * 2, SyncTimings.MaxBackoff.Ticks));
            }
        }

        // Position is the stream position the peer keeps when it is already in sync.
        private async Task<(StartCheck Check, StreamPosition Position)> EvaluateStartAsync(ISyncStore store)
        {
            var snapshot = await store.ReadSnapshotAsync().ConfigureAwait(false);
            var tail = await store.ReadTailAsync().ConfigureAwait(false);

            if (snapshot == null)
                return (tail == StreamPosition.Zero ? StartCheck.PushSilently : StartCheck.NotInSync, tail);

            // Entries after the snapshot are edits the local state does not have.
            if (tail > snapshot.StreamId)
                return (StartCheck.NotInSync, tail);

            var local = await DispatchAsync(() => _target.Capture()).ConfigureAwait(false);
            var stored = MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));
            var same = ProjectStateHash.Compute(local) == ProjectStateHash.Compute(stored);
            return (same ? StartCheck.AlreadyInSync : StartCheck.NotInSync, snapshot.StreamId);
        }

        // Acts on the start check. NotInSync leaves the peer blocked until the user pushes or pulls.
        // Runs under the join lock, so a connection event during the check cannot start a second
        // check, and a queued check after a join does nothing.
        private async Task RunStartCheckAsync(CancellationToken ct)
        {
            var store = _store;
            var generation = Volatile.Read(ref _generation);
            if (store == null) return;

            if (!await WaitJoinLockAsync(ct).ConfigureAwait(false)) return;
            try
            {
                if (!IsCurrent(store, generation) || IsJoined) return;

                await SetActivityAsync("Checking").ConfigureAwait(false);
                var result = await EvaluateStartAsync(store).ConfigureAwait(false);
                if (!IsCurrent(store, generation)) return;

                await SetActivityAsync(null).ConfigureAwait(false);

                switch (result.Check)
                {
                    case StartCheck.PushSilently:
                        await PushLockedAsync(store, generation).ConfigureAwait(false);
                        break;
                    case StartCheck.NotInSync:
                        await DispatchCurrentAsync(store, generation, () => IsJoined = false).ConfigureAwait(false);
                        break;
                    default:
                        await StopFollowerAsync().ConfigureAwait(false);
                        var joined = await DispatchCurrentAsync(store, generation, () =>
                        {
                            LastAppliedId = result.Position;
                            TailId = result.Position;
                            ErrorMessage = string.Empty;
                            IsJoined = true;
                        }).ConfigureAwait(false);
                        if (joined)
                            StartFollower(store, generation);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DispatchCurrentAsync(store, generation, () => ErrorMessage = ex.Message).ConfigureAwait(false);
            }
            finally
            {
                await SetActivityAsync(null).ConfigureAwait(false);
                _joinLock.Release();
            }
        }

        private void OnConnectionChanged(bool connected)
        {
            Dispatch(() =>
            {
                if (connected) _wasConnected = true;
                IsConnected = connected;
                OnPropertyChanged(nameof(Status));

                // A compaction that threw while the store was down is not retried by itself.
                if (connected && _compactor != null && _compactor.HasFailed)
                    _compactor.Request();
            });

            // A reconnect while the peer is not in sync. The store state can have changed.
            if (connected && _started && _afterFirstConnect && !IsJoined)
            {
                var ct = _cts?.Token ?? CancellationToken.None;
                if (_autoJoin)
                    _ = Task.Run(() => JoinAsync(ct, true), ct);
                else
                    _ = Task.Run(() => RunStartCheckAsync(ct), ct);
            }
        }

        // The new position comes before the count, so a reader of both never sees a caught-up peer
        // while the entry is still on its way.
        private Task OnSentAsync(StreamPosition id) => DispatchAsync(() =>
        {
            SentMessages++;
            LastSentId = id;
            OnPropertyChanged(nameof(PendingMessages));
        });

        private IReadOnlyDictionary<string, string> HeartbeatFields() => new Dictionary<string, string>
        {
            ["name"] = Name,
            ["role"] = Role,
            ["host"] = Environment.MachineName,
            ["lastAppliedId"] = LastAppliedId.ToString()
        };

        private async Task OnPresenceTickAsync(ISyncStore store, int generation, PresenceTick tick)
        {
            var gap = false;
            await DispatchCurrentAsync(store, generation, () =>
            {
                if (tick.Tail > TailId) TailId = tick.Tail;
                if (tick.Peers != null) ReplacePeers(tick.Peers);
                gap = IsJoined && HasGap(tick.SnapshotId, tick.Oldest);

                // The store lost the snapshot, for example after a restart without the data. The
                // writer makes it again, or a late peer finds nothing to join from.
                if (IsJoined && _compactor != null && tick.SnapshotId == StreamPosition.Zero)
                    _compactor.Request();
            }).ConfigureAwait(false);

            if (gap)
                await JoinAsync().ConfigureAwait(false);
        }

        // The stream was trimmed past the own position when its oldest entry is newer than that
        // position and the snapshot is ahead. Then the entries in between are gone, and only the
        // snapshot has the state. An empty stream has no gap.
        private bool HasGap(StreamPosition snapshotId, StreamPosition? oldest)
            => snapshotId > LastAppliedId && oldest.HasValue && oldest.Value > LastAppliedId;

        private void ReplacePeers(IReadOnlyList<PeerInfo> peers)
        {
            Peers.Clear();
            foreach (var peer in peers)
                Peers.Add(peer);
        }

        // skipWhenJoined is for the auto-join. A try that waited for the lock does nothing when
        // another try joined already. A Pull and the gap re-join always run.
        private async Task JoinAsync(CancellationToken ct, bool skipWhenJoined)
        {
            var store = _store;
            var generation = Volatile.Read(ref _generation);
            if (store == null) return;

            if (!await WaitJoinLockAsync(ct).ConfigureAwait(false)) return;
            try
            {
                if (!IsCurrent(store, generation)) return;
                if (skipWhenJoined && IsJoined) return;

                await StopFollowerAsync().ConfigureAwait(false);
                await SetActivityAsync("Joining").ConfigureAwait(false);

                var snapshot = await store.ReadSnapshotAsync().ConfigureAwait(false);
                if (!IsCurrent(store, generation)) return;

                var tail = await store.ReadTailAsync().ConfigureAwait(false);
                if (!IsCurrent(store, generation)) return;

                // Nothing to adopt: the local state becomes the store state. A non-writer keeps
                // its local state and follows the stream from the start instead.
                if (snapshot == null && tail == StreamPosition.Zero)
                {
                    if (IsWriter)
                        await PushCoreAsync(store, generation).ConfigureAwait(false);
                    else
                        await DispatchCurrentAsync(store, generation, () =>
                        {
                            LastAppliedId = StreamPosition.Zero;
                            TailId = StreamPosition.Zero;
                        }).ConfigureAwait(false);

                    if (!IsCurrent(store, generation)) return;
                }
                else
                {
                    var position = StreamPosition.Zero;
                    if (snapshot != null)
                    {
                        var model = MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));
                        position = snapshot.StreamId;
                        await DispatchCurrentAsync(store, generation, () =>
                        {
                            _target.ApplySnapshot(model);
                            LastAppliedId = position;
                            if (position > TailId) TailId = position;
                        }).ConfigureAwait(false);
                    }
                    else
                    {
                        await DispatchCurrentAsync(store, generation, () => LastAppliedId = StreamPosition.Zero).ConfigureAwait(false);
                    }

                    if (!IsCurrent(store, generation)) return;

                    await ReplayAsync(store, generation, position, ct).ConfigureAwait(false);
                    if (!IsCurrent(store, generation)) return;
                }

                var joined = await DispatchCurrentAsync(store, generation, () =>
                {
                    ErrorMessage = string.Empty;
                    IsJoined = true;
                }).ConfigureAwait(false);
                if (joined)
                    StartFollower(store, generation);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DispatchCurrentAsync(store, generation, () => ErrorMessage = ex.Message).ConfigureAwait(false);
            }
            finally
            {
                await SetActivityAsync(null).ConfigureAwait(false);
                _joinLock.Release();
            }
        }

        private async Task PushAsync(CancellationToken ct)
        {
            if (!IsWriter)
            {
                await DispatchAsync(() => ErrorMessage = "This peer does not write.").ConfigureAwait(false);
                return;
            }

            var store = _store;
            var generation = Volatile.Read(ref _generation);
            if (store == null) return;

            if (!await WaitJoinLockAsync(ct).ConfigureAwait(false)) return;
            try
            {
                if (!IsCurrent(store, generation)) return;

                await PushLockedAsync(store, generation).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DispatchCurrentAsync(store, generation, () => ErrorMessage = ex.Message).ConfigureAwait(false);
            }
            finally
            {
                await SetActivityAsync(null).ConfigureAwait(false);
                _joinLock.Release();
            }
        }

        // The caller holds the join lock.
        private async Task PushLockedAsync(ISyncStore store, int generation)
        {
            await StopFollowerAsync().ConfigureAwait(false);
            await SetActivityAsync("Pushing").ConfigureAwait(false);
            await PushCoreAsync(store, generation).ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;

            var joined = await DispatchCurrentAsync(store, generation, () =>
            {
                ErrorMessage = string.Empty;
                IsJoined = true;
            }).ConfigureAwait(false);
            if (joined)
                StartFollower(store, generation);
        }

        // Appends the full model as one entry for the running peers, then writes the snapshot for
        // the late ones, then trims everything older.
        private async Task PushCoreAsync(ISyncStore store, int generation)
        {
            var model = await DispatchAsync(() => _target.Capture()).ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;

            var envelope = new MessageEnvelope
            {
                SenderID = _peerId,
                MessageID = Guid.NewGuid(),
                Payload = new MessageProjectSnapshot(Guid.NewGuid(), model)
            };

            var id = await store.AppendAsync(MessagePackSerialization.Serialize(envelope)).ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;

            var snapshot = new Snapshot(MessagePackSerialization.Serialize(model), id, _peerId, DateTime.UtcNow);
            await store.WriteSnapshotAsync(snapshot).ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;

            await store.TrimAsync(id).ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;

            await DispatchCurrentAsync(store, generation, () =>
            {
                LastSentId = id;
                LastAppliedId = id;
                TailId = id;
                SentMessages++;
            }).ConfigureAwait(false);
        }

        private async Task ReplayAsync(ISyncStore store, int generation, StreamPosition position, CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                var entries = await store.ReadRangeAsync(position, ReplayBatch).ConfigureAwait(false);
                if (entries.Count == 0) return;
                if (!IsCurrent(store, generation)) return;

                foreach (var entry in entries)
                {
                    await ApplyEntryAsync(entry).ConfigureAwait(false);
                    if (!IsCurrent(store, generation)) return;

                    position = entry.Id;
                }
            }
        }

        private async Task ApplyEntryAsync(StreamEntry entry)
        {
            MessageEnvelope envelope = null;
            try
            {
                envelope = MessagePackSerialization.Deserialize<MessageEnvelope>(new ReadOnlyMemory<byte>(entry.Envelope));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            var payload = envelope?.Payload;
            var own = envelope == null || envelope.SenderID == _peerId || payload == null;

            await DispatchAsync(() =>
            {
                if (!own)
                {
                    try
                    {
                        _target.Apply(payload);
                        AppliedMessages++;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        ErrorMessage = ex.Message;
                    }
                }

                LastAppliedId = entry.Id;
                if (entry.Id > TailId) TailId = entry.Id;

                if (!own) MessageApplied?.Invoke(entry, payload);
            }).ConfigureAwait(false);
        }

        private void StartFollower(ISyncStore store, int generation)
        {
            var cts = _cts;
            if (cts == null || cts.IsCancellationRequested) return;

            var followerCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
            var follower = new StreamFollower(store, () => LastAppliedId, ApplyEntryAsync, () => OnFollowerErrorAsync(store, generation));
            _followerCts = followerCts;
            _followerTask = Task.Run(() => follower.RunAsync(followerCts.Token), followerCts.Token);
        }

        private async Task StopFollowerAsync()
        {
            var cts = _followerCts;
            var task = _followerTask;
            _followerCts = null;
            _followerTask = null;
            if (cts == null) return;

            // The source is not disposed. Stop can still hold a reference to it, and a cancelled
            // linked source without a timer costs nearly nothing.
            Cancel(cts);
            if (task != null)
            {
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        // After a store error: show the state, and re-apply the snapshot only when the stream was
        // trimmed past the own position while the peer was away. Otherwise the reader replays.
        private async Task OnFollowerErrorAsync(ISyncStore store, int generation)
        {
            await DispatchCurrentAsync(store, generation, () =>
            {
                IsConnected = store.IsConnected;
                OnPropertyChanged(nameof(Status));
            }).ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;

            var snapshotId = await store.ReadSnapshotIdAsync().ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;
            if (snapshotId <= LastAppliedId) return;

            var first = await store.ReadRangeAsync(StreamPosition.Zero, 1).ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;
            if (!HasGap(snapshotId, first.Count > 0 ? first[0].Id : null)) return;

            var snapshot = await store.ReadSnapshotAsync().ConfigureAwait(false);
            if (!IsCurrent(store, generation)) return;
            if (snapshot == null || snapshot.StreamId <= LastAppliedId) return;

            var model = MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));
            await DispatchCurrentAsync(store, generation, () =>
            {
                _target.ApplySnapshot(model);
                AppliedMessages++;
                LastAppliedId = snapshot.StreamId;
                if (snapshot.StreamId > TailId) TailId = snapshot.StreamId;
            }).ConfigureAwait(false);
        }

        // False after a Stop, or after a Start that made a new store. Then the caller belongs to an
        // earlier run and must change nothing.
        private bool IsCurrent(ISyncStore store, int generation)
            => ReferenceEquals(_store, store) && Volatile.Read(ref _generation) == generation;

        // Makes the state change only when the run is still the current one. Returns false when it
        // did nothing. The dispatcher and Start and Stop use the same thread, so the check and the
        // change cannot be separated.
        private Task<bool> DispatchCurrentAsync(ISyncStore store, int generation, Action action) => DispatchAsync(() =>
        {
            if (!IsCurrent(store, generation)) return false;

            action();
            return true;
        });

        // Returns false when the peer stopped while the caller waited for the lock.
        private async Task<bool> WaitJoinLockAsync(CancellationToken ct)
        {
            try
            {
                await _joinLock.WaitAsync(ct).ConfigureAwait(false);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        // A source that another task disposed is already cancelled.
        private static void Cancel(CancellationTokenSource cts)
        {
            try
            {
                cts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private Task SetActivityAsync(string activity) => DispatchAsync(() =>
        {
            _activity = activity;
            OnPropertyChanged(nameof(Status));
        });

        private void CreateCompactor()
        {
            if (_compactor != null || !_compactionEnabled || _store == null) return;

            _compactor = new SnapshotCompactor(this, _target, _store, action => DispatchAsync(action), CompactionDelay);
        }

        private void DisposeCompactor()
        {
            var compactor = _compactor;
            _compactor = null;
            compactor?.Dispose();
        }

        private void Dispatch(Action action)
        {
            if (_dispatcher != null)
                _dispatcher(action);
            else
                action();
        }

        private Task DispatchAsync(Action action) => DispatchAsync(() =>
        {
            action();
            return true;
        });

        private Task<T> DispatchAsync<T>(Func<T> func)
        {
            if (_dispatcher == null)
                return Task.FromResult(func());

            var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            _dispatcher(() =>
            {
                try
                {
                    completion.SetResult(func());
                }
                catch (Exception ex)
                {
                    completion.SetException(ex);
                }
            });
            return completion.Task;
        }
    }
}
