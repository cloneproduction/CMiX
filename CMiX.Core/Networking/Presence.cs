// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;

namespace CMiX.Core.Networking
{
    // What one heartbeat tick found in the store.
    public record PresenceTick(StreamPosition Tail, StreamPosition SnapshotId, StreamPosition? Oldest, IReadOnlyList<PeerInfo> Peers);

    // Writes the own heartbeat and reads the stream tail, the snapshot position and, when asked,
    // the peer list, once per interval.
    public sealed class Presence
    {
        private readonly ISyncStore _store;
        private readonly string _peerId;
        private readonly Func<IReadOnlyDictionary<string, string>> _fields;
        private readonly Func<bool> _listPeers;
        private readonly Func<StreamPosition> _position;
        private readonly Func<PresenceTick, Task> _onTick;
        private readonly SyncTimings _timings;

        public Presence(ISyncStore store, string peerId, Func<IReadOnlyDictionary<string, string>> fields, Func<bool> listPeers,
            Func<StreamPosition> position, Func<PresenceTick, Task> onTick, SyncTimings timings)
        {
            _store = store;
            _peerId = peerId;
            _fields = fields;
            _listPeers = listPeers;
            _position = position;
            _onTick = onTick;
            _timings = timings;
        }

        public async Task RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _store.HeartbeatAsync(_peerId, _fields(), _timings.HeartbeatTtl).ConfigureAwait(false);
                    var tail = await _store.ReadTailAsync().ConfigureAwait(false);
                    var snapshotId = await _store.ReadSnapshotIdAsync().ConfigureAwait(false);

                    // A joined peer is at or above the snapshot, so the oldest entry is read only
                    // when a gap is possible.
                    var oldest = (StreamPosition?)null;
                    if (snapshotId > _position())
                    {
                        var first = await _store.ReadRangeAsync(StreamPosition.Zero, 1).ConfigureAwait(false);
                        if (first.Count > 0)
                            oldest = first[0].Id;
                    }

                    var peers = _listPeers() ? await _store.ListPeersAsync().ConfigureAwait(false) : null;
                    await _onTick(new PresenceTick(tail, snapshotId, oldest, peers)).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                try
                {
                    await Task.Delay(_timings.HeartbeatInterval, ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
