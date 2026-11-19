// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
        private sealed record DecodedEntry(StreamEntry Entry, MessageEnvelope Envelope, bool IsOwn);

        private readonly ISyncTarget _target;
        private readonly ControlMessenger _messenger;
        private readonly Func<SyncOptions, ISyncStore> _storeFactory;
        private readonly OutgoingQueue _outgoing = new();
        private readonly SemaphoreSlim _joinLock = new(1, 1);
        private readonly string _peerId;
        // Only the dispatcher touches it.
        private readonly AppliedMemory _applied = new();

        // The run of the last Start. Null after a Stop. A caller of an earlier run must change nothing.
        private SyncRun _run;
        private SnapshotCompactor _compactor;
        private bool _compactionEnabled;
        private Task _runTask;
        private TaskCompletionSource<bool> _stopped;
        private Action<Action> _dispatcher;
        private bool _started;
        private bool _wasConnected;
        private bool _afterFirstConnect;
        // True while the follower recovery runs. The recovery and the heartbeat run on different
        // threads, so both use Volatile for this field.
        private bool _recovering;
        private string _activity;
        // The last store reason the peer showed. A clear must not wipe another message.
        private string _storeError = string.Empty;

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

        // The time values of the sync. Start takes the set that its run uses.
        public SyncTimings Timings { get; set; } = SyncTimings.Default;

        // How long a value change waits before the compactor writes a new snapshot. Tests shorten it.
        public TimeSpan CompactionDelay
        {
            get => Timings.CompactionDelay;
            set => Timings = Timings with { CompactionDelay = value };
        }

        // How many entries one read returns, and so how many one dispatched action applies. The
        // patch sets it before Start.
        public int ReadBatch
        {
            get => Timings.ReadBatch;
            set => Timings = Timings with { ReadBatch = value };
        }

        // A follower that made no read for this long checks the gap first. Tests shorten it.
        public TimeSpan FollowerStalePause
        {
            get => Timings.StalePause;
            set => Timings = Timings with { StalePause = value };
        }

        public int PendingMessages => _outgoing.PendingCount;

        // Completes when the run of the last Start has ended: the loops have returned and the store
        // is disposed. Before the first Start it is a completed task. A test waits for it before it
        // deletes the keys of the run.
        public Task Stopped => _stopped?.Task ?? Task.CompletedTask;

        public event EventHandler<MessageAppliedEventArgs> MessageApplied;
        public event EventHandler<MessageSentEventArgs> MessageSent;

        // Comes after the target applied a snapshot, on the dispatcher thread. A listener empties
        // what the snapshot does not carry. The events follow the .NET event pattern, so vvvv
        // shows them as observable nodes.
        public event EventHandler SnapshotApplied;

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

        // The queue appends only while the peer is joined, so entries from before a restart never
        // land on a store the user has not chosen yet.
        partial void OnIsJoinedChanged(bool value)
        {
            _messenger.IsSendingBlocked = !value;
            _outgoing.SetOpen(value);
        }

        public bool IsInSync => IsConnected && IsJoined && LastAppliedId == TailId;

        public string Status =>
            !_started ? "Not started" :
            !IsConnected ? (_wasConnected ? "Reconnecting" : "Connecting") :
            _activity != null ? _activity :
            !IsJoined ? "Not in sync" : "Connected";

        public void SetDispatcher(Action<Action> dispatcher) => _dispatcher = dispatcher;

        // Returns at once. A background task connects, joins when asked, and starts the loops.
        public void Start(SyncOptions options)
        {
            Stop();
            Options = options.WithFallbacks();
            StartWith(new SyncRun(_storeFactory(Options), Timings, new CancellationTokenSource(), false));
        }

        public void StartAndJoin(SyncOptions options)
        {
            Stop();
            Options = options.WithFallbacks();
            StartWith(new SyncRun(_storeFactory(Options), Timings, new CancellationTokenSource(), true));
        }

        private void StartWith(SyncRun run)
        {
            Volatile.Write(ref _run, run);
            CreateCompactor();
            _stopped = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _started = true;
            _wasConnected = false;
            _afterFirstConnect = false;
            _storeError = string.Empty;
            OnPropertyChanged(nameof(Options));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Role));
            OnPropertyChanged(nameof(Status));

            _runTask = Task.Run(() => RunAsync(run), run.Cts.Token);
        }

        // Returns at once. The loops end and the store is disposed on a background task. Stopped
        // completes when that work is done. Queued messages stay queued.
        public void Stop()
        {
            if (!_started) return;

            _started = false;
            var compactor = _compactor;
            DisposeCompactor();
            // The dispose blocks a new compaction, so this is the last one.
            var compacting = compactor?.Running ?? Task.CompletedTask;
            var run = _run;
            var followerTask = run.FollowerTask;
            var runTask = _runTask;
            var stopped = _stopped;
            var store = run.Store;
            var stopTimeout = run.Timings.StopTimeout;
            Volatile.Write(ref _run, null);
            _runTask = null;

            Cancel(run.FollowerCts);
            Cancel(run.Cts);

            store.ConnectionChanged -= OnConnectionChanged;

            _ = Task.Run(async () =>
            {
                try
                {
                    // The loops and the compaction must end before the store closes. Otherwise a
                    // last heartbeat, append or snapshot lands after the stop.
                    await Task.WhenAny(WaitForAsync(runTask, followerTask, compacting), Task.Delay(stopTimeout)).ConfigureAwait(false);

                    await store.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    stopped?.TrySetResult(true);
                }
            });

            _activity = null;
            _wasConnected = false;
            _afterFirstConnect = false;
            _storeError = string.Empty;

            // The loops write the state through the dispatcher. The reset goes the same way, so it
            // comes after a write of a loop that is already queued. It posts and does not wait.
            Dispatch(() =>
            {
                // A new store can restart its IDs. A kept position would look like an entry that
                // never arrives, or like a gap.
                LastSentId = StreamPosition.Zero;
                LastAppliedId = StreamPosition.Zero;
                TailId = StreamPosition.Zero;
                IsConnected = false;
                IsJoined = false;
                Peers.Clear();
                OnPropertyChanged(nameof(Status));
            });
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
            // Through the dispatcher, so this notification and the one after the append keep their order.
            Dispatch(() => OnPropertyChanged(nameof(PendingMessages)));
            MessageSent?.Invoke(this, new MessageSentEventArgs(message));
        }

        // Pull: adopt the store state, then follow the stream.
        public Task JoinAsync()
        {
            var run = Volatile.Read(ref _run);
            if (run == null) return Task.CompletedTask;

            return JoinAsync(run);
        }

        // Push: publish the local state as the new store state, then follow the stream.
        public Task PushAsync() => PushAsync(Volatile.Read(ref _run));

        // Compares the local state with the store state. It reads only, and changes nothing.
        public async Task<StartCheck> CheckStartAsync()
        {
            var run = Volatile.Read(ref _run);
            if (run == null) return StartCheck.NotInSync;

            var result = await EvaluateStartAsync(run.Store).ConfigureAwait(false);
            return result.Check;
        }

        // Returns when the run has ended, so the stop work can wait for the loops it started.
        private async Task RunAsync(SyncRun run)
        {
            var store = run.Store;
            var timings = run.Timings;
            var ct = run.Cts.Token;
            Task outgoing = null;
            Task presence = null;

            try
            {
                store.ConnectionChanged += OnConnectionChanged;
                await WaitForConnectionAsync(store, timings, ct).ConfigureAwait(false);
                if (!IsCurrent(run)) return;

                _afterFirstConnect = true;
                await DispatchCurrentAsync(run, () =>
                {
                    _wasConnected = true;
                    IsConnected = true;
                }).ConfigureAwait(false);

                outgoing = Task.Run(() => _outgoing.RunAsync(store, timings, id => OnSentAsync(run, id),
                    () => OnDrainedAsync(run), ct), ct);
                var presenceLoop = new Presence(store, _peerId, HeartbeatFields, () => ListPeersEnabled,
                    () => LastAppliedId, tick => OnPresenceTickAsync(run, tick), timings);
                presence = Task.Run(() => presenceLoop.RunAsync(ct), ct);

                await OnConnectedAsync(run).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DispatchCurrentAsync(run, () => ErrorMessage = ex.Message).ConfigureAwait(false);
            }
            finally
            {
                await WaitForAsync(outgoing, presence).ConfigureAwait(false);
            }
        }

        // Waits for the tasks of a run. A cancelled task is the normal end here.
        private static async Task WaitForAsync(params Task[] tasks)
        {
            foreach (var task in tasks)
            {
                if (task == null) continue;

                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private async Task WaitForConnectionAsync(ISyncStore store, SyncTimings timings, CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                await store.ConnectAsync(ct).ConfigureAwait(false);
                if (store.IsConnected) return;

                // A store that misses the event of the server keeps the peer waiting. The connect
                // of a store that already has its client is cheap, so the poll is short.
                await WaitForConnectionEventAsync(store, timings.HeartbeatInterval, ct).ConfigureAwait(false);
                if (store.IsConnected) return;
            }
        }

        private static async Task WaitForConnectionEventAsync(ISyncStore store, TimeSpan timeout, CancellationToken ct)
        {
            var connected = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            void OnChanged(ISyncStore sender, bool value)
            {
                if (value && ReferenceEquals(sender, store)) connected.TrySetResult(true);
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

        private async Task OnConnectedAsync(SyncRun run)
        {
            if (!run.AutoJoin)
            {
                await RunStartCheckAsync(run).ConfigureAwait(false);
                return;
            }

            var store = run.Store;
            var ct = run.Cts.Token;

            // A store error makes the join fail. An engine has no user who repeats it, so it tries
            // again until it is in the sync.
            var timings = run.Timings;
            var backoff = timings.MinBackoff;
            while (!ct.IsCancellationRequested && !IsJoined)
            {
                if (!IsCurrent(run)) return;

                if (store.IsConnected)
                {
                    await AutoJoinAsync(run).ConfigureAwait(false);
                    if (!IsCurrent(run)) return;
                    if (IsJoined) return;

                    await Task.Delay(backoff, ct).ConfigureAwait(false);
                }
                else
                {
                    await WaitForConnectionEventAsync(store, backoff, ct).ConfigureAwait(false);
                }

                backoff = TimeSpan.FromTicks(Math.Min(backoff.Ticks * 2, timings.MaxBackoff.Ticks));
            }
        }

        // Position is the stream position the peer keeps when it is already in sync.
        private async Task<StartEvaluation> EvaluateStartAsync(ISyncStore store)
        {
            var snapshot = await store.ReadSnapshotAsync().ConfigureAwait(false);
            var tail = await store.ReadTailAsync().ConfigureAwait(false);

            if (snapshot == null)
                return new StartEvaluation(tail == StreamPosition.Zero ? StartCheck.PushSilently : StartCheck.NotInSync, tail);

            // Entries after the snapshot are edits the local state does not have.
            if (tail > snapshot.StreamId)
                return new StartEvaluation(StartCheck.NotInSync, tail);

            var local = await DispatchAsync(() => _target.Capture()).ConfigureAwait(false);
            var stored = MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));
            var same = ProjectStateHash.Compute(local) == ProjectStateHash.Compute(stored);
            return new StartEvaluation(same ? StartCheck.AlreadyInSync : StartCheck.NotInSync, snapshot.StreamId);
        }

        // Acts on the start check. NotInSync leaves the peer blocked until the user pushes or pulls.
        // Runs under the join lock, so a connection event during the check cannot start a second
        // check, and a queued check after a join does nothing.
        private Task RunStartCheckAsync(SyncRun run)
        {
            return RunUnderJoinLockAsync(run, CheckAsync);

            async Task CheckAsync()
            {
                if (IsJoined) return;

                await SetActivityAsync("Checking").ConfigureAwait(false);
                var result = await EvaluateStartAsync(run.Store).ConfigureAwait(false);
                if (!IsCurrent(run)) return;

                await SetActivityAsync(null).ConfigureAwait(false);

                switch (result.Check)
                {
                    case StartCheck.PushSilently:
                        await PushLockedAsync(run).ConfigureAwait(false);
                        break;
                    case StartCheck.NotInSync:
                        await DispatchCurrentAsync(run, () => IsJoined = false).ConfigureAwait(false);
                        break;
                    default:
                        await StopFollowerAsync(run).ConfigureAwait(false);
                        await DispatchCurrentAsync(run, () =>
                        {
                            LastAppliedId = result.Position;
                            TailId = result.Position;
                            ErrorMessage = string.Empty;
                            IsJoined = true;
                            StartFollower(run);
                        }).ConfigureAwait(false);
                        break;
                }
            }
        }

        // A run that subscribes after its Stop leaves a handler on the store it left. The sender
        // tells those events from the events of the current store.
        private void OnConnectionChanged(ISyncStore sender, bool connected)
        {
            var run = Volatile.Read(ref _run);
            if (run == null || !ReferenceEquals(sender, run.Store)) return;

            var reason = sender.LastError ?? string.Empty;

            Dispatch(() =>
            {
                if (connected) _wasConnected = true;
                IsConnected = connected;
                if (connected)
                    ClearStoreError();
                else
                    ShowStoreError(reason);
                OnPropertyChanged(nameof(Status));

                // A compaction that threw while the store was down is not retried by itself.
                if (connected && _compactor != null && _compactor.HasFailed)
                    _compactor.Request();
            });

            // The follower can wait out a long backoff. Only the follower of the current run gets
            // the signal, because Stop and a new join clear the field.
            if (connected)
                run.Follower?.Wake();

            // A reconnect while the peer is not in sync. The store state can have changed.
            if (connected && _started && _afterFirstConnect && !IsJoined)
            {
                var ct = run.Cts.Token;
                if (run.AutoJoin)
                    _ = Task.Run(() => AutoJoinAsync(run), ct);
                else
                    _ = Task.Run(() => RunStartCheckAsync(run), ct);
            }
        }

        // Runs on the dispatcher thread.
        private void ClearStoreError()
        {
            if (ErrorMessage == _storeError)
                ErrorMessage = string.Empty;

            _storeError = string.Empty;
        }

        private void ShowStoreError(string reason)
        {
            if (reason.Length == 0)
                return;

            ErrorMessage = reason;
            _storeError = reason;
        }

        // The position comes first. The count comes after the entry left the queue, so a reader of
        // both never sees a caught-up peer while the entry is still on its way. Returns false when
        // the run is over. The entry then belongs to the store of that run only.
        private Task<bool> OnSentAsync(SyncRun run, StreamPosition id)
            => DispatchCurrentAsync(run, () =>
            {
                SentMessages++;
                LastSentId = id;
            });

        private Task OnDrainedAsync(SyncRun run)
            => DispatchCurrentAsync(run, () => OnPropertyChanged(nameof(PendingMessages)));

        private IReadOnlyDictionary<string, string> HeartbeatFields() => new Dictionary<string, string>
        {
            ["name"] = Name,
            ["role"] = Role,
            ["host"] = Environment.MachineName,
            ["lastAppliedId"] = LastAppliedId.ToString()
        };

        private async Task OnPresenceTickAsync(SyncRun run, PresenceTick tick)
        {
            var gap = false;
            var behindTail = false;
            await DispatchCurrentAsync(run, () =>
            {
                behindTail = IsJoined && IsBehindTail(tick.Tail, tick.Position);
                // The tail of another store is the true one. Every other tick only moves it up.
                if (tick.Tail > TailId || behindTail) TailId = tick.Tail;
                if (tick.Peers != null) ReplacePeers(tick.Peers);
                // The recovery can see the same gap. Then the snapshot is applied twice.
                gap = IsJoined && !Volatile.Read(ref _recovering) && (behindTail || HasGap(tick.SnapshotId, tick.Oldest));

                // The store lost the snapshot, for example after a restart without the data. The
                // writer makes it again, or a late peer finds nothing to join from.
                if (IsJoined && _compactor != null && tick.SnapshotId == StreamPosition.Zero)
                    _compactor.Request();
            }).ConfigureAwait(false);

            // A peer behind the tail of a replacement store must adopt the state of that store, also
            // when its snapshot is below the own position. Only the re-join after a trim can skip.
            if (!gap) return;

            if (behindTail)
                await JoinAsync(run).ConfigureAwait(false);
            else
                await RejoinAfterGapAsync(run).ConfigureAwait(false);
        }

        // The stream was trimmed past the own position when its oldest entry is newer than that
        // position and the snapshot is ahead. Then the entries in between are gone, and only the
        // snapshot has the state. An empty stream has no gap.
        private bool HasGap(StreamPosition snapshotId, StreamPosition? oldest)
            => snapshotId > LastAppliedId && oldest.HasValue && oldest.Value > LastAppliedId;

        // A replacement store with an earlier clock gives IDs below the own position. The reader
        // then waits for entries that never come, so the peer joins the new store from its snapshot.
        // The tail is compared with the position from before the tail was read: own appends after
        // the read, for example during a drag, move the position past that tail in the same store.
        // A join after the read can lower the position, so the lower of the two positions counts.
        private bool IsBehindTail(StreamPosition tail, StreamPosition positionAtRead)
        {
            var position = positionAtRead < LastAppliedId ? positionAtRead : LastAppliedId;
            return tail != StreamPosition.Zero && tail < position;
        }

        private void ReplacePeers(IReadOnlyList<PeerInfo> peers)
        {
            Peers.Clear();
            foreach (var peer in peers)
                Peers.Add(peer);
        }

        private Task JoinAsync(SyncRun run)
            => RunUnderJoinLockAsync(run, () => AdoptStoreAsync(run));

        // The engine joins by itself. A try that waited for the lock does nothing when another try joined already.
        private Task AutoJoinAsync(SyncRun run)
            => RunUnderJoinLockAsync(run, () => IsJoined ? Task.CompletedTask : AdoptStoreAsync(run));

        // The re-join of the heartbeat after a trim. The follower stop and the activity come before the shortcut.
        private Task RejoinAfterGapAsync(SyncRun run)
        {
            var store = run.Store;

            return RunUnderJoinLockAsync(run, RejoinLockedAsync);

            async Task RejoinLockedAsync()
            {
                await StopFollowerAsync(run).ConfigureAwait(false);
                await SetActivityAsync("Joining").ConfigureAwait(false);

                var snapshotId = await store.ReadSnapshotIdAsync().ConfigureAwait(false);
                if (!IsCurrent(run)) return;

                // The follower closed the gap while this join waited for it: its recovery
                // applied the snapshot. A second apply would rebuild the state again, so this
                // join only replays what comes after the position. A store without a snapshot
                // says nothing, so it takes the full join.
                if (snapshotId != StreamPosition.Zero && snapshotId <= LastAppliedId)
                {
                    await ReplayAsync(run, LastAppliedId).ConfigureAwait(false);
                    if (!IsCurrent(run)) return;

                    await FinishJoinAsync(run).ConfigureAwait(false);
                    return;
                }

                await ApplyStoreStateAsync(run).ConfigureAwait(false);
            }
        }

        // The caller holds the join lock.
        private async Task AdoptStoreAsync(SyncRun run)
        {
            await StopFollowerAsync(run).ConfigureAwait(false);
            await SetActivityAsync("Joining").ConfigureAwait(false);
            await ApplyStoreStateAsync(run).ConfigureAwait(false);
        }

        // The caller holds the join lock and has stopped the follower.
        private async Task ApplyStoreStateAsync(SyncRun run)
        {
            var store = run.Store;
            var ct = run.Cts.Token;

            // The store state replaces the local one, so the local edits in the queue go away.
            // An append in flight lands before the read, so it is part of what is adopted.
            _outgoing.Discard();
            await _outgoing.IdleAsync(ct).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var snapshot = await store.ReadSnapshotAsync().ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var tail = await store.ReadTailAsync().ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            // Nothing to adopt: the local state becomes the store state. A non-writer keeps
            // its local state and follows the stream from the start instead.
            if (snapshot == null && tail == StreamPosition.Zero)
            {
                if (IsWriter)
                    await PushCoreAsync(run).ConfigureAwait(false);
                else
                    await DispatchCurrentAsync(run, () =>
                    {
                        LastAppliedId = StreamPosition.Zero;
                        TailId = StreamPosition.Zero;
                    }).ConfigureAwait(false);

                if (!IsCurrent(run)) return;
            }
            else
            {
                var position = StreamPosition.Zero;
                if (snapshot != null)
                {
                    var model = MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));
                    position = snapshot.StreamId;
                    await DispatchCurrentAsync(run, () => ApplySnapshotOnDispatcher(model, position)).ConfigureAwait(false);
                }
                else
                {
                    await DispatchCurrentAsync(run, () => LastAppliedId = StreamPosition.Zero).ConfigureAwait(false);
                }

                if (!IsCurrent(run)) return;

                await ReplayAsync(run, position).ConfigureAwait(false);
                if (!IsCurrent(run)) return;
            }

            await FinishJoinAsync(run).ConfigureAwait(false);
        }

        private async Task PushAsync(SyncRun run)
        {
            if (!IsWriter)
            {
                await DispatchAsync(() => ErrorMessage = "This peer does not write.").ConfigureAwait(false);
                return;
            }

            if (run == null) return;

            await RunUnderJoinLockAsync(run, () => PushLockedAsync(run)).ConfigureAwait(false);
        }

        private async Task RunUnderJoinLockAsync(SyncRun run, Func<Task> body)
        {
            if (!await WaitJoinLockAsync(run.Cts.Token).ConfigureAwait(false)) return;
            try
            {
                if (!IsCurrent(run)) return;

                await body().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DispatchCurrentAsync(run, () => ErrorMessage = ex.Message).ConfigureAwait(false);
            }
            finally
            {
                await SetActivityAsync(null).ConfigureAwait(false);
                _joinLock.Release();
            }
        }

        // The caller holds the join lock.
        private async Task PushLockedAsync(SyncRun run)
        {
            await StopFollowerAsync(run).ConfigureAwait(false);
            await SetActivityAsync("Pushing").ConfigureAwait(false);
            await PushCoreAsync(run).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            await FinishJoinAsync(run).ConfigureAwait(false);
        }

        // Ends a join or a push: the peer is in sync, and the follower reads from the position.
        private Task FinishJoinAsync(SyncRun run)
            => DispatchCurrentAsync(run, () =>
            {
                ErrorMessage = string.Empty;
                IsJoined = true;
                StartFollower(run);
            });

        // Appends the full model as one entry for the running peers, then writes the snapshot for
        // the late ones, then trims everything older.
        private async Task PushCoreAsync(SyncRun run)
        {
            var store = run.Store;

            // The model holds every queued edit, so the queue is emptied. An append in flight must
            // land before the snapshot entry, or a peer applies it on top of the pushed state.
            _outgoing.Discard();
            await _outgoing.IdleAsync(run.Cts.Token).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var model = await DispatchAsync(() => _target.Capture()).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var envelope = new MessageEnvelope
            {
                SenderID = _peerId,
                MessageID = Guid.NewGuid(),
                Payload = new MessageProjectSnapshot(Guid.NewGuid(), model)
            };

            var id = await store.AppendAsync(MessagePackSerialization.Serialize(envelope)).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var snapshot = new Snapshot(MessagePackSerialization.Serialize(model), id, _peerId, DateTime.UtcNow);
            await store.WriteSnapshotAsync(snapshot).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            await store.TrimAsync(id).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            await DispatchCurrentAsync(run, () =>
            {
                LastSentId = id;
                LastAppliedId = id;
                TailId = id;
                SentMessages++;
            }).ConfigureAwait(false);
        }

        private async Task ReplayAsync(SyncRun run, StreamPosition position)
        {
            var ct = run.Cts.Token;
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                var entries = await run.Store.ReadRangeAsync(position, run.Timings.ReadBatch).ConfigureAwait(false);
                if (entries.Count == 0) return;
                if (!IsCurrent(run)) return;

                await ApplyEntriesAsync(run, entries).ConfigureAwait(false);
                if (!IsCurrent(run)) return;

                position = entries[entries.Count - 1].Id;
            }
        }

        // The run is that of the reader that found the entries. Entries that were read before a
        // restart must change nothing after it. The batch runs as one action on the dispatcher
        // thread, and Start and Stop run on that thread too, so one run check at the start of the
        // action covers the whole batch. One action per batch keeps a frame based dispatcher from
        // applying one entry per frame.
        private async Task ApplyEntriesAsync(SyncRun run, IReadOnlyList<StreamEntry> entries)
        {
            if (entries.Count == 0) return;

            var batch = new List<DecodedEntry>(entries.Count);
            foreach (var entry in entries)
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

                var own = envelope == null || envelope.SenderID == _peerId || envelope.Payload == null;
                batch.Add(new DecodedEntry(entry, envelope, own));
            }

            await DispatchCurrentAsync(run, () =>
            {
                foreach (var decoded in batch)
                {
                    var entry = decoded.Entry;
                    var envelope = decoded.Envelope;
                    var own = decoded.IsOwn;
                    var payload = envelope?.Payload;
                    // An append that timed out on the client but reached the store is sent again.
                    // The entry then arrives twice, and a move applied twice gives a wrong order.
                    var apply = !own && !_applied.IsDuplicate(envelope.MessageID);

                    if (apply)
                    {
                        try
                        {
                            _target.Apply(payload);
                            if (payload is MessageProjectSnapshot)
                            {
                                _applied.Forget();
                                SnapshotApplied?.Invoke(this, EventArgs.Empty);
                            }

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

                    if (apply) MessageApplied?.Invoke(this, new MessageAppliedEventArgs(entry, payload));
                }
            }).ConfigureAwait(false);
        }

        private void StartFollower(SyncRun run)
        {
            if (run.Cts.IsCancellationRequested) return;

            var followerCts = CancellationTokenSource.CreateLinkedTokenSource(run.Cts.Token);
            var follower = new StreamFollower(run.Store, () => LastAppliedId, entries => ApplyEntriesAsync(run, entries),
                () => OnFollowerErrorAsync(run), run.Timings);
            run.FollowerCts = followerCts;
            run.Follower = follower;
            run.FollowerTask = Task.Run(() => follower.RunAsync(followerCts.Token), followerCts.Token);
        }

        private async Task StopFollowerAsync(SyncRun run)
        {
            var cts = run.FollowerCts;
            var task = run.FollowerTask;
            run.FollowerCts = null;
            run.Follower = null;
            run.FollowerTask = null;
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

        // After a store error or a long pause: show the state, and re-apply the snapshot only when
        // the stream was trimmed past the own position while the peer was away. Otherwise the
        // reader replays.
        private async Task OnFollowerErrorAsync(SyncRun run)
        {
            var store = run.Store;
            Volatile.Write(ref _recovering, true);
            try
            {
                await DispatchCurrentAsync(run, () =>
                {
                    IsConnected = store.IsConnected;
                    OnPropertyChanged(nameof(Status));
                }).ConfigureAwait(false);
                if (!IsCurrent(run)) return;

                var snapshotId = await store.ReadSnapshotIdAsync().ConfigureAwait(false);
                if (!IsCurrent(run)) return;
                if (snapshotId <= LastAppliedId) return;

                var first = await store.ReadRangeAsync(StreamPosition.Zero, 1).ConfigureAwait(false);
                if (!IsCurrent(run)) return;
                if (!HasGap(snapshotId, first.Count > 0 ? first[0].Id : null)) return;

                var snapshot = await store.ReadSnapshotAsync().ConfigureAwait(false);
                if (!IsCurrent(run)) return;
                if (snapshot == null || snapshot.StreamId <= LastAppliedId) return;

                var model = MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));
                await DispatchCurrentAsync(run, () =>
                {
                    ApplySnapshotOnDispatcher(model, snapshot.StreamId);
                    AppliedMessages++;
                }).ConfigureAwait(false);
            }
            finally
            {
                Volatile.Write(ref _recovering, false);
            }
        }

        private void ApplySnapshotOnDispatcher(ProjectModel model, StreamPosition position)
        {
            _target.ApplySnapshot(model);
            _applied.Forget();
            SnapshotApplied?.Invoke(this, EventArgs.Empty);
            LastAppliedId = position;
            if (position > TailId) TailId = position;
        }

        // False after a Stop, or after a Start that made a new run. Then the caller belongs to an
        // earlier run and must change nothing.
        private bool IsCurrent(SyncRun run) => ReferenceEquals(Volatile.Read(ref _run), run);

        // Makes the state change only when the run is still the current one. Returns false when it
        // did nothing. The dispatcher and Start and Stop use the same thread, so the check and the
        // change cannot be separated.
        private Task<bool> DispatchCurrentAsync(SyncRun run, Action action) => DispatchAsync(() =>
        {
            if (!IsCurrent(run)) return false;

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
            var run = _run;
            if (_compactor != null || !_compactionEnabled || run == null) return;

            _compactor = new SnapshotCompactor(this, _target, run.Store, action => DispatchAsync(action), run.Timings);
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
