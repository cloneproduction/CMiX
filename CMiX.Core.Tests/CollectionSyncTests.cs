using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
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
        public void SendAdd_SendsTheItemAndItsIndexUnderTheIdOfTheCollection()
        {
            var (sync, sender, item) = Create();

            sync.SendAdd(item, 3);

            var message = Assert.IsType<MessageAddItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal(item.ID, message.Model.ID);
            Assert.Equal(3, message.SelectedIndex);
        }

        [Fact]
        public void SendRemove_SendsTheIdOfTheItemAndTheSelectedIndex()
        {
            var (sync, sender, item) = Create();

            sync.SendRemove(item, 2);

            var message = Assert.IsType<MessageRemoveItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal(item.ID, message.ModelID);
            Assert.Equal(2, message.SelectedIndex);
        }

        [Fact]
        public void SendMove_SendsBothIndices()
        {
            var (sync, sender, _) = Create();

            sync.SendMove(1, 4);

            var message = Assert.IsType<MessageMoveItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal((1, 4), (message.OldIndex, message.NewIndex));
        }

        [Fact]
        public void SendSelect_SendsTheItemAndTheIndex()
        {
            var (sync, sender, item) = Create();

            sync.SendSelect(item, 0);

            var message = Assert.IsType<MessageSelectedItemChanged>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
            Assert.Equal(item.ID, message.ControlID);
            Assert.Equal(0, message.Index);
        }

        [Fact]
        public void SendRemoveSelected_SendsTheIdOfTheCollection()
        {
            var (sync, sender, _) = Create();

            sync.SendClearSelection();

            var message = Assert.IsType<MessageRemoveSelectedItem>(Assert.Single(sender.Sent));
            Assert.Equal(sync.Collection.ManagerData.ID, message.ID);
        }
    }
}
