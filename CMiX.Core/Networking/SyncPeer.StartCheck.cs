// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;

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

            // No snapshot and an empty stream gives PushSilently.
            if (snapshot == null)
                return new StartEvaluation(tail == StreamPosition.Zero ? StartCheck.PushSilently : StartCheck.NotInSync, tail);

            // Entries after the snapshot are edits the local state does not have.
            if (tail > snapshot.StreamId)
                return new StartEvaluation(StartCheck.NotInSync, tail);

            var local = await DispatchAsync(() => _target.Capture()).ConfigureAwait(false);
            var stored = DecodeModel(snapshot);
            var same = ProjectStateHash.Compute(local) == ProjectStateHash.Compute(stored);
            return new StartEvaluation(same ? StartCheck.AlreadyInSync : StartCheck.NotInSync, snapshot.StreamId);
        }

        // Runs under the join lock. NotInSync leaves the peer blocked until the user pushes or pulls.
        private Task RunStartCheckAsync(SyncRun run)
            => RunUnderJoinLockAsync(run, () => CheckStartLockedAsync(run));

        private async Task CheckStartLockedAsync(SyncRun run)
        {
            // A check queued after a join does nothing.
            if (IsJoined) return;

            await SetActivityAsync("Checking").ConfigureAwait(false);
            var result = await EvaluateStartAsync(run.Store).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            // The activity clears before the check acts, and again in the finally of the lock.
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
                        MarkJoined(run);
                    }).ConfigureAwait(false);
                    break;
            }
        }
    }
}
