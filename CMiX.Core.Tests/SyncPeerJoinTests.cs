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
    // Join of a peer: start as engine or studio, the snapshot, the SnapshotApplied event and a join after a failure. The store is in memory. Peers apply inline, because no dispatcher is set.
    public class SyncPeerJoinTests
    {
        [Fact]
        public async Task Start_OnEmptyStore_PushesOwnStateAndJoins()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store);

            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            var snapshot = await store.ReadSnapshotAsync();
            Assert.NotNull(snapshot);
            Assert.Single(store.Entries);
            Assert.Equal(store.Entries[0].Id, snapshot.StreamId);
            Assert.Equal(snapshot.StreamId, peer.LastAppliedId);
            Assert.True(peer.IsInSync);
            Assert.Equal("Connected", peer.Status);
        }

        [Fact]
        public async Task EngineOnEmptyStore_JoinsAtZero_WithoutPushing()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store, isWriter: false);

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined);

            Assert.Null(await store.ReadSnapshotAsync());
            Assert.Empty(store.Entries);
            Assert.Equal(StreamPosition.Zero, peer.LastAppliedId);
            Assert.True(peer.IsInSync);
        }

        [Fact]
        public async Task StartAndJoin_OfAnEngineOnAStoreWithASnapshot_JoinsFromIt()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var tail = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await store.WriteSnapshotAsync(SnapshotOf(model, tail));

            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined);

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tail, peer.LastAppliedId);
        }

        [Fact]
        public async Task Join_WithSnapshot_AppliesModelAndReplaysOnlyLaterEntries()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var before = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var atSnapshot = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var afterId = Guid.NewGuid();
            var after = await store.AppendAsync(Envelope("other", new MessageOnClick(afterId)));
            var model = ModelWithOneComposition();
            await store.WriteSnapshotAsync(SnapshotOf(model, atSnapshot));

            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("B"));
            await WaitUntilAsync(() => peer.IsJoined);

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Single(target.Applied);
            Assert.Equal(afterId, target.Applied[0].ID);
            Assert.Equal(after, peer.LastAppliedId);
            Assert.True(before < peer.LastAppliedId);
        }

        [Fact]
        public async Task SnapshotApply_OnRealProject_GivesTheSameHashAsTheModel()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var a = CreatePeer(targetA, store);
            a.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => a.IsJoined);

            var providerB = TestServiceProviderFactory.Create();
            var projectB = providerB.GetRequiredService<Project>();
            var messengerB = providerB.GetRequiredService<ControlMessenger>();
            var targetB = new ProjectSyncTarget(projectB, messengerB, providerB.GetRequiredService<UndoManager>());
            using var b = CreatePeer(targetB, store, messengerB, isWriter: false);
            messengerB.Register(b);
            b.StartAndJoin(Options("B"));
            await WaitUntilAsync(() => b.IsJoined);

            var expected = ProjectStateHash.Compute(targetA.Model);
            Assert.Equal(expected, ProjectStateHash.Compute(projectB));

            var captured = targetB.Capture();
            var bytes = VL.Serialization.MessagePack.MessagePackSerialization.Serialize(captured);
            var roundTripped = VL.Serialization.MessagePack.MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(bytes));
            Assert.Equal(expected, ProjectStateHash.Compute(roundTripped));
        }

        [Fact]
        public async Task TwoPeers_ExchangeEditsBothWays_AndLateJoinerGetsFullState()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var targetB = new RecordingSyncTarget();
            using var a = CreatePeer(targetA, store);
            using var b = CreatePeer(targetB, store);
            a.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => a.IsJoined);
            b.StartAndJoin(Options("B"));
            await WaitUntilAsync(() => b.IsJoined);

            var fromA = new MessageOnClick(Guid.NewGuid());
            a.SendMessage(fromA);
            await WaitUntilAsync(() => targetB.Applied.Any(m => m.ID == fromA.ID));

            var fromB = new MessageOnClick(Guid.NewGuid());
            b.SendMessage(fromB);
            await WaitUntilAsync(() => targetA.Applied.Any(m => m.ID == fromB.ID));

            var targetC = new RecordingSyncTarget();
            using var c = CreatePeer(targetC, store);
            c.StartAndJoin(Options("C"));
            await WaitUntilAsync(() => c.IsJoined);

            Assert.Equal(1, targetC.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(targetA.Model), ProjectStateHash.Compute(targetC.Model));
            Assert.Equal(new[] { fromA.ID, fromB.ID }, targetC.Applied.Select(m => m.ID));
            Assert.Equal(a.LastAppliedId, c.LastAppliedId);
        }

        // The Studio empties its repository managers when it gets a snapshot, so every snapshot
        // apply must tell it. There are three: the join, a snapshot entry, and the recovery.
        [Fact]
        public async Task SnapshotApplied_OnAJoinFromASnapshot_ComesOnce()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var tail = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await store.WriteSnapshotAsync(SnapshotOf(model, tail));

            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            var events = 0;
            object sender = null;
            peer.SnapshotApplied += (s, _) =>
            {
                sender = s;
                Interlocked.Increment(ref events);
            };

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined);
            await SettleAsync();

            Assert.Equal(1, Volatile.Read(ref events));
            Assert.Same(peer, sender);
        }

        [Fact]
        public async Task SnapshotApplied_OnASnapshotEntry_ComesOnce()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            var events = 0;
            peer.SnapshotApplied += (_, _) => Interlocked.Increment(ref events);

            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);
            Assert.Equal(0, Volatile.Read(ref events));

            await store.AppendAsync(Envelope("other", new MessageProjectSnapshot(Guid.NewGuid(), ModelWithOneComposition())));
            await WaitUntilAsync(() => target.SnapshotsApplied == 1);
            await SettleAsync();

            Assert.Equal(1, Volatile.Read(ref events));
        }

        [Fact]
        public async Task SnapshotApplied_OnARecoveryAfterAGap_ComesOnce()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            var events = 0;
            peer.SnapshotApplied += (_, _) => Interlocked.Increment(ref events);

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
            await SettleAsync();

            Assert.Equal(1, Volatile.Read(ref events));
        }

        // A replacement host with an earlier clock gives entry IDs below the position the peer
        // holds. Without a re-join the reader waits for entries that never come.
        [Fact]
        public async Task Tick_WithTheTailBelowTheOwnPosition_JoinsTheNewStore()
        {
            var inner = new InMemorySyncStore(1000000);
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);
            Assert.True(peer.LastAppliedId > StreamPosition.Zero);

            var replacement = new InMemorySyncStore();
            await replacement.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var snapshotId = await replacement.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await replacement.WriteSnapshotAsync(SnapshotOf(model, snapshotId));
            var afterId = Guid.NewGuid();
            var tail = await replacement.AppendAsync(Envelope("other", new MessageOnClick(afterId)));
            store.Replace(replacement);

            await WaitUntilAsync(() => peer.LastAppliedId == tail, 15000, () => $"status={peer.Status} last={peer.LastAppliedId}");
            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Equal(afterId, Assert.Single(target.Applied).ID);
            await WaitUntilAsync(() => peer.IsInSync, detail: () => $"tail={peer.TailId} last={peer.LastAppliedId}");
        }

        [Fact]
        public async Task Engine_WhenTheFirstJoinFails_TriesAgainAndJoins()
        {
            var inner = new InMemorySyncStore();
            var store = new UnreliableSyncStore(inner) { Fail = true };
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => store.FailedCalls > 0 && peer.ErrorMessage.Length > 0,
                detail: () => $"status={peer.Status} failed={store.FailedCalls}");
            Assert.False(peer.IsJoined);

            store.Fail = false;

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        [Fact]
        public async Task Engine_AfterAFailedJoin_AppliesTheSnapshotOnce()
        {
            var inner = new InMemorySyncStore();
            await inner.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));

            var store = new UnreliableSyncStore(inner) { Fail = true };
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.ErrorMessage.Length > 0, detail: () => $"status={peer.Status}");

            store.Fail = false;

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            await SettleAsync();

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tail, peer.LastAppliedId);
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        [Fact]
        public async Task Engine_WhenTheStoreGoesAwayDuringTheRetry_JoinsOnTheNextConnection()
        {
            var inner = new InMemorySyncStore();
            var store = new UnreliableSyncStore(inner) { Fail = true };
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);

            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.ErrorMessage.Length > 0, detail: () => $"status={peer.Status}");

            store.SetConnected(false);
            await WaitUntilAsync(() => !peer.IsConnected);

            // Longer than the largest backoff, so the peer waits for the connection event.
            await Task.Delay(300);
            Assert.False(peer.IsJoined);

            store.Fail = false;
            store.SetConnected(true);

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }
    }
}
