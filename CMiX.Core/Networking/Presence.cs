// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;

namespace CMiX.Core.Networking
{
    // What one heartbeat tick found in the store.
    public record PresenceTick(StreamPosition Tail, StreamPosition SnapshotId, IReadOnlyList<PeerInfo> Peers);

    // Writes the own heartbeat and reads the stream tail, the snapshot position and, when asked,
    // the peer list, once per interval.
    public sealed class Presence
    {
        private readonly ISyncStore _store;
        private readonly string _peerId;
        private readonly Func<IReadOnlyDictionary<string, string>> _fields;
        private readonly Func<bool> _listPeers;
        private readonly Func<PresenceTick, Task> _onTick;

        public Presence(ISyncStore store, string peerId, Func<IReadOnlyDictionary<string, string>> fields, Func<bool> listPeers, Func<PresenceTick, Task> onTick)
        {
            _store = store;
            _peerId = peerId;
            _fields = fields;
            _listPeers = listPeers;
            _onTick = onTick;
        }

        public async Task RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _store.HeartbeatAsync(_peerId, _fields(), SyncTimings.HeartbeatTtl).ConfigureAwait(false);
                    var tail = await _store.ReadTailAsync().ConfigureAwait(false);
                    var snapshotId = await _store.ReadSnapshotIdAsync().ConfigureAwait(false);
                    var peers = _listPeers() ? await _store.ListPeersAsync().ConfigureAwait(false) : null;
                    await _onTick(new PresenceTick(tail, snapshotId, peers)).ConfigureAwait(false);
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
                    await Task.Delay(SyncTimings.HeartbeatInterval, ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
