// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics;
using CMiX.Core.Compositing;

namespace CMiX.Core.Networking
{
    public partial class SyncPeer
    {
        // The stream was trimmed past the own position: the oldest entry is newer and the snapshot is ahead.
        private bool HasGap(StreamPosition snapshotId, StreamPosition? oldest)
            => snapshotId > LastAppliedId && oldest.HasValue && oldest.Value > LastAppliedId;

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

            // The source is not disposed: Stop can still hold it, and a cancelled source costs nearly nothing.
            Cancel(cts);
            await WaitForAsync(task).ConfigureAwait(false);
        }

        // Recovery: apply the snapshot again only when the stream was trimmed past the own position.
        private async Task OnFollowerErrorAsync(SyncRun run)
        {
            var store = run.Store;
            // The flag covers the whole recovery.
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

                var model = DecodeModel(snapshot);
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
    }
}
