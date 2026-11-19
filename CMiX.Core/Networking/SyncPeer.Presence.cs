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
    public partial class SyncPeer
    {
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
    }
}
