using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
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

        private sealed class DisposableControl : IControl, IDisposable
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public bool IsDisposed { get; private set; }
            public IControlModel ToModel() => throw new NotSupportedException();
            public void FromModel(IControlModel model) => throw new NotSupportedException();
            public void Dispose() => IsDisposed = true;
        }

        [Fact]
        public void Insert_OfAnItem_PutsItInTheListAndSendsAdd()
        {
            var (sync, sender, item) = Create();
            sync.Collection.RemoveItem(item);
            IItemList list = sync;

            list.Insert(item, 0);

            Assert.Same(item, Assert.Single(sync.Collection.ManagerData.Items));
            var message = Assert.IsType<MessageAddItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal(item.ID, message.Model.ID);
            Assert.Equal(0, message.SelectedIndex);
        }

        [Fact]
        public void Remove_OfAnItem_TakesItOutAndSendsRemoveWithTheSelectedIndex()
        {
            var (sync, sender, item) = Create();
            IItemList list = sync;

            var index = list.Remove(item);

            Assert.Equal(0, index);
            Assert.Equal(0, list.Count);
            var message = Assert.IsType<MessageRemoveItem>(Assert.Single(sender.Sent));
            Assert.Equal(item.ID, message.ModelID);
            Assert.Equal(list.SelectedIndex, message.SelectedIndex);
        }

        [Fact]
        public void Move_OfTwoIndices_MovesTheItemAndSendsMove()
        {
            var (sync, sender, item) = Create();
            sync.Collection.InsertItem(new DisposableControl(), 1);
            IItemList list = sync;

            list.Move(0, 1);

            Assert.Equal(1, list.IndexOf(item));
            var message = Assert.IsType<MessageMoveItem>(Assert.Single(sender.Sent));
            Assert.Equal((0, 1), (message.OldIndex, message.NewIndex));
        }

        [Fact]
        public void SelectAt_OfAnIndex_SelectsTheItemAndSendsSelect()
        {
            var (sync, sender, item) = Create();
            IItemList list = sync;

            list.SelectAt(0);

            Assert.Same(item, list.SelectedItem);
            var message = Assert.IsType<MessageSelectItem>(Assert.Single(sender.Sent));
            Assert.Equal(item.ID, message.ControlID);
            Assert.Equal(0, message.Index);
        }

        [Fact]
        public void ClearSelection_OfASelectedList_ClearsAndSendsClearSelection()
        {
            var (sync, sender, _) = Create();
            sync.Collection.ManagerData.SelectAt(0);
            IItemList list = sync;

            list.ClearSelection();

            Assert.Null(list.SelectedItem);
            var message = Assert.IsType<MessageClearSelection>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
        }

        [Fact]
        public void AddFromModel_OfAModel_AddsSelectsAndSendsAdd()
        {
            var (sync, sender, _) = Create();
            var model = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(CheckerBoard)).ToModel();
            IItemList list = sync;

            list.AddFromModel(model);

            Assert.Equal(2, list.Count);
            Assert.Equal(model.ID, list.SelectedItem.ID);
            var message = Assert.IsType<MessageAddItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal(model.ID, message.Model.ID);
            Assert.Equal(1, message.SelectedIndex);
        }

        [Fact]
        public void Clear_OfAListWithItems_EmptiesItAndSendsOneRemovePerItemWithIndexMinusOne()
        {
            var (sync, sender, item) = Create();
            var second = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(CheckerBoard));
            sync.Collection.InsertItem(second, 1);
            IItemList list = sync;

            list.Clear();

            Assert.Equal(0, list.Count);
            var messages = sender.Sent.Select(sent => Assert.IsType<MessageRemoveItem>(sent)).ToList();
            Assert.Equal(new[] { item.ID, second.ID }, messages.Select(message => message.ModelID));
            Assert.All(messages, message => Assert.Equal(-1, message.SelectedIndex));
            Assert.All(messages, message => Assert.Equal(sync.Collection.ManagerData.ID, message.ID));
        }

        [Fact]
        public void Release_OfAnItemInNoList_DisposesIt()
        {
            var (sync, sender, _) = Create();
            var item = new DisposableControl();
            IItemList list = sync;

            list.Release(item);

            Assert.True(item.IsDisposed);
            Assert.Empty(sender.Sent);
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
