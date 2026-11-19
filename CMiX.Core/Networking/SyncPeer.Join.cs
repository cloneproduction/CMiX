// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using VL.Serialization.MessagePack;

namespace CMiX.Core.Networking
{
    public partial class SyncPeer
    {
        // Pull: adopt the store state, then follow the stream.
        public Task JoinAsync()
        {
            var run = Volatile.Read(ref _run);
            if (run == null) return Task.CompletedTask;

            return JoinAsync(run);
        }

        // Push: publish the local state as the new store state, then follow the stream.
        public Task PushAsync() => PushAsync(Volatile.Read(ref _run));

        private async Task OnConnectedAsync(SyncRun run)
        {
            if (!run.AutoJoin)
            {
                await RunStartCheckAsync(run).ConfigureAwait(false);
                return;
            }

            var store = run.Store;
            var ct = run.Cts.Token;

            // An engine has no user who repeats a failed join, so it tries again with a growing backoff.
            var timings = run.Timings;
            var backoff = timings.MinBackoff;
            while (!ct.IsCancellationRequested && !IsJoined)
            {
                if (!IsCurrent(run)) return;

                if (store.IsConnected)
                {
                    await AutoJoinAsync(run).ConfigureAwait(false);
                    if (!IsCurrent(run)) return;
                    if (IsJoined) return;

                    await Task.Delay(backoff, ct).ConfigureAwait(false);
                }
                else
                {
                    await WaitForConnectionEventAsync(store, backoff, ct).ConfigureAwait(false);
                }

                // Double the backoff up to MaxBackoff. The loop checks the run after every wait.
                backoff = TimeSpan.FromTicks(Math.Min(backoff.Ticks * 2, timings.MaxBackoff.Ticks));
            }
        }

        private Task JoinAsync(SyncRun run)
            => RunUnderJoinLockAsync(run, () => AdoptStoreAsync(run));

        // The engine joins by itself. Under the lock, a peer that is joined already does nothing.
        private Task AutoJoinAsync(SyncRun run)
            => RunUnderJoinLockAsync(run, () => IsJoined ? Task.CompletedTask : AdoptStoreAsync(run));

        // The re-join after a trim. The follower stop and the activity come before the shortcut.
        private Task RejoinAfterGapAsync(SyncRun run)
        {
            return RunUnderJoinLockAsync(run, RejoinLockedAsync);

            async Task RejoinLockedAsync()
            {
                var store = run.Store;
                await StopFollowerAsync(run).ConfigureAwait(false);
                await SetActivityAsync("Joining").ConfigureAwait(false);

                var snapshotId = await store.ReadSnapshotIdAsync().ConfigureAwait(false);
                if (!IsCurrent(run)) return;

                // The follower recovery applied the snapshot already, so only replay from the position.
                if (snapshotId != StreamPosition.Zero && snapshotId <= LastAppliedId)
                {
                    await ReplayAsync(run, LastAppliedId).ConfigureAwait(false);
                    if (!IsCurrent(run)) return;

                    await FinishJoinAsync(run).ConfigureAwait(false);
                    return;
                }

                await ApplyStoreStateAsync(run).ConfigureAwait(false);
            }
        }

        // The caller holds the join lock.
        private async Task AdoptStoreAsync(SyncRun run)
        {
            await StopFollowerAsync(run).ConfigureAwait(false);
            await SetActivityAsync("Joining").ConfigureAwait(false);
            await ApplyStoreStateAsync(run).ConfigureAwait(false);
        }

