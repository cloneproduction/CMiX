using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Round trips a project through a file into a fresh object graph.
    public class ProjectLoadTests : IDisposable
    {
        private readonly TempDirectoryFixture _tempDir = new();
        private string _directory => _tempDir.Path;

        public void Dispose() => _tempDir.Dispose();

        [Fact]
        public void SaveThenLoad_KeepsPrefabNamesAndResolvesEveryControl()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;
            composition.PrefabService.Name.Value = "Opening Scene";

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.PrefabService.Name.Value = "Background";

            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            entity.PrefabService.Name.Value = "Hero";

            var path = Path.Combine(_directory, "names.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            var repository = target.GetRequiredService<ControlRepository>();

            targetProject.CompositionManager.AddItem(compositionModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            var loadedLayer = (Layer)loadedComposition.LayerManager.ManagerData.Items[0];
            var loadedEntity = (Entity)loadedLayer.ModelEntityManager.ManagerData.Items[0];

            Assert.Equal("Opening Scene", loadedComposition.PrefabService.Name.Value);
            Assert.Equal("Background", loadedLayer.PrefabService.Name.Value);
            Assert.Equal("Hero", loadedEntity.PrefabService.Name.Value);

            Assert.Equal(composition.ID, loadedComposition.ID);
            Assert.Equal(layer.ID, loadedLayer.ID);
            Assert.Equal(entity.ID, loadedEntity.ID);

            Assert.Same(loadedComposition, repository.GetControl(loadedComposition.ID));
            Assert.Same(loadedLayer, repository.GetControl(loadedLayer.ID));
            Assert.Same(loadedEntity, repository.GetControl(loadedEntity.ID));
        }

        [Fact]
        public void SaveThenLoad_KeepsNewNamesUniqueAgainstTheLoadedOnes()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));

            var path = Path.Combine(_directory, "unique.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            targetProject.CompositionManager.AddItem(compositionModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            Assert.Equal("Layer", ((Layer)loadedComposition.LayerManager.ManagerData.Items[0]).PrefabService.Name.Value);

            loadedComposition.LayerManager.AddItem(typeof(Layer));
            var addedLayer = (Layer)loadedComposition.LayerManager.SelectedItem;

            Assert.Equal("Layer.001", addedLayer.PrefabService.Name.Value);
        }

        [Fact]
        public void SaveThenLoad_KeepsAValidOutputSlotSelection()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;
            var slot = sourceProject.OutputMappingManager.Items[3];
            composition.SelectedOutputMapping = slot;

            var path = Path.Combine(_directory, "output-slot.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            targetProject.CompositionManager.AddItem(compositionModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            Assert.Equal(slot.ID, loadedComposition.SelectedOutputMappingID.Value);
            Assert.Same(targetProject.OutputMappingManager.Items[3], loadedComposition.SelectedOutputMapping);
        }

        [Fact]
        public void SaveThenLoad_WithAnUnknownOutputSlot_KeepsTheDefaultSlot()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;
            composition.SelectedOutputMapping = sourceProject.OutputMappingManager.Items[3];

            var path = Path.Combine(_directory, "unknown-output-slot.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            // Duplicate and open replace every GUID, so the saved slot ID names no slot.
            var replacedModel = compositionModel with
            {
                SelectedOutputMappingID = compositionModel.SelectedOutputMappingID with { Value = Guid.NewGuid() }
            };

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            targetProject.CompositionManager.AddItem(replacedModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            var defaultSlot = targetProject.OutputMappingManager.Items[0];
            Assert.Equal(defaultSlot.ID, loadedComposition.SelectedOutputMappingID.Value);
            Assert.Same(defaultSlot, loadedComposition.SelectedOutputMapping);
        }

        [Fact]
        public void SaveThenLoad_WithTwoCompositions_KeepsBothAndTheirLayersSeparate()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var compositionA = (Composition)sourceProject.CompositionManager.SelectedItem;
            compositionA.PrefabService.Name.Value = "CompositionA";
            compositionA.LayerManager.AddItem(typeof(Layer));
            var layerA = (Layer)compositionA.LayerManager.SelectedItem;
            layerA.PrefabService.Name.Value = "LayerA";

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var compositionB = (Composition)sourceProject.CompositionManager.SelectedItem;
            compositionB.PrefabService.Name.Value = "CompositionB";
            compositionB.LayerManager.AddItem(typeof(Layer));
            var layerB = (Layer)compositionB.LayerManager.SelectedItem;
            layerB.PrefabService.Name.Value = "LayerB";

            var path = Path.Combine(_directory, "multi-composition.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModels = loadedModel.CompositionManager.ManagerData.Items;

            Assert.Equal(2, compositionModels.Count);

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            foreach (var compositionModel in compositionModels)
                targetProject.CompositionManager.AddItem(compositionModel);

            var loadedCompositions = targetProject.CompositionManager.ManagerData.Items;
            Assert.Equal(2, loadedCompositions.Count);

            var loadedCompositionA = (Composition)loadedCompositions.Single(c => ((Composition)c).PrefabService.Name.Value == "CompositionA");
            var loadedCompositionB = (Composition)loadedCompositions.Single(c => ((Composition)c).PrefabService.Name.Value == "CompositionB");

            Assert.NotEqual(Guid.Empty, loadedCompositionA.ID);
            Assert.NotEqual(Guid.Empty, loadedCompositionB.ID);
            Assert.NotEqual(loadedCompositionA.ID, loadedCompositionB.ID);
            Assert.Equal(compositionA.ID, loadedCompositionA.ID);
            Assert.Equal(compositionB.ID, loadedCompositionB.ID);

            var loadedLayerA = (Layer)Assert.Single(loadedCompositionA.LayerManager.ManagerData.Items);
            var loadedLayerB = (Layer)Assert.Single(loadedCompositionB.LayerManager.ManagerData.Items);

            Assert.Equal("LayerA", loadedLayerA.PrefabService.Name.Value);
            Assert.Equal("LayerB", loadedLayerB.PrefabService.Name.Value);
        }
    }
}
