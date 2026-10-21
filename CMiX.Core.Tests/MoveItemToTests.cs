using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // MoveItemTo is the move that the context menu uses: one undo step, one message, bad indices do nothing.
    public class MoveItemToTests
    {
        private static (IServiceProvider provider, PrefabManager layers, List<Layer> items) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));
            return (provider, layers, layers.ManagerData.Items.Cast<Layer>().ToList());
        }

        [Fact]
        public void MoveItemTo_MovesTheItemAndUndoPutsItBack()
        {
            var (provider, layers, items) = Create();
            var undo = provider.GetRequiredService<UndoManager>();

            layers.MoveItemTo(items[0], 2);
            Assert.Equal(new[] { items[1], items[2], items[0] }, layers.ManagerData.Items);

            undo.Undo();
            Assert.Equal(items, layers.ManagerData.Items);

            undo.Redo();
            Assert.Equal(new[] { items[1], items[2], items[0] }, layers.ManagerData.Items);
        }

        [Fact]
        public void MoveItemTo_SendsOneMoveMessage()
        {
            var (provider, layers, items) = Create();
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<CMiX.Core.Networking.ControlMessenger>().Register(sender);

            layers.MoveItemTo(items[2], 0);

            var message = Assert.Single(sender.Sent.OfType<MessageMoveItem>());
            Assert.Equal(2, message.OldIndex);
            Assert.Equal(0, message.NewIndex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3)]
        public void MoveItemTo_WithTheSameOrABadIndex_DoesNothing(int newIndex)
        {
            var (provider, layers, items) = Create();
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<CMiX.Core.Networking.ControlMessenger>().Register(sender);

            layers.MoveItemTo(items[0], newIndex);

            Assert.Equal(items, layers.ManagerData.Items);
            Assert.Empty(sender.Sent.OfType<MessageMoveItem>());
        }

        [Fact]
        public void MoveItemTo_ForAnItemOfAnotherList_DoesNothing()
        {
            var (_, layers, items) = Create();
            var other = Create().items[0];

            layers.MoveItemTo(other, 1);

            Assert.Equal(items, layers.ManagerData.Items);
        }
    }
}
