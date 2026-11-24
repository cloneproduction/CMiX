using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Names in the Blender style: Layer, Layer.001, Layer.002. New, numbered, typed, renamed and loaded names.
    public class UniqueNameTests
    {
        private static (ControlFactory factory, PrefabManager layers) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            layers.AddItem(typeof(Layer));
            return (provider.GetRequiredService<ControlFactory>(), layers);
        }

        [Fact]
        public void UniqueName_OfAUsedName_AddsTheFirstFreeNumber()
        {
            var (factory, _) = Create();

            Assert.Equal("Layer.001", factory.UniqueName("Layer"));
        }

        [Fact]
        public void UniqueName_OfANumberedName_DropsTheOldNumber()
        {
            var (factory, layers) = Create();
            layers.AddItem(typeof(Layer));

            Assert.Equal("Layer", ((Layer)layers.ManagerData.Items[0]).PrefabService.Name.Value);
            Assert.Equal("Layer.002", factory.UniqueName("Layer.001"));
        }

        [Fact]
        public void UniqueName_OfANameTheUserTyped_AddsANumber()
        {
            var (factory, _) = Create();

            Assert.Equal("Hero.001", factory.UniqueName("Hero"));
        }

        [Fact]
        public void UniqueName_AfterARename_SeesTheNewName()
        {
            var (factory, layers) = Create();
            ((Layer)layers.SelectedItem).PrefabService.Name.Value = "Hero.001";

            Assert.Equal("Hero.002", factory.UniqueName("Hero"));
        }

        [Fact]
        public void FromModel_OfTwoEntitiesWithOneName_KeepsBothNamesAndNamesTheNextOneUniquely()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var layer = (Layer)factory.Create(typeof(Layer));
            var layerModel = (LayerModel)layer.ToModel();

            var entityModelA = (EntityModel)factory.Create(typeof(Entity)).ToModel();
            var entityModelB = (EntityModel)factory.Create(typeof(Entity)).ToModel();

            // The app never writes two equal names, but nothing stops a hand made file, so the load must keep them.
            entityModelA.PrefabService = entityModelA.PrefabService with { Name = new GenericValueModel<string>("Entity") };
            entityModelB.PrefabService = entityModelB.PrefabService with { Name = new GenericValueModel<string>("Entity") };

            layerModel.ModelEntityManager.ManagerData.Items.Add(entityModelA);
            layerModel.ModelEntityManager.ManagerData.Items.Add(entityModelB);

            layer.FromModel(layerModel);

            var loadedItems = layer.ModelEntityManager.ManagerData.Items.ToList();
            Assert.Equal(2, loadedItems.Count);
            Assert.All(loadedItems, item => Assert.Equal("Entity", ((IPrefab)item).PrefabService.Name.Value));

            layer.ModelEntityManager.AddItem(typeof(Entity));
            var newEntity = (IPrefab)layer.ModelEntityManager.SelectedItem;

            Assert.Equal("Entity.001", newEntity.PrefabService.Name.Value);
            Assert.DoesNotContain(loadedItems, item => ReferenceEquals(item, newEntity));
        }

        // The factory caches the names. A rename must clear that cache, or the next control gets a duplicate.
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
        public void AddItem_AfterALoadFromAFile_GivesANameUniqueAgainstTheLoadedOnes()
        {
            using var temp = new TempDirectoryFixture();
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));

            var path = Path.Combine(temp.Path, "unique.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            targetProject.CompositionManager.AddItemFromModel(compositionModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            Assert.Equal("Layer", ((Layer)loadedComposition.LayerManager.ManagerData.Items[0]).PrefabService.Name.Value);

            loadedComposition.LayerManager.AddItem(typeof(Layer));
            var addedLayer = (Layer)loadedComposition.LayerManager.SelectedItem;

            Assert.Equal("Layer.001", addedLayer.PrefabService.Name.Value);
        }
    }
}
