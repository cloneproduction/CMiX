using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
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
            using var peer = CreatePeer(new ProjectSyncTarget(project), store, messenger);
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
    }
}
