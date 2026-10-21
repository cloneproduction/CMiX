using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Redo of an add puts the item back at the index where it was added, not at the end.
    public class AddItemUndoCommandTests
    {
        [Fact]
        public void UndoThenRedo_OfAnItemInTheMiddle_KeepsItsIndex()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            layers.AddItem(typeof(Layer));
            layers.AddItem(typeof(Layer));
            layers.AddItem(typeof(Layer));
            var items = layers.ManagerData.Items.ToList();

            var added = provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var sync = new CollectionSync(layers.Collection, layers.ControlMessenger, layers.MessageFactory);
            var command = new AddItemUndoCommand(sync, added, 1);

            command.Execute();
            Assert.Equal(new[] { items[0], added, items[1], items[2] }, layers.ManagerData.Items);

            command.Undo();
            Assert.Equal(items, layers.ManagerData.Items);

            command.Execute();
            Assert.Equal(new[] { items[0], added, items[1], items[2] }, layers.ManagerData.Items);
            Assert.Equal(1, layers.ManagerData.SelectedIndex);
        }
    }
}
