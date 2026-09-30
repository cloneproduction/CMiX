using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Round trips a project through the file and back into a fresh object graph, which is the only
    // place where the loaded names, the loaded ids and the repository index meet.
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

        // MaterialSelector.FromModel sets its selected item directly and does not raise
        // PropertyChanged (by design, see PrefabSelector.cs), so an already-selected Material
        // cannot rely on that notification to learn its CompositionID after a project load. This
        // guards the setter-time stamp that covers it instead: Entity.CompositionID only gets
        // assigned once loading has finished, at which point MaterialSelector.FromModel has
        // already run, so the material it loaded is already there to stamp.
        [Fact]
        public void SaveThenLoad_StampsCompositionIDOnAnAlreadySelectedMaterial()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;

            entity.MaterialSelector.AddItemCommand.Execute(typeof(Material));

            var path = Path.Combine(_directory, "material.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            targetProject.CompositionManager.AddItem(compositionModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            var loadedLayer = (Layer)loadedComposition.LayerManager.ManagerData.Items[0];
            var loadedEntity = (Entity)loadedLayer.ModelEntityManager.ManagerData.Items[0];
            var loadedMaterial = (Material)loadedEntity.MaterialSelector.SelectedItem;

            Assert.Equal(loadedComposition.ID, loadedMaterial.CompositionID);
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
    }
}
