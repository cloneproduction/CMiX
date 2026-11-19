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
        // Compares the local state with the store state. It reads only, and changes nothing.
        public async Task<StartCheck> CheckStartAsync()
        {
            var run = Volatile.Read(ref _run);
            if (run == null) return StartCheck.NotInSync;

            var result = await EvaluateStartAsync(run.Store).ConfigureAwait(false);
            return result.Check;
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
    }
}
