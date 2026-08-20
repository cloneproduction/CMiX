using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Covers what File > New has to empty. The menu command sweeps the seven top level repository
    // managers and the composition manager, which only reaches a control that one of those still
    // references; everything else has to be released by the teardown of its owner. A control that
    // owns managers of its own, a material with its two texture slots for instance, therefore kept
    // its contents registered in the repository after a new project, and since the textures tab
    // lists the whole repository rather than one manager's items, those textures stayed on screen.
    public class NewProjectTests
    {
        private static (Views.MainWindow window, MainViewModel viewModel) ShowMainWindow()
        {
            var provider = TestServiceProviderFactory.Create();
            var window = TestServiceProviderFactory.CreateMainWindow(provider);
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, provider.GetRequiredService<MainViewModel>());
        }

        private static void Pump()
        {
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.RunJobs();
        }

        private static RepositoryTab SelectTab(Views.MainWindow window, string toolTipText)
        {
            var tabControl = window.GetVisualDescendants().OfType<TabControl>().First();
            var tab = tabControl.Items.OfType<RepositoryTab>().Single(t => t.ToolTipText == toolTipText);
            tabControl.SelectedItem = tab;
            Pump();
            return tab;
        }

        // The type picker buttons live in a popup, which is hosted next to the window content rather
        // than inside the panel that opened it, so they are matched by the manager they bound to
        // instead of by their position in the tree. That match is also the assertion that the button
        // reaches the intended manager and not some other instance of the transient PrefabManager.
        private static void CreateThroughPopup(Views.MainWindow window, PrefabManager manager, string caption, Type prefabType)
        {
            var toggle = window.GetVisualDescendants().OfType<ToggleButton>()
                .Single(t => (t.Content as string) == caption && ReferenceEquals(t.DataContext, manager));
            toggle.IsChecked = true;
            Pump();

            var button = window.GetVisualDescendants().OfType<Button>()
                .Single(b => (b.CommandParameter as Type) == prefabType && ReferenceEquals(b.DataContext, manager));
            button.Command!.Execute(button.CommandParameter);
            Pump();
        }

        [AvaloniaFact]
        public void NewProject_ClearsATextureCreatedThroughTheTexturesTab()
        {
            var (window, viewModel) = ShowMainWindow();
            SelectTab(window, "Textures");

            CreateThroughPopup(window, viewModel.TextureManager, "New Texture", typeof(CheckerBoard));

            var texture = Assert.Single(viewModel.TextureManager.ManagerData.Items);
            Assert.Contains(viewModel.ControlRepository.Textures, item => ReferenceEquals(item, texture));

            viewModel.MainMenu.NewProjectCommand.Execute(null);
            Pump();

            Assert.Empty(viewModel.TextureManager.ManagerData.Items);
            Assert.Empty(viewModel.ControlRepository.Textures);
        }

        [AvaloniaFact]
        public void NewProject_ClearsATextureHeldByAMaterialTextureSlot()
        {
            var (window, viewModel) = ShowMainWindow();
            SelectTab(window, "Material");

            var newMaterialButton = window.GetVisualDescendants().OfType<Button>()
                .Single(b => (b.Content as string) == "New Material");
            newMaterialButton.Command!.Execute(newMaterialButton.CommandParameter);
            Pump();

            var material = Assert.IsType<Material>(Assert.Single(viewModel.MaterialManager.ManagerData.Items));
            // Selecting the material realizes its editing panel, and expanding the texture section
            // realizes the diffuse slot inside it, the same two steps the user takes.
            viewModel.MaterialManager.SelectedItem = material;
            material.DiffuseTexture.IsExpanded = true;
            Pump();

            CreateThroughPopup(window, material.DiffuseTexture.TextureManager, "Add Texture", typeof(CheckerBoard));

            var texture = Assert.Single(material.DiffuseTexture.TextureManager.ManagerData.Items);
            Assert.Contains(viewModel.ControlRepository.Textures, item => ReferenceEquals(item, texture));
            // The slot owns it alone; the top level texture manager never saw it.
            Assert.Empty(viewModel.TextureManager.ManagerData.Items);

            viewModel.MainMenu.NewProjectCommand.Execute(null);
            Pump();

            Assert.Empty(viewModel.ControlRepository.Materials);
            Assert.Empty(viewModel.ControlRepository.Textures);
        }

        // The textures of a loaded project are reachable only through the composition graph, so this
        // is the case the reported symptom came from: the material sits in the entity's material
        // selector rather than in any of the swept managers, and both it and its slot textures have
        // to come down with the composition.
        [AvaloniaFact]
        public void NewProject_ClearsTheTexturesOfALoadedProject()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-newproject-{Guid.NewGuid():N}.cmix");
            try
            {
                WriteProjectWithMaterialTextures(path);

                var (_, viewModel) = ShowMainWindow();

                // The steps MainMenu.OpenProject takes once the file dialog has returned a path.
                var projectModel = ProjectSerializer.Load(path);
                var compositionModel = (CompositionModel)projectModel.CompositionManager.ManagerData.Items.First();
                viewModel.Project.CompositionManager.AddItem(compositionModel);
                Pump();

                Assert.Equal(2, viewModel.ControlRepository.Textures.Count);
                Assert.Single(viewModel.ControlRepository.Materials);

                viewModel.MainMenu.NewProjectCommand.Execute(null);
                Pump();

                Assert.Empty(viewModel.ControlRepository.Compositions);
                Assert.Empty(viewModel.ControlRepository.Materials);
                Assert.Empty(viewModel.ControlRepository.Textures);
                Assert.Empty(viewModel.ControlRepository.Controls);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // Authors the fixture through the same managers the app uses, so it round trips the real
        // model shape: a composition holding a layer, holding an entity, whose material selector
        // holds a material with a texture in each of its two slots.
        private static void WriteProjectWithMaterialTextures(string path)
        {
            var (_, viewModel) = ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            entity.MaterialSelector.AddItemCommand.Execute(typeof(Material));
            var material = (Material)entity.MaterialSelector.SelectedItem;
            material.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            material.MaskTexture.TextureManager.AddItem(typeof(BubbleNoise));
            Pump();

            ProjectSerializer.Save(ProjectModelBuilder.Build(composition, viewModel.Project.MasterBeat, viewModel.Project.OutputMappingManager), path);
        }
    }
}
