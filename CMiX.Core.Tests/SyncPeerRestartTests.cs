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
    // A restart or a stop while a join, a push, an append, a recovery or a presence tick still runs. The store is in memory. Peers apply inline, because no dispatcher is set.
    public class SyncPeerRestartTests
    {
        // A presence tick reads the tail, and the own appends of a drag move the position on before the tick is applied.
        // The old tail is then below the position, but the store is the same. The peer must not join again: a join
        // drops the own edits that wait to be sent and applies the snapshot, so the value goes back.
        [Fact]
        public async Task PresenceTick_OwnAppendsPassTheReadTail_DoesNotJoinAgain()
        {
            var inner = new InMemorySyncStore(1000000);
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);
            var snapshotsBefore = target.SnapshotsApplied;

            var gate = new StoreGate();
            store.AfterReadTail = gate.Hold;
            await WaitUntilAsync(() => gate.IsReached, 15000, () => $"status={peer.Status}");
            store.AfterReadTail = null;

            var heldTail = await inner.ReadTailAsync();
            for (var i = 0; i < 5; i++)
                peer.SendMessage(new MessageOnClick(Guid.NewGuid()));
            await WaitUntilAsync(() => peer.LastAppliedId > heldTail, 5000, () => $"last={peer.LastAppliedId} held={heldTail}");

            await gate.ReleaseAndSettleAsync();

            Assert.Equal(snapshotsBefore, target.SnapshotsApplied);
            Assert.True(peer.IsJoined);
        }

        // The Connect button of the Studio stops and starts at once. A join that still runs on the
        // store of the earlier start must change nothing.
        [Fact]
        public async Task RestartDuringAJoin_JoinsOnTheNewStore_AndLeavesTheOldOneAlone()
        {
            var innerA = new InMemorySyncStore();
            await innerA.ConnectAsync(default);
            var modelA = ModelWithOneComposition();
            var tailA = await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await innerA.WriteSnapshotAsync(SnapshotOf(modelA, tailA));
            var storeA = new WrappingSyncStore(innerA);

            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            var modelB = ModelWithOneComposition();
            var tailB = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await storeB.WriteSnapshotAsync(SnapshotOf(modelB, tailB));
            Assert.NotEqual(ProjectStateHash.Compute(modelA), ProjectStateHash.Compute(modelB));

            var gate = new StoreGate();
            var reads = 0;
            storeA.BeforeReadSnapshot = () =>
            {
                Interlocked.Increment(ref reads);
                return gate.Hold();
            };

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = false, Timings = Fast };

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => gate.IsReached, detail: () => $"status={peer.Status}");

            peer.Stop();
            peer.StartAndJoin(Options("Engine"));

            // Stop disposed the old store. Connect it again, so the read that waits can return a
            // snapshot and the test sees what the peer does with it.
            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 3000);
            await innerA.ConnectAsync(default);
            gate.Release();

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            await SettleAsync();

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(modelB), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tailB, peer.LastAppliedId);
            Assert.Equal(1, Volatile.Read(ref reads));
            Assert.Equal(0, storeA.WriteSnapshotCalls);
            Assert.Empty(storeA.TrimCalls);
            Assert.Single(innerA.Entries);
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        [Fact]
        public async Task Start_TwiceBeforeTheFirstJoinLands_KeepsTheStateOfTheSecondRun()
        {
            var innerA = new InMemorySyncStore();
            await innerA.ConnectAsync(default);
            var tailA = await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await innerA.WriteSnapshotAsync(SnapshotOf(ModelWithOneComposition(), tailA));
            var storeA = new WrappingSyncStore(innerA);

            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            var snapshotB = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await storeB.WriteSnapshotAsync(SnapshotOf(ModelWithOneComposition(), snapshotB));
            var tailB = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));

            var gate = new StoreGate();
            storeA.BeforeReadSnapshot = gate.Hold;

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = false, Timings = Fast };

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => gate.IsReached, detail: () => $"status={peer.Status}");

            peer.Start(Options("Studio"));
            await WaitUntilAsync(() => peer.Status == "Not in sync" && peer.TailId == tailB, 15000,
                () => $"status={peer.Status} tail={peer.TailId} error={peer.ErrorMessage}");

            await WaitUntilAsync(() => !innerA.IsConnected, 3000);
            await innerA.ConnectAsync(default);
            await gate.ReleaseAndSettleAsync();

            Assert.Equal(0, target.SnapshotsApplied);
            Assert.False(peer.IsJoined);
            Assert.Equal(StreamPosition.Zero, peer.LastAppliedId);
            Assert.Equal(tailB, peer.TailId);
            Assert.Equal("Not in sync", peer.Status);
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        // The same for the push of the Studio. The entry that is on its way stays in the old store,
        // but the snapshot and the trim go to the new one only.
        [Fact]
        public async Task RestartDuringAPush_PushesToTheNewStore_AndLeavesTheOldOneAlone()
        {
            var innerA = new InMemorySyncStore();
            var storeA = new WrappingSyncStore(innerA);
            var storeB = new InMemorySyncStore();

            var gate = new StoreGate();
            storeA.BeforeAppend = gate.Hold;

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = true, Timings = Fast };

            // Both stores are empty, so the start check pushes the local state.
            peer.Start(Options("Studio"));
            await WaitUntilAsync(() => gate.IsReached, detail: () => $"status={peer.Status}");

            peer.Stop();
            peer.Start(Options("Studio"));

            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 3000);
            await innerA.ConnectAsync(default);
            gate.Release();

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            await SettleAsync();

            var snapshot = await storeB.ReadSnapshotAsync();
            Assert.NotNull(snapshot);
            var stored = ModelOf(snapshot);
            Assert.Equal(ProjectStateHash.Compute(target.Model), ProjectStateHash.Compute(stored));
            Assert.Single(storeB.Entries);
            Assert.Equal(1, peer.SentMessages);

            Assert.Equal(0, storeA.WriteSnapshotCalls);
            Assert.Empty(storeA.TrimCalls);
            Assert.Null(await innerA.ReadSnapshotAsync());
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        // An append that already runs is not cancellable. The loop of the old run must not confirm
        // its entry, and must not take an entry out of the queue that only the new loop can append.
        [Fact]
        public async Task Restart_DuringAnAppend_SendsBothMessagesToTheNewStore()
        {
            var innerA = new InMemorySyncStore();
            var storeA = new WrappingSyncStore(innerA);
            var storeB = new InMemorySyncStore();

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = true, Timings = Fast };

            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            var gate = new StoreGate();
            storeA.BeforeAppend = gate.Hold;

            var first = Guid.NewGuid();
            var second = Guid.NewGuid();
            peer.SendMessage(new MessageOnClick(first));
            peer.SendMessage(new MessageOnClick(second));
            await WaitUntilAsync(() => gate.IsReached, detail: () => $"status={peer.Status}");

            peer.Stop();
            peer.StartAndJoin(Options("Studio"));

            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            // The push of the second start waits for that append, so the release comes first.
            await WaitUntilAsync(() => !innerA.IsConnected, 3000);
            await innerA.ConnectAsync(default);
            gate.Release();
            await WaitUntilAsync(() => peer.IsJoined, 15000, () => $"status={peer.Status} error={peer.ErrorMessage}");
            var sent = peer.SentMessages;

            // The push of the second start holds both edits in its model, so the queue is dropped
            // and nothing else lands on the new store.
            await WaitUntilAsync(() => peer.PendingMessages == 0, 15000,
                () => $"pending={peer.PendingMessages} entries={storeB.Entries.Count}");
            await SettleAsync();

            Assert.Single(storeB.Entries);
            Assert.Equal(storeB.Entries[0].Id, peer.LastSentId);
            Assert.Equal(sent, peer.SentMessages);

            // The push of the first start, and the entry that was in flight.
            Assert.InRange(innerA.Entries.Count, 1, 2);
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        [Fact]
        public async Task Pull_AfterARestart_DropsTheQueuedEdits()
        {
            var innerA = new InMemorySyncStore();
            var storeA = new WrappingSyncStore(innerA);
            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            var foreign = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await storeB.WriteSnapshotAsync(SnapshotOf(model, foreign));

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = true, Timings = Fast };

            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            var gate = new StoreGate();
            storeA.BeforeAppend = gate.Hold;

            var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            foreach (var id in ids)
                peer.SendMessage(new MessageOnClick(id));
            await WaitUntilAsync(() => gate.IsReached, detail: () => $"status={peer.Status}");

            peer.Stop();
            peer.Start(Options("Studio"));
            await WaitUntilAsync(() => peer.Status == "Not in sync", 15000, () => $"status={peer.Status} error={peer.ErrorMessage}");

            // Not joined, so the queue stays closed and the new store gets nothing.
            await SettleAsync();
            Assert.Single(storeB.Entries);

            await WaitUntilAsync(() => !innerA.IsConnected, 3000);
            await innerA.ConnectAsync(default);
            gate.Release();

            await peer.JoinAsync();
            await WaitUntilAsync(() => peer.IsJoined && peer.PendingMessages == 0, 15000,
                () => $"status={peer.Status} pending={peer.PendingMessages}");
            await SettleAsync();

            Assert.Single(storeB.Entries);
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Equal(foreign, peer.LastAppliedId);
        }

        // The follower recovery reads the snapshot of the store it follows. A restart while it reads
        // must not put that snapshot into the peer.
        [Fact]
        public async Task Restart_DuringAFollowerRecovery_KeepsTheStateOfTheNewStore()
        {
            // The IDs of the old store are above the IDs of the new one. An unguarded recovery would
            // therefore find its snapshot newer than the position of the new store.
            var innerA = new InMemorySyncStore(1000000);
            var storeA = new WrappingSyncStore(innerA);

            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            var modelB = ModelWithOneComposition();
            var tailB = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await storeB.WriteSnapshotAsync(SnapshotOf(modelB, tailB));

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { Timings = Fast };

            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            // The stream of the old store is trimmed past the own position, so the recovery reads
            // the snapshot.
            storeA.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && storeA.FailedReads > 0);
            await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tailA = await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var modelA = ModelWithOneComposition();
            await innerA.WriteSnapshotAsync(SnapshotOf(modelA, tailA));
            await innerA.TrimAsync(tailA);

            var gate = new StoreGate();
            storeA.BeforeReadSnapshot = gate.Hold;
            storeA.SetFail(false);
            await WaitUntilAsync(() => gate.IsReached, 15000,
                () => $"status={peer.Status} last={peer.LastAppliedId} tailA={tailA} failed=[{storeA.FailedSummary}] snapshotsApplied={target.SnapshotsApplied}");

            peer.Stop();
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");

            // Stop disposed the old store. Connect it again, so the read that waits can return the
            // snapshot and the test sees what the peer does with it.
            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 3000);
            await innerA.ConnectAsync(default);
            await gate.ReleaseAndSettleAsync();

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(modelB), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tailB, peer.LastAppliedId);
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        // The follower of the old store can hold an entry it read before the restart. That entry
        // belongs to another stream, so it must not reach the target or the position.
        [Fact]
        public async Task Restart_WhileAnEntryWaits_LeavesTheEntryOfTheOldStoreUnapplied()
        {
            var innerA = new InMemorySyncStore();
            var storeA = new WrappingSyncStore(innerA);

            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            var modelB = ModelWithOneComposition();
            var tailB = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await storeB.WriteSnapshotAsync(SnapshotOf(modelB, tailB));

            var gate = new StoreGate();
            storeA.AfterReadBlocking = gate.Hold;

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = false, Timings = Fast };

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            var click = Guid.NewGuid();
            await innerA.AppendAsync(Envelope("other", new MessageOnClick(click)));
            await WaitUntilAsync(() => gate.IsReached, 15000, () => $"status={peer.Status}");

            peer.Stop();
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");

            // Stop disposed the old store. Connect it again, so the read that waits can return the
            // entry and the test sees what the peer does with it.
            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 3000);
            await innerA.ConnectAsync(default);
            await gate.ReleaseAndSettleAsync();

            Assert.DoesNotContain(target.Applied, message => message is MessageOnClick clicked && clicked.ID == click);
            Assert.Equal(tailB, peer.LastAppliedId);
            Assert.Equal(ProjectStateHash.Compute(modelB), ProjectStateHash.Compute(target.Model));
        }

        // A presence tick of the store that the restart left behind must not move the tail or the
        // peer list.
        [Fact]
        public async Task Restart_DuringAPresenceTick_KeepsTheTailAndThePeersOfTheNewStore()
        {
            var innerA = new InMemorySyncStore(1000000);
            var storeA = new WrappingSyncStore(innerA);
            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            await storeB.HeartbeatAsync("fresh", new Dictionary<string, string> { ["name"] = "New" }, TimeSpan.FromMinutes(1));

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { ListPeersEnabled = true, Timings = Fast };

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            // The join read the tail already. From here only a presence tick reads it.
            var gate = new StoreGate();
            storeA.BeforeReadTail = gate.Hold;
            await WaitUntilAsync(() => gate.IsReached, 15000, () => $"status={peer.Status}");

            peer.Stop();
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            await WaitUntilAsync(() => PeerIds(peer).Contains("fresh"), 15000, () => $"status={peer.Status}");

            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 3000);
            await innerA.ConnectAsync(default);
            await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await innerA.HeartbeatAsync("ghost", new Dictionary<string, string> { ["name"] = "Old" }, TimeSpan.FromMinutes(1));
            await gate.ReleaseAndSettleAsync();

            Assert.Equal(StreamPosition.Zero, peer.TailId);
            Assert.DoesNotContain("ghost", PeerIds(peer));
            Assert.Contains("fresh", PeerIds(peer));
        }

        [Fact]
        public async Task PresenceTick_OfAStoppedRun_StartsNoJoinOnTheNewRun()
        {
            using var dispatcher = new SingleThreadDispatcher();
            var innerA = new InMemorySyncStore();
            var storeA = new WrappingSyncStore(innerA);

            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            var snapshotB = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await storeB.WriteSnapshotAsync(SnapshotOf(ModelWithOneComposition(), snapshotB));
            await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = false, Timings = Fast };
            peer.SetDispatcher(dispatcher.Post);

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            var followerGate = new StoreGate();
            storeA.BeforeReadBlocking = followerGate.Hold;
            await WaitUntilAsync(() => followerGate.IsReached, 15000, () => $"status={peer.Status}");

            var tickGate = new StoreGate();
            storeA.BeforeReadTail = tickGate.Hold;
            await WaitUntilAsync(() => tickGate.IsReached, 15000, () => $"status={peer.Status}");
            storeA.BeforeReadTail = null;

            var tailA = await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await innerA.WriteSnapshotAsync(SnapshotOf(ModelWithOneComposition(), tailA));
            await innerA.TrimAsync(tailA);

            var restarted = 0;
            peer.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName != nameof(SyncPeer.TailId) || peer.TailId != tailA) return;
                if (Interlocked.Exchange(ref restarted, 1) == 1) return;

                peer.Stop();
                peer.Start(Options("Studio"));
            };
            tickGate.Release();

            await WaitUntilAsync(() => Volatile.Read(ref restarted) == 1, 15000, () => $"status={peer.Status} tail={peer.TailId}");
            await WaitUntilAsync(() => peer.Status == "Not in sync" || peer.IsJoined, 15000,
                () => $"status={peer.Status} error={peer.ErrorMessage}");
            await SettleAsync();
            followerGate.Release();

            Assert.False(peer.IsJoined);
            Assert.Equal(0, target.SnapshotsApplied);
            Assert.Equal("Not in sync", peer.Status);
        }

        // The presence tick replaces Peers on its own thread. Copy it again when the copy fails.
        private static string[] PeerIds(SyncPeer peer) => PeersOf(peer).Select(info => info.PeerId).ToArray();

        // Stop cancels the follower source that a join can stop at the same time. A disposed source
        // would throw on the thread of the user.
        [Fact]
        public async Task JoinAndStop_AtTheSameTime_ThrowNothing()
        {
            for (var i = 0; i < 20; i++)
            {
                var store = new InMemorySyncStore();
                var target = new RecordingSyncTarget();
                using var peer = CreatePeer(target, store);
                peer.StartAndJoin(Options("A"));
                await WaitUntilAsync(() => peer.IsJoined, detail: () => $"iteration {i}");

                var join = Task.Run(() => peer.JoinAsync());
                Assert.Null(Record.Exception(() => peer.Stop()));
                Assert.Null(await Record.ExceptionAsync(() => join));
            }
        }

        // A run that subscribes after its own Stop leaves a handler on the store it left. An event
        // of that store must not change the state the current store gave.
        [Fact]
        public async Task AfterARestart_AnEventOfTheOldStore_ChangesNothing()
        {
            var storeA = new LingeringSyncStore(new InMemorySyncStore());
            var storeB = new InMemorySyncStore();
            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { Timings = Fast };

            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            peer.Stop();
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");

            storeA.SimulateDisconnect("old reason");

            Assert.True(peer.IsConnected);
            Assert.Equal(string.Empty, peer.ErrorMessage);

            storeB.SimulateDisconnect("new reason");

            Assert.False(peer.IsConnected);
            Assert.Equal("new reason", peer.ErrorMessage);
        }
    }
}
