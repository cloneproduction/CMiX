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
                // The recovery sees the same gap, so the check skips while it runs.
                gap = IsJoined && !Volatile.Read(ref _recovering) && (behindTail || HasGap(tick.SnapshotId, tick.Oldest));

                // The store lost the snapshot, for example after a restart. The writer makes a new one.
                if (IsJoined && _compactor != null && tick.SnapshotId == StreamPosition.Zero)
                    _compactor.Request();
            }).ConfigureAwait(false);

            // After the tick: a gap starts the re-join, the full join when the peer is behind the tail.
            if (!gap) return;

            if (behindTail)
                await JoinAsync(run).ConfigureAwait(false);
            else
                await RejoinAfterGapAsync(run).ConfigureAwait(false);
        }

        // The tail is below the lower of the own position and the position at the read: a replacement store.
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
