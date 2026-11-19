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
        // The stream was trimmed past the own position when its oldest entry is newer than that
        // position and the snapshot is ahead. Then the entries in between are gone, and only the
        // snapshot has the state. An empty stream has no gap.
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

            // The source is not disposed. Stop can still hold a reference to it, and a cancelled
            // linked source without a timer costs nearly nothing.
            Cancel(cts);
            if (task != null)
            {
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        // After a store error or a long pause: show the state, and re-apply the snapshot only when
        // the stream was trimmed past the own position while the peer was away. Otherwise the
        // reader replays.
        private async Task OnFollowerErrorAsync(SyncRun run)
        {
            var store = run.Store;
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

                var model = MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));
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
