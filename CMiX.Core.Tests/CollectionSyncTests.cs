using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using VL.Serialization.MessagePack;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every message that CollectionSync sends, its trip over the wire, and how a peer applies an add.
    public class CollectionSyncTests
    {
        private static (CollectionSync Sync, RecordingMessageSender Sender, IControl Item) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            var collection = provider.GetRequiredService<CollectionManager>();
            var item = provider.GetRequiredService<ControlFactory>().Create(typeof(CheckerBoard));
            collection.InsertItem(item, 0);
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);
            return (new CollectionSync(collection, messenger, provider.GetRequiredService<MessageFactory>()), sender, item);
        }

        [Fact]
        public void SendAdd_OfAnItem_SendsTheItemAndItsIndexUnderTheIdOfTheCollection()
        {
            var (sync, sender, item) = Create();

            sync.SendAdd(item, 3);

            var message = Assert.IsType<MessageAddItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal(item.ID, message.Model.ID);
            Assert.Equal(3, message.SelectedIndex);
        }

        [Fact]
        public void SendRemove_OfAnItem_SendsItsIdAndTheSelectedIndex()
        {
            var (sync, sender, item) = Create();

            sync.SendRemove(item, 2);

            var message = Assert.IsType<MessageRemoveItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal(item.ID, message.ModelID);
            Assert.Equal(2, message.SelectedIndex);
        }

        [Fact]
        public void SendMove_OfTwoIndices_SendsBoth()
        {
            var (sync, sender, _) = Create();

            sync.SendMove(1, 4);

            var message = Assert.IsType<MessageMoveItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal((1, 4), (message.OldIndex, message.NewIndex));
        }

        [Fact]
        public void SendSelect_OfAnItem_SendsTheItemAndTheIndex()
        {
            var (sync, sender, item) = Create();

            sync.SendSelect(item, 0);

            var message = Assert.IsType<MessageSelectItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal(item.ID, message.ControlID);
            Assert.Equal(0, message.Index);
        }

        [Fact]
        public void SendClearSelection_OfACollection_SendsItsId()
        {
            var (sync, sender, _) = Create();

            sync.SendClearSelection();

            var message = Assert.IsType<MessageClearSelection>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
        }

        private static MessageEnvelope Roundtrip(IMessage message) =>
            MessagePackSerialization.Deserialize<MessageEnvelope>(new ReadOnlyMemory<byte>(SyncTestHelpers.Envelope("peer", message)));

        [Fact]
        public void Deserialize_OfASelectMessage_KeepsItsTypeAndValues()
        {
            var id = Guid.NewGuid();
            var controlID = Guid.NewGuid();

            var message = Assert.IsType<MessageSelectItem>(Roundtrip(new MessageSelectItem(id, controlID, 2)).Payload);

            Assert.Equal((id, controlID, 2), (message.ID, message.ControlID, message.Index));
        }

        [Fact]
        public void Deserialize_OfAClearSelectionMessage_KeepsItsTypeAndValues()
        {
            var id = Guid.NewGuid();

            var message = Assert.IsType<MessageClearSelection>(Roundtrip(new MessageClearSelection(id)).Payload);

            Assert.Equal(id, message.ID);
        }

        // A late engine replays entries after a snapshot. An add of an item that the snapshot holds must not add it twice.
        private static IControlModel CompositionModel()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return project.CompositionManager.ManagerData.Items[0].ToModel();
        }

        [Fact]
        public void Receive_AnAddOfAnItemAlreadyInTheList_UpdatesOnlyTheSelectedIndex()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var manager = project.CompositionManager;
            var model = CompositionModel();
            var message = new MessageAddItem(manager.ManagerData.ID, model, 0);

            manager.Receive(message);
            manager.Receive(message);

            var item = Assert.Single(manager.ManagerData.Items);
            Assert.Equal(model.ID, item.ID);
            Assert.Equal(message.SelectedIndex, manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void Receive_AddsOfTwoItems_AddsBoth()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var manager = project.CompositionManager;
            var firstModel = CompositionModel();
            var secondModel = CompositionModel();

            manager.Receive(new MessageAddItem(manager.ManagerData.ID, firstModel, 0));
            manager.Receive(new MessageAddItem(manager.ManagerData.ID, secondModel, 1));

            Assert.Equal(2, manager.ManagerData.Items.Count);
        }

        [Fact]
        public void Receive_AnAddWithAnIndexInTheMiddle_InsertsAtThatIndex()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var manager = project.CompositionManager;
            var first = CompositionModel();
            var second = CompositionModel();
            var inserted = CompositionModel();

            manager.Receive(new MessageAddItem(manager.ManagerData.ID, first, 0));
            manager.Receive(new MessageAddItem(manager.ManagerData.ID, second, 1));
            manager.Receive(new MessageAddItem(manager.ManagerData.ID, inserted, 1));

            Assert.Equal(new[] { first.ID, inserted.ID, second.ID }, manager.ManagerData.Items.Select(item => item.ID));
            Assert.Equal(1, manager.ManagerData.SelectedIndex);
        }
    }
}