        // The caller holds the join lock and has stopped the follower.
        private async Task ApplyStoreStateAsync(SyncRun run)
        {
            var store = run.Store;
            var ct = run.Cts.Token;

            // The store state replaces the local one: discard the queue and wait for an append in flight.
            _outgoing.Discard();
            await _outgoing.IdleAsync(ct).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            // Read the snapshot, then the tail.
            var snapshot = await store.ReadSnapshotAsync().ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var tail = await store.ReadTailAsync().ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            // Nothing to adopt: a writer pushes its state, a non-writer follows the stream from the start.
            if (snapshot == null && tail == StreamPosition.Zero)
            {
                if (IsWriter)
                    await PushCoreAsync(run).ConfigureAwait(false);
                else
                    await DispatchCurrentAsync(run, () =>
                    {
                        LastAppliedId = StreamPosition.Zero;
                        TailId = StreamPosition.Zero;
                    }).ConfigureAwait(false);

                if (!IsCurrent(run)) return;
            }
            else
            {
                var position = StreamPosition.Zero;
                if (snapshot != null)
                {
                    var model = DecodeModel(snapshot);
                    position = snapshot.StreamId;
                    await DispatchCurrentAsync(run, () => ApplySnapshotOnDispatcher(model, position)).ConfigureAwait(false);
                }
                else
                {
                    await DispatchCurrentAsync(run, () => LastAppliedId = StreamPosition.Zero).ConfigureAwait(false);
                }

                if (!IsCurrent(run)) return;

                await ReplayAsync(run, position).ConfigureAwait(false);
                if (!IsCurrent(run)) return;
            }

            await FinishJoinAsync(run).ConfigureAwait(false);
        }

        private async Task PushAsync(SyncRun run)
        {
            if (!IsWriter)
            {
                await DispatchAsync(() => ErrorMessage = "This peer does not write.").ConfigureAwait(false);
                return;
            }

            if (run == null) return;

            await RunUnderJoinLockAsync(run, () => PushLockedAsync(run)).ConfigureAwait(false);
        }

        private async Task RunUnderJoinLockAsync(SyncRun run, Func<Task> body)
        {
            // Take the lock, check the run, then run the body.
            if (!await WaitJoinLockAsync(run.Cts.Token).ConfigureAwait(false)) return;
            try
            {
                if (!IsCurrent(run)) return;

                await body().ConfigureAwait(false);
            }
            // A cancel ends in silence. Any other error goes to ErrorMessage.
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DispatchCurrentAsync(run, () => ErrorMessage = ex.Message).ConfigureAwait(false);
            }
            finally
            {
                // The activity clears first, then the lock releases.
                await SetActivityAsync(null).ConfigureAwait(false);
                _joinLock.Release();
            }
        }

        // The caller holds the join lock.
        private async Task PushLockedAsync(SyncRun run)
        {
            await StopFollowerAsync(run).ConfigureAwait(false);
            await SetActivityAsync("Pushing").ConfigureAwait(false);
            await PushCoreAsync(run).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            await FinishJoinAsync(run).ConfigureAwait(false);
        }

        // Ends a join or a push: ErrorMessage clears, IsJoined is true, then the follower starts.
        private Task FinishJoinAsync(SyncRun run)
            => DispatchCurrentAsync(run, () =>
            {
                ErrorMessage = string.Empty;
                IsJoined = true;
                StartFollower(run);
            });

        // Push order: discard and idle the queue, the full model as one entry, the snapshot, the trim.
        private async Task PushCoreAsync(SyncRun run)
        {
            var store = run.Store;

            // The model holds every queued edit. An append in flight must land before the snapshot entry.
            _outgoing.Discard();
            await _outgoing.IdleAsync(run.Cts.Token).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var model = await DispatchAsync(() => _target.Capture()).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var envelope = new MessageEnvelope
            {
                SenderID = _peerId,
                MessageID = Guid.NewGuid(),
                Payload = new MessageProjectSnapshot(Guid.NewGuid(), model)
            };

            var id = await store.AppendAsync(MessagePackSerialization.Serialize(envelope)).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            var snapshot = new Snapshot(MessagePackSerialization.Serialize(model), id, _peerId, DateTime.UtcNow);
            await store.WriteSnapshotAsync(snapshot).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            // The trim comes after the snapshot, so a late engine finds it.
            await store.TrimAsync(id).ConfigureAwait(false);
            if (!IsCurrent(run)) return;

            // The four state values change in one dispatched write.
            await DispatchCurrentAsync(run, () =>
            {
                LastSentId = id;
                LastAppliedId = id;
                TailId = id;
                SentMessages++;
            }).ConfigureAwait(false);
        }

        // Returns false when the peer stopped while the caller waited for the lock.
        private async Task<bool> WaitJoinLockAsync(CancellationToken ct)
        {
            try
            {
                await _joinLock.WaitAsync(ct).ConfigureAwait(false);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }
}
