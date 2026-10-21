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
    public class ControlRepositoryTests
    {
        [Fact]
        public void GetControl_ReturnsTheRegisteredInstance_AndNullForUnknownIds()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var first = factory.Create(typeof(CheckerBoard));
            var second = factory.Create(typeof(CheckerBoard));
            repository.AddControl(first, collectionManager);
            repository.AddControl(second, collectionManager);

            Assert.Same(first, repository.GetControl(first.ID));
            Assert.Same(second, repository.GetControl(second.ID));
            Assert.Null(repository.GetControl(Guid.NewGuid()));

            repository.RemoveControl(first, collectionManager);

            Assert.Null(repository.GetControl(first.ID));
            Assert.Same(second, repository.GetControl(second.ID));
            Assert.DoesNotContain(first, repository.Controls);
        }

        [Fact]
        public void AddControl_IgnoresASecondControlWithAnAlreadyRegisteredId()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var first = factory.Create(typeof(CheckerBoard));
            var second = factory.Create(typeof(CheckerBoard));
            second.ID = first.ID;

            repository.AddControl(first, collectionManager);
            repository.AddControl(second, collectionManager);

            Assert.Single(repository.Controls);
            Assert.Same(first, repository.GetControl(first.ID));
        }

        // The factory caches the names it has to stay unique against, so a rename made after a
        // control was created has to invalidate that cache or the next control gets a duplicate.
        [Fact]
        public void NameControl_StaysUnique_AfterAControlWasRenamed()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var first = (IPrefab)factory.Create(typeof(CheckerBoard));
            repository.AddControl((IControl)first, collectionManager);
            var second = (IPrefab)factory.Create(typeof(CheckerBoard));
            repository.AddControl((IControl)second, collectionManager);

            Assert.Equal("Checker Board", first.PrefabService.Name.Value);
            Assert.Equal("Checker Board.001", second.PrefabService.Name.Value);

            second.PrefabService.Name.Value = "Checker Board.002";

            var third = (IPrefab)factory.Create(typeof(CheckerBoard));
            repository.AddControl((IControl)third, collectionManager);

            Assert.Equal("Checker Board.001", third.PrefabService.Name.Value);
        }

        [Fact]
        public void ModelEntities_HoldsEntitiesAndTextsInTheOrderTheyWereAdded()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var entity1 = factory.Create(typeof(Entity));
            var text1 = factory.Create(typeof(TextEntity));
            var entity2 = factory.Create(typeof(Entity));
            var text2 = factory.Create(typeof(TextEntity));
            var entity3 = factory.Create(typeof(Entity));

            repository.AddControl(entity1, collectionManager);
            repository.AddControl(text1, collectionManager);
            repository.AddControl(entity2, collectionManager);
            repository.AddControl(text2, collectionManager);

            Assert.Equal(new[] { entity1, text1, entity2, text2 }, repository.ModelEntities);

            repository.RemoveControl(entity1, collectionManager);
            repository.RemoveControl(text1, collectionManager);

            Assert.Equal(new[] { entity2, text2 }, repository.ModelEntities);

            repository.AddControl(entity3, collectionManager);

            Assert.Equal(new[] { entity2, text2, entity3 }, repository.ModelEntities);
        }

        [Fact]
        public void ModelEntities_EmptiesWhenControlsIsCleared()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var entity = factory.Create(typeof(Entity));
            var text = factory.Create(typeof(TextEntity));
            repository.AddControl(entity, collectionManager);
            repository.AddControl(text, collectionManager);

            Assert.Equal(new[] { entity, text }, repository.ModelEntities);

            repository.Controls.Clear();

            Assert.Empty(repository.ModelEntities);
            Assert.Empty(repository.Entities);
            Assert.Empty(repository.Texts);
        }

        [Fact]
        public void ARemovedControl_IsNoLongerHeldByThatCollectionManager()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var manager = provider.GetRequiredService<PrefabManager>();

            manager.AddItem(typeof(CheckerBoard));
            var texture = manager.ManagerData.Items[0];

            Assert.Same(manager.Collection, Assert.Single(repository.GetCollectionManagers(texture)));

            manager.RemoveItem(texture);

            Assert.Empty(repository.GetCollectionManagers(texture));
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

            Assert.All(items, item => Assert.Empty(repository.GetCollectionManagers(item)));
            Assert.Equal(0, HeldCollectionManagers.Count(repository));
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
            var baseline = HeldCollectionManagers.Count(repository);

            for (int i = 0; i < 5; i++)
                AddAndDropALayer(composition, undoManager);

            Assert.Equal(baseline, HeldCollectionManagers.Count(repository));
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

            var beforeTheLayer = HeldCollectionManagers.Count(repository);
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));

            Assert.True(HeldCollectionManagers.Count(repository) > beforeTheLayer);

            composition.LayerManager.RemoveItem(layer);

            Assert.True(HeldCollectionManagers.Count(repository) > beforeTheLayer);

            undoManager.Clear();

            Assert.Equal(beforeTheLayer, HeldCollectionManagers.Count(repository));
        }

        [Fact]
        public void DroppingATexture_TakesItsFilterModifiersWithIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var textureManager = provider.GetRequiredService<PrefabManager>();

            var beforeTheTexture = HeldCollectionManagers.Count(repository);

            textureManager.AddItem(typeof(CheckerBoard));
            var texture = (CheckerBoard)textureManager.SelectedItem;
            texture.TextureModifierManager.AddItem(typeof(Blur));
            var filter = (Blur)texture.TextureModifierManager.SelectedItem;

            Assert.Same(filter, repository.GetControl(filter.ID));
            Assert.True(HeldCollectionManagers.Count(repository) > beforeTheTexture);

            textureManager.RemoveItem(texture);

            Assert.Same(filter, repository.GetControl(filter.ID));

            undoManager.Clear();

            Assert.Null(repository.GetControl(texture.ID));
            Assert.Null(repository.GetControl(filter.ID));
            Assert.Equal(beforeTheTexture, HeldCollectionManagers.Count(repository));
        }

        private static void AddAndDropALayer(Composition composition, UndoManager undoManager)
        {
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            composition.LayerManager.RemoveItem(layer);
            undoManager.Clear();
        }
    }
}
