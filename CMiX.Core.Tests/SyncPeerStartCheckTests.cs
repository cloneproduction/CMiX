using System;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Microsoft.Extensions.DependencyInjection;
using VL.Serialization.MessagePack;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // The Studio start check: the peer connects without a join and reports what the user must do.
    public class SyncPeerStartCheckTests
    {
        private static ProjectModel ModelWithOneComposition()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return (ProjectModel)project.ToModel();
        }

        private static Snapshot SnapshotOf(ProjectModel model, StreamPosition id) =>
            new Snapshot(MessagePackSerialization.Serialize(model), id, "other", DateTime.UtcNow);

        private static ProjectModel ModelOf(Snapshot snapshot) =>
            MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));

        [Fact]
        public async Task Start_OnEmptyStore_PushesSilently()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store);

            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");

            var snapshot = await store.ReadSnapshotAsync();
            Assert.NotNull(snapshot);
            Assert.Single(store.Entries);
            Assert.Equal("Connected", peer.Status);
            Assert.Equal(1, peer.SentMessages);
        }

        [Fact]
        public async Task Start_WithEqualSnapshotAndNoNewerEntries_JoinsWithoutTheSnapshot()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var tail = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await store.WriteSnapshotAsync(SnapshotOf(model, tail));

            var target = new RecordingSyncTarget { Model = model };
            using var peer = CreatePeer(target, store);

            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");

            Assert.Equal(tail, peer.LastAppliedId);
            Assert.Equal(0, target.SnapshotsApplied);
            Assert.Empty(target.Applied);
            Assert.Equal("Connected", peer.Status);
            Assert.Equal(StartCheck.AlreadyInSync, await peer.CheckStartAsync());
        }

        [Fact]
        public async Task Start_WithDifferentSnapshot_StaysNotInSync()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var tail = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await store.WriteSnapshotAsync(SnapshotOf(ModelWithOneComposition(), tail));

            var messenger = new ControlMessenger();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store, messenger);

            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsConnected && peer.Status == "Not in sync", detail: () => $"status={peer.Status}");

            Assert.False(peer.IsJoined);
            Assert.True(messenger.IsSendingBlocked);
            Assert.Equal(0, target.SnapshotsApplied);
            Assert.Equal(StartCheck.NotInSync, await peer.CheckStartAsync());
        }

        [Fact]
        public async Task Start_WithEqualSnapshotAndANewerEntry_IsNotInSync()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var atSnapshot = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await store.WriteSnapshotAsync(SnapshotOf(model, atSnapshot));
            await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));

            var target = new RecordingSyncTarget { Model = model };
            using var peer = CreatePeer(target, store);

            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsConnected && peer.Status == "Not in sync", detail: () => $"status={peer.Status}");

            Assert.False(peer.IsJoined);
            Assert.Equal(StartCheck.NotInSync, await peer.CheckStartAsync());
        }

        [Fact]
        public async Task Start_WithoutSnapshotButANonEmptyStream_IsNotInSync()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));

            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store);

            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsConnected && peer.Status == "Not in sync", detail: () => $"status={peer.Status}");

            Assert.False(peer.IsJoined);
            Assert.Equal(StartCheck.NotInSync, await peer.CheckStartAsync());
        }

        [Fact]
        public async Task Pull_AfterNotInSync_AdoptsTheStoreState()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var storeModel = ModelWithOneComposition();
            var tail = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await store.WriteSnapshotAsync(SnapshotOf(storeModel, tail));

            var messenger = new ControlMessenger();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store, messenger);

            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsConnected && peer.Status == "Not in sync", detail: () => $"status={peer.Status}");

            await peer.JoinAsync();

            Assert.True(peer.IsJoined);
            Assert.False(messenger.IsSendingBlocked);
            Assert.Equal(1, target.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(storeModel), ProjectStateHash.Compute(target.Model));
        }

        [Fact]
        public async Task Push_AfterNotInSync_PublishesTheLocalState()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var tail = await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await store.WriteSnapshotAsync(SnapshotOf(ModelWithOneComposition(), tail));

            var messenger = new ControlMessenger();
            var localModel = ModelWithOneComposition();
            var target = new RecordingSyncTarget { Model = localModel };
            using var peer = CreatePeer(target, store, messenger);

            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsConnected && peer.Status == "Not in sync", detail: () => $"status={peer.Status}");

            await peer.PushAsync();

            Assert.True(peer.IsJoined);
            Assert.False(messenger.IsSendingBlocked);
            Assert.Single(store.Entries);
            var snapshot = await store.ReadSnapshotAsync();
            Assert.Equal(ProjectStateHash.Compute(localModel), ProjectStateHash.Compute(ModelOf(snapshot)));
        }

        [Fact]
        public async Task Reconnect_WhileNotInSync_RunsTheCheckAgain()
        {
            var inner = new InMemorySyncStore();
            await inner.ConnectAsync(default);
            var model = ModelWithOneComposition();
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await inner.WriteSnapshotAsync(SnapshotOf(ModelWithOneComposition(), tail));

            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = model };
            using var peer = CreatePeer(target, store);

            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsConnected && peer.Status == "Not in sync", detail: () => $"status={peer.Status}");
            Assert.Equal(StartCheck.NotInSync, await peer.CheckStartAsync());

            store.SetFail(true);
            await WaitUntilAsync(() => !peer.IsConnected);
            await inner.WriteSnapshotAsync(SnapshotOf(model, tail));
            store.SetFail(false);

            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");

            Assert.Equal(0, target.SnapshotsApplied);
            Assert.Equal("Connected", peer.Status);
        }
    }
}
