// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.ComponentModel;
using System.Diagnostics;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using VL.Serialization.MessagePack;

namespace CMiX.Core.Networking
{
    // Writes a new snapshot and trims the stream, so a late peer does not replay hours of edits.
    // Only the Studio does this. A structural message compacts at once. A value change compacts
    // after the delay, and more value changes in that window do not restart it.
    public sealed class SnapshotCompactor : IDisposable
    {
        private readonly SyncPeer _peer;
        private readonly ISyncTarget _target;
        private readonly ISyncStore _store;
        private readonly Func<Action, Task> _dispatch;
        private readonly SyncTimings _timings;
        private readonly Timer _timer;
        private readonly object _gate = new();

        private bool _running;
        private Task _runTask = Task.CompletedTask;
        private bool _dirty;
        private bool _pending;
        private bool _deferred;
        private bool _failed;
        private bool _disposed;

        public SnapshotCompactor(SyncPeer peer, ISyncTarget target, ISyncStore store, Func<Action, Task> dispatch, SyncTimings timings)
        {
            _peer = peer;
            _target = target;
            _store = store;
            _dispatch = dispatch;
            _timings = timings;
            _timer = new Timer(_ => OnTimer(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            _peer.MessageApplied += OnMessageApplied;
            _peer.MessageSent += OnMessageSent;
            _peer.PropertyChanged += OnPeerPropertyChanged;
        }

        // True while a compaction runs and a newer message asks for one more.
        public bool IsDirty
        {
            get
            {
                lock (_gate)
                    return _dirty;
            }
        }

        // The compaction that runs now, or a completed task. The stop work of the peer waits for it,
        // so no write reaches a store that closes.
        public Task Running
        {
            get
            {
                lock (_gate)
                    return _runTask;
            }
        }

        // True when the last compaction threw, so the store can miss the newest snapshot.
        public bool HasFailed
        {
            get
            {
                lock (_gate)
                    return _failed;
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                if (_disposed) return;

                _disposed = true;
                _pending = false;
                _dirty = false;
                _deferred = false;
            }

            _peer.MessageApplied -= OnMessageApplied;
            _peer.MessageSent -= OnMessageSent;
            _peer.PropertyChanged -= OnPeerPropertyChanged;
            _timer.Dispose();
        }

        // Captures the model and the positions on the dispatcher thread, then writes and trims.
        public async Task CompactAsync()
        {
            ProjectModel model = null;
            var lastAppliedId = StreamPosition.Zero;
            var tailId = StreamPosition.Zero;

            await _dispatch(() =>
            {
                if (!_peer.IsJoined) return;

                // The model already holds the own edits. The snapshot must not take a position
                // below the entries of those edits, or a late peer replays them on top.
                if (!IsCaughtUp())
                {
                    lock (_gate)
                        _deferred = true;

                    return;
                }

                model = _target.Capture();
                lastAppliedId = _peer.LastAppliedId;
                tailId = _peer.TailId;
            }).ConfigureAwait(false);

            if (model == null) return;

            var bytes = MessagePackSerialization.Serialize(model);
            await _store.WriteSnapshotAsync(new Snapshot(bytes, lastAppliedId, _peer.PeerId.ToString(), DateTime.UtcNow)).ConfigureAwait(false);
            await _store.TrimAsync(TrimPosition(lastAppliedId, tailId, _timings.Retention)).ConfigureAwait(false);
        }

        // Keeps the last retention of entries, so a peer that lags a little does not lose them, and
        // never drops an entry the own state does not have yet.
        internal static StreamPosition TrimPosition(StreamPosition lastAppliedId, StreamPosition tailId, TimeSpan retention)
        {
            var oldest = tailId.Milliseconds - (long)retention.TotalMilliseconds;
            var byRetention = oldest <= 0 ? StreamPosition.Zero : new StreamPosition(oldest, 0);
            return byRetention < lastAppliedId ? byRetention : lastAppliedId;
        }

        // True when every own entry is in the store and back in the own state.
        private bool IsCaughtUp() => _peer.PendingMessages == 0 && _peer.LastAppliedId >= _peer.LastSentId;

        // Runs on the dispatcher thread. It only releases a deferred capture.
        private void OnPeerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SyncPeer.LastAppliedId) && e.PropertyName != nameof(SyncPeer.PendingMessages))
                return;

            lock (_gate)
            {
                if (_disposed || !_deferred || !IsCaughtUp()) return;

                _deferred = false;
            }

            Request();
        }

        private void OnMessageApplied(object sender, MessageAppliedEventArgs e) => OnMessage(e.Message);

        private void OnMessageSent(object sender, MessageSentEventArgs e) => OnMessage(e.Message);

        private void OnMessage(IMessage message)
        {
            if (message == null) return;

            if (message is IMessageManager or MessageProjectSnapshot)
            {
                StopTimer();
                Request();
                return;
            }

            StartTimer();
        }

        private void StartTimer()
        {
            lock (_gate)
            {
                if (_disposed || _pending) return;

                _pending = true;
                _timer.Change(_timings.CompactionDelay, Timeout.InfiniteTimeSpan);
            }
        }

        private void StopTimer()
        {
            lock (_gate)
            {
                if (_disposed || !_pending) return;

                _pending = false;
                _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
        }

        private void OnTimer()
        {
            lock (_gate)
                _pending = false;

            Request();
        }

        // One compaction at a time. A request during a run makes the loop run once more.
        public void Request()
        {
            lock (_gate)
            {
                if (_disposed) return;

                if (_running)
                {
                    _dirty = true;
                    return;
                }

                _running = true;
                _runTask = Task.Run(RunAsync);
            }
        }

        private async Task RunAsync()
        {
            while (true)
            {
                try
                {
                    await CompactAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // The store is down. The peer asks again when it is back.
                    Debug.WriteLine(ex);
                    lock (_gate)
                    {
                        _failed = true;
                        _dirty = false;
                        _running = false;
                    }

                    return;
                }

                lock (_gate)
                {
                    _failed = false;

                    if (!_dirty || _disposed)
                    {
                        _running = false;
                        return;
                    }

                    _dirty = false;
                }
            }
        }
    }
}
