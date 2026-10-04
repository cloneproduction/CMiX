using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // Shared setup for the integration tests that need a project already saved to disk.
    internal static class ProjectFixtures
    {
        // Saves a project with a composition, a layer, an entity, and a material with a texture in each slot.
        public static void WriteProjectWithMaterialTextures(string path)
        {
            var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            var material = entity.Material;
            material.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            material.MaskTexture.TextureManager.AddItem(typeof(BubbleNoise));
            TestServiceProviderFactory.Pump();

            ProjectSerializer.Save(ProjectModelBuilder.Build(viewModel.Project), path);
        }

        // Saves compositions A to C, each with one layer, with the middle one selected.
        public static void WriteProjectWithThreeCompositions(string path)
        {
            var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var compositionManager = viewModel.Project.CompositionManager;

            foreach (var suffix in new[] { "A", "B", "C" })
            {
                compositionManager.AddItem(typeof(Composition));
                var composition = (Composition)compositionManager.SelectedItem;
                composition.PrefabService.Name.Value = "Composition" + suffix;
                composition.LayerManager.AddItem(typeof(Layer));
                var layer = (Layer)composition.LayerManager.SelectedItem;
                layer.PrefabService.Name.Value = "Layer" + suffix;
            }

            compositionManager.SelectedItem = compositionManager.ManagerData.Items[1];
            TestServiceProviderFactory.Pump();

            ProjectSerializer.Save(ProjectModelBuilder.Build(viewModel.Project), path);
        }

        // Saves a project with a name and a model; modelPath must be an existing file.
        public static void WriteProjectWithNameAndModel(string path, string projectName, string modelPath)
        {
            var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.Project.PrefabService.Name.Value = projectName;
            viewModel.Project.Model.SetAssetFromPath(modelPath);
            TestServiceProviderFactory.Pump();

            ProjectSerializer.Save(ProjectModelBuilder.Build(viewModel.Project), path);
        }

        // Saves two compositions on different output slots, neither of them slot 0.
        public static void WriteProjectWithDifferentOutputMappings(string path)
        {
            var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var compositionManager = viewModel.Project.CompositionManager;
            var slots = viewModel.Project.OutputMappingManager.Items;

            compositionManager.AddItem(typeof(Composition));
            ((Composition)compositionManager.SelectedItem).SelectedOutputMapping = slots[1];

            compositionManager.AddItem(typeof(Composition));
            ((Composition)compositionManager.SelectedItem).SelectedOutputMapping = slots[2];

            TestServiceProviderFactory.Pump();

            ProjectSerializer.Save(ProjectModelBuilder.Build(viewModel.Project), path);
        }
    }
}
