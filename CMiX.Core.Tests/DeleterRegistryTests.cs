using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every manager registers a deleter that captures the manager, its collection and its items,
    // so an entry that outlives its manager pins the whole graph it was holding.
    public class DeleterRegistryTests
    {
        [Fact]
        public void RegisterDeleter_AfterAnIdReassignment_LeavesNoEntryUnderTheOldId()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var manager = provider.GetRequiredService<PrefabManager>();

            var countAfterConstruction = repository.DeleterCount;

            manager.ManagerData.ID = Guid.NewGuid();
            manager.RegisterDeleter();

            Assert.Equal(countAfterConstruction, repository.DeleterCount);
        }

        [Fact]
        public void DeletingLayersRepeatedly_DoesNotGrowTheDeleterRegistry()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            AddAndDropALayer(composition, undoManager);
            var baseline = repository.DeleterCount;

            for (int i = 0; i < 5; i++)
                AddAndDropALayer(composition, undoManager);

            Assert.Equal(baseline, repository.DeleterCount);
        }

        [Fact]
        public void DroppingTheUndoEntryOfADeletedLayer_UnregistersItsNestedManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            var beforeTheLayer = repository.DeleterCount;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            Assert.True(repository.DeleterCount > beforeTheLayer);

            composition.LayerManager.DeleteItem(layer);

            // The undo entry still holds the layer, so its managers stay registered until it drops.
            Assert.True(repository.DeleterCount > beforeTheLayer);

            undoManager.Clear();

            Assert.Equal(beforeTheLayer, repository.DeleterCount);
        }

        private static void AddAndDropALayer(Composition composition, UndoManager undoManager)
        {
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));

            composition.LayerManager.DeleteItem(layer);
            undoManager.Clear();
        }
    }
}
