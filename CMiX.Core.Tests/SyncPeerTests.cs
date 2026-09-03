using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    // Drives the protocol on an in-memory store. Peers apply inline, because no dispatcher is set.
    public class SyncPeerTests
    {
        private static ProjectModel ModelWithOneComposition()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return (ProjectModel)project.ToModel();
        }

        [Fact]
        public async Task Start_OnEmptyStore_PushesOwnStateAndJoins()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store);

            peer.Start(Options("A"), autoJoin: true);
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

            peer.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            Assert.Null(await store.ReadSnapshotAsync());
            Assert.Empty(store.Entries);
            Assert.Equal(StreamPosition.Zero, peer.LastAppliedId);
            Assert.True(peer.IsInSync);
        }

        [Fact]
        public async Task Engine_SendMessage_AppendsNothing()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            peer.SendMessage(new MessageOnClick(Guid.NewGuid()));

            Assert.Equal(0, peer.PendingMessages);
            Assert.Empty(store.Entries);
        }

        [Fact]
        public async Task Engine_PushAsync_SetsErrorAndAppendsNothing()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            await peer.PushAsync();

            Assert.Equal("This peer does not write.", peer.ErrorMessage);
            Assert.Empty(store.Entries);
        }

        [Fact]
        public async Task EngineOnStoreWithSnapshot_JoinsFromItAsBefore()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var tail = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await store.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), tail, "other", DateTime.UtcNow));

            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.Start(Options("Engine"), autoJoin: true);
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
            await store.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), atSnapshot, "other", DateTime.UtcNow));

            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.Start(Options("B"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(model), ProjectStateHash.Compute(target.Model));
            Assert.Single(target.Applied);
            Assert.Equal(afterId, target.Applied[0].ID);
            Assert.Equal(after, peer.LastAppliedId);
            Assert.True(before < peer.LastAppliedId);
        }

        [Fact]
        public async Task OwnEntries_AdvancePositionWithoutApply()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);
            var pushId = peer.LastAppliedId;

            peer.SendMessage(new MessageOnClick(Guid.NewGuid()));
            await WaitUntilAsync(() => peer.LastAppliedId > pushId);

            Assert.Empty(target.Applied);
            Assert.Equal(0, peer.AppliedMessages);
            Assert.Equal(2, peer.SentMessages);
            Assert.True(peer.IsInSync);
        }

        [Fact]
        public async Task ValueChange_FromAnotherPeer_ReachesControlInRealProject()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var store = new InMemorySyncStore();
            var target = new ProjectSyncTarget(project, messenger, provider.GetRequiredService<UndoManager>());
            using var peer = CreatePeer(target, store, messenger);
            messenger.Register(peer);
            peer.Start(Options("B"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            project.CompositionManager.AddItem(typeof(Composition));
            var opacity = ((Composition)project.CompositionManager.ManagerData.Items[0]).LayerSettings.Opacity;
            await WaitUntilAsync(() => peer.SentMessages >= 2);

            var change = new MessageValueChanged(opacity.ID, new GenericValueModel<float> { ID = opacity.ID, Value = 0.25f });
            await store.AppendAsync(Envelope("other", change));
            await WaitUntilAsync(() => Math.Abs(opacity.Value - 0.25f) < 0.0001f);

            Assert.Equal(1, peer.AppliedMessages);
        }

        [Fact]
        public async Task SnapshotApply_OnRealProject_GivesTheSameHashAsTheModel()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var a = CreatePeer(targetA, store);
            a.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => a.IsJoined);

            var providerB = TestServiceProviderFactory.Create();
            var projectB = providerB.GetRequiredService<Project>();
            var messengerB = providerB.GetRequiredService<ControlMessenger>();
            var targetB = new ProjectSyncTarget(projectB, messengerB, providerB.GetRequiredService<UndoManager>());
            using var b = CreatePeer(targetB, store, messengerB, isWriter: false);
            messengerB.Register(b);
            b.Start(Options("B"), autoJoin: true);
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
            a.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => a.IsJoined);
            b.Start(Options("B"), autoJoin: true);
            await WaitUntilAsync(() => b.IsJoined);

            var fromA = new MessageOnClick(Guid.NewGuid());
            a.SendMessage(fromA);
            await WaitUntilAsync(() => targetB.Applied.Any(m => m.ID == fromA.ID));

            var fromB = new MessageOnClick(Guid.NewGuid());
            b.SendMessage(fromB);
            await WaitUntilAsync(() => targetA.Applied.Any(m => m.ID == fromB.ID));

            var targetC = new RecordingSyncTarget();
            using var c = CreatePeer(targetC, store);
            c.Start(Options("C"), autoJoin: true);
            await WaitUntilAsync(() => c.IsJoined);

            Assert.Equal(1, targetC.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(targetA.Model), ProjectStateHash.Compute(targetC.Model));
            Assert.Equal(new[] { fromA.ID, fromB.ID }, targetC.Applied.Select(m => m.ID));
            Assert.Equal(a.LastAppliedId, c.LastAppliedId);
        }

        [Fact]
        public async Task Push_WritesSnapshotAppendsEntryTrims_AndRunningPeerApplies()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget();
            var targetB = new RecordingSyncTarget();
            using var a = CreatePeer(targetA, store);
            using var b = CreatePeer(targetB, store);
            a.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => a.IsJoined);
            b.Start(Options("B"), autoJoin: true);
            await WaitUntilAsync(() => b.IsJoined);

            for (var i = 0; i < 3; i++)
                a.SendMessage(new MessageOnClick(Guid.NewGuid()));
            await WaitUntilAsync(() => targetB.Applied.Count == 3);

            targetA.Model = ModelWithOneComposition();
            await a.PushAsync();
            await WaitUntilAsync(() => targetB.SnapshotsApplied == 2);

            var snapshot = await store.ReadSnapshotAsync();
            Assert.Single(store.Entries);
            Assert.Equal(store.Entries[0].Id, snapshot.StreamId);
            Assert.Equal(ProjectStateHash.Compute(targetA.Model), ProjectStateHash.Compute(targetB.Model));
            await WaitUntilAsync(() => b.LastAppliedId == a.LastAppliedId);
        }

        [Fact]
        public async Task Recovery_AfterStreamTrimmedPastOwnPosition_ReappliesSnapshot()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            store.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && store.FailedCalls > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), tail, "other", DateTime.UtcNow));
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
            b.Start(Options("B"), autoJoin: false);
            await WaitUntilAsync(() => b.IsJoined);

            var targetA = new RecordingSyncTarget();
            using var a = CreatePeer(targetA, inner, isWriter: false);
            a.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => a.IsJoined);

            storeB.SetFail(true);
            await WaitUntilAsync(() => !b.IsConnected && storeB.FailedCalls > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), tail, "other", DateTime.UtcNow));
            await inner.TrimAsync(tail);
            storeB.SetFail(false);

            await WaitUntilAsync(() => b.LastAppliedId == tail, 15000, () => $"status={b.Status} last={b.LastAppliedId}");
            var composition = Assert.Single(project.CompositionManager.ManagerData.Items);
            Assert.Equal(model.CompositionManager.ManagerData.Items[0].ID, composition.ID);

            await Task.Delay(300);
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
            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            store.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && store.FailedCalls > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), tail, "other", DateTime.UtcNow));
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
            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            store.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && store.FailedCalls > 0);
            await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var model = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), tail, "other", DateTime.UtcNow));
            await inner.TrimAsync(tail);

            // Holds the recovery open over two heartbeat intervals.
            var reached = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            store.BeforeReadSnapshot = async () =>
            {
                reached.TrySetResult(true);
                await release.Task;
            };
            store.SetFail(false);
            await WaitUntilAsync(() => reached.Task.IsCompleted, 15000, () => $"status={peer.Status}");

            await Task.Delay(5000);
            release.SetResult(true);
            await Task.Delay(2000);

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(tail, peer.LastAppliedId);
        }

        // A replacement host with an earlier clock gives entry IDs below the position the peer
        // holds. Without a re-join the reader waits for entries that never come.
        [Fact]
        public async Task WhenTheTailIsBelowTheOwnPosition_ThePeerJoinsTheNewStore()
        {
            var inner = new InMemorySyncStore(1000000);
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store);
            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);
            Assert.True(peer.LastAppliedId > StreamPosition.Zero);

            var replacement = new InMemorySyncStore();
            await replacement.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var snapshotId = await replacement.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await replacement.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), snapshotId, "other", DateTime.UtcNow));
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
        public async Task Outgoing_KeepsOrder()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget();
            var targetB = new RecordingSyncTarget();
            using var a = CreatePeer(targetA, store);
            using var b = CreatePeer(targetB, store);
            a.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => a.IsJoined);
            b.Start(Options("B"), autoJoin: true);
            await WaitUntilAsync(() => b.IsJoined);

            var ids = Enumerable.Range(0, 100).Select(_ => Guid.NewGuid()).ToArray();
            foreach (var id in ids)
                a.SendMessage(new MessageOnClick(id));
            await WaitUntilAsync(() => targetB.Applied.Count == 100);

            Assert.Equal(ids, targetB.Applied.Select(m => m.ID));
        }

        [Fact]
        public async Task RemoveThenAdd_ArriveInOrder()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget();
            var targetB = new RecordingSyncTarget();
            using var a = CreatePeer(targetA, store);
            using var b = CreatePeer(targetB, store);
            a.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => a.IsJoined);
            b.Start(Options("B"), autoJoin: true);
            await WaitUntilAsync(() => b.IsJoined);

            var managerId = Guid.NewGuid();
            a.SendMessage(new MessageRemoveItem(managerId, Guid.NewGuid(), -1));
            a.SendMessage(new MessageAddItem(managerId, new GenericValueModel<float> { ID = Guid.NewGuid(), Value = 1f }, 0));
            await WaitUntilAsync(() => targetB.Applied.Count == 2);

            Assert.IsType<MessageRemoveItem>(targetB.Applied[0]);
            Assert.IsType<MessageAddItem>(targetB.Applied[1]);
        }

        // A retry of an append that timed out puts the same envelope into the stream a second time.
        [Fact]
        public async Task TheSameEntryTwice_IsAppliedOnce()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            var envelope = Envelope("other", new MessageOnClick(Guid.NewGuid()));
            await store.AppendAsync(envelope);
            var second = await store.AppendAsync(envelope);

            await WaitUntilAsync(() => peer.LastAppliedId == second, detail: () => $"last={peer.LastAppliedId}");
            await Task.Delay(300);

            Assert.Single(target.Applied);
            Assert.Equal(1, peer.AppliedMessages);
            Assert.Equal(second, peer.LastAppliedId);
        }

        // Two clicks on the same control are two messages with one payload ID. Only the message ID
        // tells a repeat from a second edit.
        [Fact]
        public async Task TwoEntriesWithTheSamePayloadId_AreBothApplied()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            var id = Guid.NewGuid();
            await store.AppendAsync(Envelope("other", new MessageOnClick(id)));
            var second = await store.AppendAsync(Envelope("other", new MessageOnClick(id)));

            await WaitUntilAsync(() => target.Applied.Count == 2, detail: () => $"applied={target.Applied.Count}");
            Assert.Equal(second, peer.LastAppliedId);
        }

        [Fact]
        public void Start_AndStop_ReturnAtOnce_WhenTheStoreNeverConnects()
        {
            var messenger = new ControlMessenger();
            using var peer = CreatePeer(new RecordingSyncTarget(), new HangingSyncStore(), messenger);

            var watch = Stopwatch.StartNew();
            peer.Start(Options("A"), autoJoin: true);
            Assert.True(watch.ElapsedMilliseconds < 50);
            Assert.Equal("Connecting", peer.Status);
            Assert.True(messenger.IsSendingBlocked);

            watch.Restart();
            peer.SendMessage(new MessageOnClick(Guid.NewGuid()));
            Assert.True(watch.ElapsedMilliseconds < 50);
            Assert.Equal(1, peer.PendingMessages);

            watch.Restart();
            peer.Stop();
            Assert.True(watch.ElapsedMilliseconds < 50);
            Assert.Equal("Not started", peer.Status);
            Assert.Equal(StreamPosition.Zero, peer.LastAppliedId);
            Assert.Equal(StreamPosition.Zero, peer.TailId);
            Assert.Empty(peer.Peers);
        }

        // A test on a real server deletes its keys after Stopped, so no late call may write one.
        [Fact]
        public async Task Stopped_CompletesAfterTheLastStoreCall()
        {
            var store = new WrappingSyncStore(new InMemorySyncStore());
            using var peer = CreatePeer(new RecordingSyncTarget(), store);

            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);
            var stopped = peer.Stopped;
            peer.Stop();

            Assert.Same(stopped, await Task.WhenAny(stopped, Task.Delay(5000)));

            var calls = store.CallThreads.Count;
            await Task.Delay(500);

            Assert.Equal(calls, store.CallThreads.Count);
        }

        [Fact]
        public async Task StoreCalls_NeverRunOnTheDispatcherThread()
        {
            using var dispatcher = new SingleThreadDispatcher();
            var store = new WrappingSyncStore(new InMemorySyncStore());
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.SetDispatcher(dispatcher.Post);

            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);
            peer.SendMessage(new MessageOnClick(Guid.NewGuid()));
            await WaitUntilAsync(() => peer.SentMessages == 2);
            await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await WaitUntilAsync(() => target.Applied.Count == 1);

            Assert.NotEmpty(store.CallThreads);
            Assert.DoesNotContain(dispatcher.ThreadId, store.CallThreads);
        }

        [Fact]
        public async Task Engine_WhenTheFirstJoinFails_TriesAgainAndJoins()
        {
            var inner = new InMemorySyncStore();
            var store = new UnreliableSyncStore(inner) { Fail = true };
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);

            peer.Start(Options("Engine"), autoJoin: true);
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
            await inner.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), tail, "other", DateTime.UtcNow));

            var store = new UnreliableSyncStore(inner) { Fail = true };
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);

            peer.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.ErrorMessage.Length > 0, detail: () => $"status={peer.Status}");

            store.Fail = false;

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            await Task.Delay(200);

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

            peer.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.ErrorMessage.Length > 0, detail: () => $"status={peer.Status}");

            store.SetConnected(false);
            await WaitUntilAsync(() => !peer.IsConnected);

            // Longer than the first backoff, so the peer waits for the connection event.
            await Task.Delay(1200);
            Assert.False(peer.IsJoined);

            store.Fail = false;
            store.SetConnected(true);

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            Assert.Equal(string.Empty, peer.ErrorMessage);
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
            await innerA.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(modelA), tailA, "other", DateTime.UtcNow));
            var storeA = new WrappingSyncStore(innerA);

            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            var modelB = ModelWithOneComposition();
            var tailB = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await storeB.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(modelB), tailB, "other", DateTime.UtcNow));
            Assert.NotEqual(ProjectStateHash.Compute(modelA), ProjectStateHash.Compute(modelB));

            var reached = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var reads = 0;
            storeA.BeforeReadSnapshot = async () =>
            {
                Interlocked.Increment(ref reads);
                reached.TrySetResult(true);
                await release.Task;
            };

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = false };

            peer.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => reached.Task.IsCompleted, detail: () => $"status={peer.Status}");

            peer.Stop();
            peer.Start(Options("Engine"), autoJoin: true);

            // Stop disposed the old store. Connect it again, so the read that waits can return a
            // snapshot and the test sees what the peer does with it.
            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 15000);
            await innerA.ConnectAsync(default);
            release.SetResult(true);

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            await Task.Delay(200);

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(modelB), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tailB, peer.LastAppliedId);
            Assert.Equal(1, Volatile.Read(ref reads));
            Assert.Equal(0, storeA.WriteSnapshotCalls);
            Assert.Empty(storeA.TrimCalls);
            Assert.Single(innerA.Entries);
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

            var reached = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            storeA.BeforeAppend = async () =>
            {
                reached.TrySetResult(true);
                await release.Task;
            };

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { IsWriter = true };

            // Both stores are empty, so the start check pushes the local state.
            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => reached.Task.IsCompleted, detail: () => $"status={peer.Status}");

            peer.Stop();
            peer.Start(Options("Studio"), autoJoin: false);

            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 15000);
            await innerA.ConnectAsync(default);
            release.SetResult(true);

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            await Task.Delay(200);

            var snapshot = await storeB.ReadSnapshotAsync();
            Assert.NotNull(snapshot);
            var stored = VL.Serialization.MessagePack.MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));
            Assert.Equal(ProjectStateHash.Compute(target.Model), ProjectStateHash.Compute(stored));
            Assert.Single(storeB.Entries);
            Assert.Equal(1, peer.SentMessages);

            Assert.Equal(0, storeA.WriteSnapshotCalls);
            Assert.Empty(storeA.TrimCalls);
            Assert.Null(await innerA.ReadSnapshotAsync());
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        // The follower recovery reads the snapshot of the store it follows. A restart while it reads
        // must not put that snapshot into the peer.
        [Fact]
        public async Task RestartDuringAFollowerRecovery_KeepsTheStateOfTheNewStore()
        {
            // The IDs of the old store are above the IDs of the new one. An unguarded recovery would
            // therefore find its snapshot newer than the position of the new store.
            var innerA = new InMemorySyncStore(1000000);
            var storeA = new WrappingSyncStore(innerA);

            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            var modelB = ModelWithOneComposition();
            var tailB = await storeB.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await storeB.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(modelB), tailB, "other", DateTime.UtcNow));

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue());

            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            // The stream of the old store is trimmed past the own position, so the recovery reads
            // the snapshot.
            storeA.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected && storeA.FailedCalls > 0);
            await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var tailA = await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var modelA = ModelWithOneComposition();
            await innerA.WriteSnapshotAsync(new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(modelA), tailA, "other", DateTime.UtcNow));
            await innerA.TrimAsync(tailA);

            var reached = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            storeA.BeforeReadSnapshot = async () =>
            {
                reached.TrySetResult(true);
                await release.Task;
            };
            storeA.SetFail(false);
            await WaitUntilAsync(() => reached.Task.IsCompleted, 15000, () => $"status={peer.Status}");

            peer.Stop();
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");

            // Stop disposed the old store. Connect it again, so the read that waits can return the
            // snapshot and the test sees what the peer does with it.
            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 15000);
            await innerA.ConnectAsync(default);
            release.SetResult(true);
            await Task.Delay(300);

            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(modelB), ProjectStateHash.Compute(target.Model));
            Assert.Equal(tailB, peer.LastAppliedId);
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        // A presence tick of the store that the restart left behind must not move the tail or the
        // peer list.
        [Fact]
        public async Task RestartDuringAPresenceTick_KeepsTheTailAndThePeersOfTheNewStore()
        {
            var innerA = new InMemorySyncStore(1000000);
            var storeA = new WrappingSyncStore(innerA);
            var storeB = new InMemorySyncStore();
            await storeB.ConnectAsync(default);
            await storeB.HeartbeatAsync("fresh", new Dictionary<string, string> { ["name"] = "New" }, TimeSpan.FromMinutes(1));

            var stores = new Queue<ISyncStore>(new ISyncStore[] { storeA, storeB });
            var target = new RecordingSyncTarget();
            using var peer = new SyncPeer(target, new ControlMessenger(), _ => stores.Dequeue()) { ListPeersEnabled = true };

            peer.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status}");

            // The join read the tail already. From here only a presence tick reads it.
            var reached = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            storeA.BeforeReadTail = async () =>
            {
                reached.TrySetResult(true);
                await release.Task;
            };
            await WaitUntilAsync(() => reached.Task.IsCompleted, 15000, () => $"status={peer.Status}");

            peer.Stop();
            peer.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            await WaitUntilAsync(() => PeerIds(peer).Contains("fresh"), 15000, () => $"status={peer.Status}");

            // The call that waits keeps the old run alive, so the store closes after the stop timeout.
            await WaitUntilAsync(() => !innerA.IsConnected, 15000);
            await innerA.ConnectAsync(default);
            await innerA.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await innerA.HeartbeatAsync("ghost", new Dictionary<string, string> { ["name"] = "Old" }, TimeSpan.FromMinutes(1));
            release.SetResult(true);
            await Task.Delay(300);

            Assert.Equal(StreamPosition.Zero, peer.TailId);
            Assert.DoesNotContain("ghost", PeerIds(peer));
            Assert.Contains("fresh", PeerIds(peer));
        }

        // The presence tick replaces Peers on its own thread. Copy it again when the copy fails.
        private static string[] PeerIds(SyncPeer peer)
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    return peer.Peers.Select(info => info.PeerId).ToArray();
                }
                catch (InvalidOperationException)
                {
                }
            }

            return Array.Empty<string>();
        }

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
                peer.Start(Options("A"), autoJoin: true);
                await WaitUntilAsync(() => peer.IsJoined, detail: () => $"iteration {i}");

                var join = Task.Run(() => peer.JoinAsync());
                Assert.Null(Record.Exception(() => peer.Stop()));
                Assert.Null(await Record.ExceptionAsync(() => join));
            }
        }

        [Fact]
        public async Task LostConnection_ShowsTheStoreReason_AndAReconnectClearsIt()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.Start(Options("A"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            inner.SimulateDisconnect("test reason");
            await WaitUntilAsync(() => peer.ErrorMessage == "test reason", detail: () => $"error={peer.ErrorMessage}");

            inner.SimulateReconnect();
            await WaitUntilAsync(() => peer.ErrorMessage.Length == 0, detail: () => $"error={peer.ErrorMessage}");
        }

        // A store that fails its data calls while it still reports the connection, like a Redis
        // client whose commands time out. A test can also take the connection away.
        private sealed class UnreliableSyncStore : ISyncStore
        {
            private readonly ISyncStore _inner;
            private int _failedCalls;
            private volatile bool _connected = true;
            private volatile bool _fail;

            public UnreliableSyncStore(ISyncStore inner) => _inner = inner;

            public bool Fail { get => _fail; set => _fail = value; }
            public int FailedCalls => Volatile.Read(ref _failedCalls);

            public bool IsConnected => _connected;
            public string LastError => _inner.LastError;
            public event Action<bool> ConnectionChanged;

            public void SetConnected(bool connected)
            {
                _connected = connected;
                ConnectionChanged?.Invoke(connected);
            }

            private void Enter()
            {
                if (!_fail && _connected) return;

                Interlocked.Increment(ref _failedCalls);
                throw new InvalidOperationException("The store is failing.");
            }

            public Task ConnectAsync(CancellationToken ct) => _inner.ConnectAsync(ct);
            public Task<Snapshot> ReadSnapshotAsync() { Enter(); return _inner.ReadSnapshotAsync(); }
            public Task<StreamPosition> ReadSnapshotIdAsync() { Enter(); return _inner.ReadSnapshotIdAsync(); }
            public Task WriteSnapshotAsync(Snapshot snapshot) { Enter(); return _inner.WriteSnapshotAsync(snapshot); }
            public Task<StreamPosition> AppendAsync(byte[] envelope) { Enter(); return _inner.AppendAsync(envelope); }
            public Task<IReadOnlyList<StreamEntry>> ReadRangeAsync(StreamPosition afterExclusive, int count) { Enter(); return _inner.ReadRangeAsync(afterExclusive, count); }
            public Task<IReadOnlyList<StreamEntry>> ReadBlockingAsync(StreamPosition afterExclusive, TimeSpan timeout, CancellationToken ct) { Enter(); return _inner.ReadBlockingAsync(afterExclusive, timeout, ct); }
            public Task<StreamPosition> ReadTailAsync() { Enter(); return _inner.ReadTailAsync(); }
            public Task TrimAsync(StreamPosition minId) { Enter(); return _inner.TrimAsync(minId); }
            public Task HeartbeatAsync(string peerId, IReadOnlyDictionary<string, string> fields, TimeSpan ttl) { Enter(); return _inner.HeartbeatAsync(peerId, fields, ttl); }
            public Task<IReadOnlyList<PeerInfo>> ListPeersAsync() { Enter(); return _inner.ListPeersAsync(); }
            public ValueTask DisposeAsync() => _inner.DisposeAsync();
        }
    }
}
