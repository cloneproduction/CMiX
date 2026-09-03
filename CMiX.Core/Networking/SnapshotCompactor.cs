// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
        private readonly TimeSpan _delay;
        private readonly Timer _timer;
        private readonly object _gate = new();

        private bool _running;
        private bool _dirty;
        private bool _pending;
        private bool _disposed;

        public SnapshotCompactor(SyncPeer peer, ISyncTarget target, ISyncStore store, Func<Action, Task> dispatch, TimeSpan? delay = null)
        {
            _peer = peer;
            _target = target;
            _store = store;
            _dispatch = dispatch;
            _delay = delay ?? SyncTimings.CompactionDelay;
            _timer = new Timer(_ => OnTimer(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            _peer.MessageApplied += OnMessageApplied;
            _peer.MessageSent += OnMessageSent;
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

        public void Dispose()
        {
            lock (_gate)
            {
                if (_disposed) return;

                _disposed = true;
                _pending = false;
                _dirty = false;
            }

            _peer.MessageApplied -= OnMessageApplied;
            _peer.MessageSent -= OnMessageSent;
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

                model = _target.Capture();
                lastAppliedId = _peer.LastAppliedId;
                tailId = _peer.TailId;
            }).ConfigureAwait(false);

            if (model == null) return;

            var bytes = MessagePackSerialization.Serialize(model);
            await _store.WriteSnapshotAsync(new Snapshot(bytes, lastAppliedId, _peer.PeerId.ToString(), DateTime.UtcNow)).ConfigureAwait(false);
            await _store.TrimAsync(TrimPosition(lastAppliedId, tailId)).ConfigureAwait(false);
        }

        // Keeps the last Retention of entries, so a peer that lags a little does not lose them, and
        // never drops an entry the own state does not have yet.
        internal static StreamPosition TrimPosition(StreamPosition lastAppliedId, StreamPosition tailId)
        {
            var oldest = tailId.Milliseconds - (long)SyncTimings.Retention.TotalMilliseconds;
            var byRetention = oldest <= 0 ? StreamPosition.Zero : new StreamPosition(oldest, 0);
            return byRetention < lastAppliedId ? byRetention : lastAppliedId;
        }

        private void OnMessageApplied(StreamEntry entry, IMessage message) => OnMessage(message);

        private void OnMessageSent(IMessage message) => OnMessage(message);

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
                _timer.Change(_delay, Timeout.InfiniteTimeSpan);
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
        private void Request()
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
            }

            _ = Task.Run(RunAsync);
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
                    Debug.WriteLine(ex);
                }

                lock (_gate)
                {
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
