// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // Recovery of a follower after a gap in the stream, and the heartbeat that sees the same gap. The store is in memory. Peers apply inline, because no dispatcher is set.
    public class SyncPeerRecoveryTests
    {
        [Fact]
        public async Task Recovery_AfterStreamTrimmedPastOwnPosition_ReappliesSnapshot()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            store.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && store.FailedReads > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));
            await inner.TrimAsync(tail);
            store.SetFail(false);

            await WaitUntilAsync(() => target.SnapshotsApplied == 1, 15000, () => $"status={peer.Status} last={peer.LastAppliedId}");
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tail, peer.LastAppliedId);
            Assert.Empty(target.Applied);
            await WaitUntilAsync(() => peer.IsConnected);
        }

        // A snapshot apply on a real project clears the compositions through the manager, which
        // sends one remove message per composition. B is joined and writes, so an unblocked apply
        // would put those messages into the stream and delete the compositions of every peer.
        [Fact]
        public async Task Recovery_OnRealProject_AppliesSnapshotWithoutWritingToTheStream()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var inner = new InMemorySyncStore();
            var storeB = new WrappingSyncStore(inner);
            var targetB = new ProjectSyncTarget(project, messenger, provider.GetRequiredService<UndoManager>());
            using var b = CreatePeer(targetB, storeB, messenger);
            messenger.Register(b);

            // The peer blocks sending until it joins, so this composition stays local.
            project.CompositionManager.AddItem(typeof(Composition));
            b.Start(Options("B"));
            await WaitUntilAsync(() => b.IsJoined);

            var targetA = new RecordingSyncTarget();
            using var a = CreatePeer(targetA, inner, isWriter: false);
            a.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => a.IsJoined);

            storeB.SetFail(true);
            await WaitUntilAsync(() => !b.IsConnected && storeB.FailedReads > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));
            await inner.TrimAsync(tail);
            storeB.SetFail(false);

            await WaitUntilAsync(() => b.LastAppliedId == tail, 15000, () => $"status={b.Status} last={b.LastAppliedId}");
            var composition = Assert.Single(project.CompositionManager.ManagerData.Items);
            Assert.Equal(model.CompositionManager.ManagerData.Items[0].ID, composition.ID);

            await SettleAsync();
            Assert.DoesNotContain(targetA.Applied, m => m is MessageRemoveItem || m is MessageAddItem);
            // A is not a writer, so an entry after the snapshot position could only come from B.
            Assert.DoesNotContain(inner.Entries, entry => entry.Id > tail);
        }

        [Fact]
        public async Task Recovery_WhenTheStreamStillHoldsTheOwnPosition_ReplaysWithoutTheSnapshot()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            store.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && store.FailedReads > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));
            store.SetFail(false);

            await WaitUntilAsync(() => target.Applied.Count == 2, 15000, () => $"status={peer.Status} last={peer.LastAppliedId}");
            Assert.Equal(0, target.SnapshotsApplied);
            Assert.Equal(tail, peer.LastAppliedId);
        }

        // The heartbeat sees the gap of an outage that the follower recovery already handles. Only
        // one of the two may rebuild the state.
        [Fact]
        public async Task Recovery_WhileTheHeartbeatSeesTheSameGap_AppliesTheSnapshotOnce()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            store.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && store.FailedReads > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));
            await inner.TrimAsync(tail);

            // Holds the recovery open over two heartbeat intervals.
            var gate = new StoreGate();
            store.BeforeReadSnapshot = gate.Hold;
            store.SetFail(false);
            await WaitUntilAsync(() => gate.IsReached, 15000, () => $"status={peer.Status}");

            await Task.Delay(300);
            gate.Release();
            await WaitUntilAsync(() => peer.LastAppliedId == tail, 5000, () => $"status={peer.Status} last={peer.LastAppliedId}");

            // Gives a second apply the time to appear.
            await SettleAsync();

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(tail, peer.LastAppliedId);
        }

        // The other order: the heartbeat starts the re-join before the recovery sets its flag. The
        // join then waits for the follower, which applies the snapshot. Only one rebuild may run.
        [Fact]
        public async Task TickJoin_DuringAFollowerRecovery_AppliesTheSnapshotOnce()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined);

            store.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && store.FailedReads > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));
            await inner.TrimAsync(tail);

            // Holds the recovery in its snapshot read, so a heartbeat tick can see the same gap.
            var gate = new StoreGate();
            store.BeforeReadSnapshot = gate.Hold;
            store.SetFail(false);
            await WaitUntilAsync(() => gate.IsReached, 15000,
                () => $"status={peer.Status} last={peer.LastAppliedId} tail={tail} failed=[{store.FailedSummary}] snapshotsApplied={target.SnapshotsApplied}");

            await Task.Delay(300);
            gate.Release();
            await WaitUntilAsync(() => peer.LastAppliedId == tail, 5000, () => $"status={peer.Status} last={peer.LastAppliedId}");

            // Gives a second apply the time to appear.
            await SettleAsync();

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tail, peer.LastAppliedId);
        }

        // The re-join of the heartbeat waits for the follower in StopFollowerAsync. The follower
        // moves the peer to the snapshot position while it waits, so the join has nothing to adopt.
        [Fact]
        public async Task TickJoin_WhenTheFollowerReachedTheSnapshot_DoesNotApplyItAgain()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined);

            // Holds the entry that the follower found. The peer stays behind the snapshot, so the
            // heartbeat sees a gap and starts the re-join.
            var gate = new StoreGate();
            store.AfterReadBlocking = gate.Hold;

            var model = ModelWithOneComposition();
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));

            // The tick reads the oldest entry only for a gap check. The join starts right after it.
            await WaitUntilAsync(() => store.OldestReadCalls > 0, 15000, () => $"status={peer.Status}");
            await Task.Delay(300);
            gate.Release();

            await WaitUntilAsync(() => peer.LastAppliedId == tail, 15000, () => $"status={peer.Status} last={peer.LastAppliedId}");
            await SettleAsync();

            Assert.Equal(0, target.SnapshotsApplied);
            Assert.Single(target.Applied);
            Assert.True(peer.IsJoined);
        }

        // A follower can pause without a store error: the process is suspended, or the machine
        // sleeps with a socket that survives. Its read then returns the entries after the trim
        // point. An apply of those entries would move the peer past the gap and hide it.
        [Fact]
        public async Task AfterALongPauseOfTheFollower_ThePeerChecksTheGap()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.FollowerStalePause = TimeSpan.FromMilliseconds(300);

            var gate = new StoreGate();
            store.BeforeReadBlocking = gate.Hold;

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");
            await WaitUntilAsync(() => gate.IsReached, 15000, () => $"status={peer.Status}");

            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));
            await inner.TrimAsync(tail);

            // Longer than the stale pause of the follower.
            await Task.Delay(500);
            gate.Release();

            await WaitUntilAsync(() => target.SnapshotsApplied == 1, 15000, () => $"status={peer.Status} last={peer.LastAppliedId}");
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tail, peer.LastAppliedId);
            Assert.Empty(target.Applied);
        }

        // The oldest entry tells a gap from a lag. A joined peer is at or above the snapshot, so an
        // idle tick must not read it. Every read of it costs one payload on every engine.
        [Fact]
        public async Task AnIdleTick_ReadsTheOldestEntryOnlyWhenTheSnapshotIsAhead()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            // A few heartbeat intervals. An idle tick has the time to read the oldest entry.
            await Task.Delay(500);
            Assert.Equal(0, store.OldestReadCalls);

            // The follower waits here, so only a tick can read the oldest entry of the gap.
            var gate = new StoreGate();
            store.BeforeReadBlocking = gate.Hold;
            await WaitUntilAsync(() => gate.IsReached, 15000, () => $"status={peer.Status}");

            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));
            await inner.TrimAsync(tail);

            await WaitUntilAsync(() => store.OldestReadCalls > 0, 15000, () => $"status={peer.Status}");
            gate.Release();

            await WaitUntilAsync(() => target.SnapshotsApplied == 1, 15000, () => $"status={peer.Status} last={peer.LastAppliedId}");
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tail, peer.LastAppliedId);
        }

        // An outage grows the recovery backoff. Without the wake the peer reads nothing for the rest
        // of that wait, although the store is back.
        [Fact]
        public async Task Reconnect_DuringAFollowerRecovery_ReadsAtOnce()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            // A backoff that is longer than the wait of the assertion. With the short one the
            // follower would read again by itself, and the test would say nothing about the wake.
            peer.Timings = Fast with { MinBackoff = TimeSpan.FromSeconds(2), MaxBackoff = TimeSpan.FromSeconds(2) };
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            store.SetFail(true);
            await WaitUntilAsync(() => store.FailedCalls >= 3, 5000, () => $"failed={store.FailedCalls}");

            // The heartbeat fails too, so the count alone does not say how long the follower waits.
            // After this delay the follower sleeps in its backoff.
            await Task.Delay(300);

            store.SetFail(false);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));

            await WaitUntilAsync(() => target.Applied.Count > 0, 500,
                () => $"applied={target.Applied.Count} status={peer.Status} lastApplied={peer.LastAppliedId}");
        }
    }
}
