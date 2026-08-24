using CMiX.Core.Compositing;
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
    }
}
