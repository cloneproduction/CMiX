using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Covers the deferred disposal of a deleted control: the delete path no longer disposes, the
    // undo command that holds the removed instance does, and only once it is dropped.
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
    }
}
