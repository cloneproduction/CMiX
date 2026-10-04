using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Covers the deferred disposal of a deleted control: the delete path no longer disposes, the
    // undo command that holds the removed instance does, and only once it is dropped. Reset item
    // follows the same ownership rule since it routes through ReplaceItemCommand, so its tests
    // live here too.
    public class DeleteUndoTests
    {
        private static (Composition composition, Layer layer, Entity entity) CreateGraph(
            IServiceProvider provider, PrefabManager compositionManager)
        {
            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;

            return (composition, layer, entity);
        }

        [Fact]
        public void DeleteThenUndo_RestoresTheLayerWithItsContents()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            var (composition, layer, entity) = CreateGraph(provider, compositionManager);

            composition.LayerManager.DeleteItem(layer);

            Assert.DoesNotContain(layer, composition.LayerManager.ManagerData.Items);

            undoManager.Undo();

            Assert.Contains(layer, composition.LayerManager.ManagerData.Items);
            Assert.Contains(entity, layer.ModelEntityManager.ManagerData.Items);
            Assert.Same(entity, repository.GetControl(entity.ID));
        }

        [Fact]
        public void DeleteThenDropTheUndoEntry_TearsTheLayerDown()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            var (composition, layer, entity) = CreateGraph(provider, compositionManager);

            composition.LayerManager.DeleteItem(layer);
            undoManager.Clear();

            Assert.Empty(layer.ModelEntityManager.ManagerData.Items);
            Assert.Null(repository.GetControl(entity.ID));
            Assert.Null(repository.GetControl(layer.ID));
        }

        [Fact]
        public void ResetItem_SwapsAFreshInstanceInAtTheSameIndex()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            var (composition, layer, _) = CreateGraph(provider, compositionManager);
            composition.LayerManager.AddItem(typeof(Layer));
            var secondLayer = (Layer)composition.LayerManager.SelectedItem;

            composition.LayerManager.ResetItem(layer);

            var items = composition.LayerManager.ManagerData.Items;
            Assert.Equal(2, items.Count);
            Assert.NotSame(layer, items[0]);
            Assert.IsType<Layer>(items[0]);
            Assert.Same(secondLayer, items[1]);
            Assert.Same(items[0], composition.LayerManager.SelectedItem);
            Assert.NotNull(repository.GetControl(items[0].ID));
            Assert.Null(repository.GetControl(layer.ID));
        }

        [Fact]
        public void ResetThenUndo_RestoresTheReplacedInstanceWithItsContents()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            var (composition, layer, entity) = CreateGraph(provider, compositionManager);

            composition.LayerManager.ResetItem(layer);
            var replacement = composition.LayerManager.ManagerData.Items[0];

            undoManager.Undo();

            Assert.Same(layer, composition.LayerManager.ManagerData.Items[0]);
            Assert.Contains(entity, layer.ModelEntityManager.ManagerData.Items);
            Assert.Same(entity, repository.GetControl(entity.ID));
            Assert.Null(repository.GetControl(replacement.ID));
        }

        // DeleteEverywhere removes a control from every manager that references it.
        [Fact]
        public void DeleteEverywhere_ControlReferencedByTwoManagers_RemovesItFromBothAndTheRepository()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var managerA = provider.GetRequiredService<PrefabManager>();
            var managerB = provider.GetRequiredService<PrefabManager>();

            managerA.AddItem(typeof(Entity));
            var entity = (Entity)managerA.SelectedItem;
            managerB.AddExistingItem(entity);

            Assert.Contains(entity, managerA.ManagerData.Items);
            Assert.Contains(entity, managerB.ManagerData.Items);
            undoManager.Clear();

            managerA.DeleteEverywhereCommand.Execute(entity);

            Assert.DoesNotContain(entity, managerA.ManagerData.Items);
            Assert.DoesNotContain(entity, managerB.ManagerData.Items);
            Assert.Null(repository.GetControl(entity.ID));
        }

        [Fact]
        public void DeleteEverywhereThenUndo_RestoresBothReferencesAsOneStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var managerA = provider.GetRequiredService<PrefabManager>();
            var managerB = provider.GetRequiredService<PrefabManager>();

            managerA.AddItem(typeof(Entity));
            var entity = (Entity)managerA.SelectedItem;
            managerB.AddExistingItem(entity);
            undoManager.Clear();

            managerA.DeleteEverywhereCommand.Execute(entity);
            Assert.True(undoManager.CanUndo);

            undoManager.Undo();

            // A single Undo call has to restore both references at once: the fan out was captured
            // as one composite command, not two independent ones.
            Assert.False(undoManager.CanUndo);
            Assert.Contains(entity, managerA.ManagerData.Items);
            Assert.Contains(entity, managerB.ManagerData.Items);
            Assert.Same(entity, repository.GetControl(entity.ID));
        }

        [Fact]
        public void ResetThenDropTheUndoEntry_TearsTheReplacedInstanceDown()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            var (composition, layer, entity) = CreateGraph(provider, compositionManager);

            composition.LayerManager.ResetItem(layer);
            var replacement = composition.LayerManager.ManagerData.Items[0];
            undoManager.Clear();

            Assert.Empty(layer.ModelEntityManager.ManagerData.Items);
            Assert.Null(repository.GetControl(entity.ID));
            Assert.Null(repository.GetControl(layer.ID));
            Assert.Same(replacement, composition.LayerManager.ManagerData.Items[0]);
            Assert.NotNull(repository.GetControl(replacement.ID));
        }
    }
}
