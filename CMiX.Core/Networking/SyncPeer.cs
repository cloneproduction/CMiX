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
