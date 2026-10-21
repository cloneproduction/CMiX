using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The context menu deletes any row, the last composition included. Delete has no confirmation, so undo must work.
    public class DeleteLastItemTests
    {
        [Fact]
        public void RemoveItem_OfTheOnlyComposition_LeavesNoneAndUndoBringsItBack()
        {
            var provider = TestServiceProviderFactory.Create();
            var undo = provider.GetRequiredService<UndoManager>();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var composition = (Composition)compositions.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));

            compositions.RemoveItem(composition);

            Assert.Empty(compositions.ManagerData.Items);
            Assert.Null(compositions.SelectedItem);

            undo.Undo();

            Assert.Same(composition, Assert.Single(compositions.ManagerData.Items));
            Assert.Single(composition.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void RemoveItem_OfAMiddleLayer_UndoRestoresItAtTheSameIndex()
        {
            var provider = TestServiceProviderFactory.Create();
            var undo = provider.GetRequiredService<UndoManager>();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));
            var items = layers.ManagerData.Items.ToList();

            layers.RemoveItem(items[1]);
            Assert.Equal(new[] { items[0], items[2] }, layers.ManagerData.Items);

            undo.Undo();
            Assert.Equal(items, layers.ManagerData.Items);
        }
    }
}
