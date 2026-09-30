using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // Shared setup for the integration tests that need a project already saved to disk.
    internal static class ProjectFixtures
    {
        // Builds a project through the same managers the app uses, so it round trips the real
        // model shape, then saves it to disk: a composition holding a layer, holding an entity,
        // whose material selector holds a material with a texture in each of its two slots. Spins
        // up its own throwaway window purely to construct the real DI wired object graph the save
        // needs; that window is not the one the calling test goes on to exercise.
        public static void WriteProjectWithMaterialTextures(string path)
        {
            var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            entity.MaterialSelector.AddItemCommand.Execute(typeof(CMiX.Core.Materials.Material));
            var material = (CMiX.Core.Materials.Material)entity.MaterialSelector.SelectedItem;
            material.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            material.MaskTexture.TextureManager.AddItem(typeof(BubbleNoise));
            TestServiceProviderFactory.Pump();

            ProjectSerializer.Save(ProjectModelBuilder.Build(viewModel.Project), path);
        }
    }
}
