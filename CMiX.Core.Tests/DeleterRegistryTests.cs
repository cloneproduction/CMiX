using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
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

        // A texture source owns a manager of filter modifiers the same way an entity owns a manager
        // of its modifiers, but it used to be the one owner of a nested manager that was not
        // disposable, so tearing a texture down left every filter it held in the repository and
        // every one of its managers in the deleter registry.
        [Fact]
        public void DroppingATexture_TakesItsFilterModifiersWithIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var textureManager = provider.GetRequiredService<PrefabManager>();

            var beforeTheTexture = repository.DeleterCount;

            textureManager.AddItem(typeof(CheckerBoard));
            var texture = (CheckerBoard)textureManager.SelectedItem;
            texture.TextureModifierManager.AddItem(typeof(Blur));
            var filter = (Blur)texture.TextureModifierManager.SelectedItem;

            Assert.Same(filter, repository.GetControl(filter.ID));
            Assert.True(repository.DeleterCount > beforeTheTexture);

            textureManager.DeleteItem(texture);

            // The undo entry still owns the texture, so nothing is torn down until it drops.
            Assert.Same(filter, repository.GetControl(filter.ID));

            undoManager.Clear();

            Assert.Null(repository.GetControl(texture.ID));
            Assert.Null(repository.GetControl(filter.ID));
            Assert.Equal(beforeTheTexture, repository.DeleterCount);
        }

        [Fact]
        public void DroppingADeletedEntity_UnregistersItsNestedManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var entityManager = provider.GetRequiredService<PrefabManager>();

            var beforeTheEntity = repository.DeleterCount;

            entityManager.AddItem(typeof(Entity));
            var entity = (Entity)entityManager.SelectedItem;

            Assert.Same(entity, repository.GetControl(entity.ID));
            Assert.True(repository.DeleterCount > beforeTheEntity);

            entityManager.DeleteItem(entity);
            undoManager.Clear();

            Assert.Null(repository.GetControl(entity.ID));
            Assert.Equal(beforeTheEntity, repository.DeleterCount);
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
