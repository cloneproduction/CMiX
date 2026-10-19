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
    public class CollectionManagerHoldingTests
    {
        [Fact]
        public void ARemovedControl_IsNoLongerHeldByThatCollectionManager()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var manager = provider.GetRequiredService<PrefabManager>();

            manager.AddItem(typeof(CheckerBoard));
            var texture = manager.ManagerData.Items[0];

            Assert.Same(manager.Collection, Assert.Single(repository.CollectionManagersHolding(texture)));

            manager.DeleteItem(texture);

            Assert.Empty(repository.CollectionManagersHolding(texture));
        }

        [Fact]
        public void ADisposedManager_ReleasesEveryControlItHeld()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(CheckerBoard));
            manager.AddItem(typeof(CheckerBoard));
            var items = manager.ManagerData.Items.ToList();

            manager.Dispose();

            Assert.All(items, item => Assert.Empty(repository.CollectionManagersHolding(item)));
            Assert.Equal(0, RepositoryHolding.CollectionManagerCount(repository));
        }

        [Fact]
        public void DeletingLayersRepeatedly_DoesNotGrowTheCollectionManagersTheRepositoryHolds()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            AddAndDropALayer(composition, undoManager);
            var baseline = RepositoryHolding.CollectionManagerCount(repository);

            for (int i = 0; i < 5; i++)
                AddAndDropALayer(composition, undoManager);

            Assert.Equal(baseline, RepositoryHolding.CollectionManagerCount(repository));
        }

        [Fact]
        public void DroppingTheUndoEntryOfADeletedLayer_ReleasesItsNestedManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            var beforeTheLayer = RepositoryHolding.CollectionManagerCount(repository);
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));

            Assert.True(RepositoryHolding.CollectionManagerCount(repository) > beforeTheLayer);

            composition.LayerManager.DeleteItem(layer);

            Assert.True(RepositoryHolding.CollectionManagerCount(repository) > beforeTheLayer);

            undoManager.Clear();

            Assert.Equal(beforeTheLayer, RepositoryHolding.CollectionManagerCount(repository));
        }

        [Fact]
        public void DroppingATexture_TakesItsFilterModifiersWithIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var textureManager = provider.GetRequiredService<PrefabManager>();

            var beforeTheTexture = RepositoryHolding.CollectionManagerCount(repository);

            textureManager.AddItem(typeof(CheckerBoard));
            var texture = (CheckerBoard)textureManager.SelectedItem;
            texture.TextureModifierManager.AddItem(typeof(Blur));
            var filter = (Blur)texture.TextureModifierManager.SelectedItem;

            Assert.Same(filter, repository.GetControl(filter.ID));
            Assert.True(RepositoryHolding.CollectionManagerCount(repository) > beforeTheTexture);

            textureManager.DeleteItem(texture);

            Assert.Same(filter, repository.GetControl(filter.ID));

            undoManager.Clear();

            Assert.Null(repository.GetControl(texture.ID));
            Assert.Null(repository.GetControl(filter.ID));
            Assert.Equal(beforeTheTexture, RepositoryHolding.CollectionManagerCount(repository));
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
