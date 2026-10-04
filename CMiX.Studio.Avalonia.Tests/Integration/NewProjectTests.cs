using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // Covers what File > New has to empty. The menu command sweeps the seven top level repository
    // managers and the composition manager, which only reaches a control that one of those still
    // references; everything else has to be released by the teardown of its owner. A control that
    // owns managers of its own, a material with its two texture slots for instance, therefore kept
    // its contents registered in the repository after a new project, and since the textures tab
    // lists the whole repository rather than one manager's items, those textures stayed on screen.
    public class NewProjectTests
    {
        private static RepositoryTab SelectTab(Views.MainWindow window, string toolTipText)
        {
            var tabControl = TestServiceProviderFactory.MainTabControl(window);
            var tab = tabControl.Items.OfType<RepositoryTab>().Single(t => t.ToolTipText == toolTipText);
            tabControl.SelectedItem = tab;
            TestServiceProviderFactory.Pump();
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
            TestServiceProviderFactory.Pump();

            var button = window.GetVisualDescendants().OfType<Button>()
                .Single(b => (b.CommandParameter as Type) == prefabType && ReferenceEquals(b.DataContext, manager));
            button.Command!.Execute(button.CommandParameter);
            TestServiceProviderFactory.Pump();
        }

        [AvaloniaFact]
        public void NewProject_ClearsATextureCreatedThroughTheTexturesTab()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            SelectTab(window, "Textures");

            CreateThroughPopup(window, viewModel.TextureManager, "New Texture", typeof(CheckerBoard));

            var texture = Assert.Single(viewModel.TextureManager.ManagerData.Items);
            Assert.Contains(viewModel.ControlRepository.Textures, item => ReferenceEquals(item, texture));

            viewModel.MainMenu.NewProjectCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.Empty(viewModel.TextureManager.ManagerData.Items);
            Assert.Empty(viewModel.ControlRepository.Textures);
        }

        // The textures of a loaded project are reachable only through the composition graph, so this
        // is the case the reported symptom came from: the slot textures sit in the entity's material
        // rather than in any of the swept managers, and they have to come down with the composition.
        [AvaloniaFact]
        public void NewProject_ClearsTheTexturesOfALoadedProject()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-newproject-{Guid.NewGuid():N}.cmix");
            try
            {
                ProjectFixtures.WriteProjectWithMaterialTextures(path);

                var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

                // The steps MainMenu.OpenProject takes once the file dialog has returned a path.
                var projectModel = ProjectSerializer.Load(path);
                var compositionModel = (CompositionModel)projectModel.CompositionManager.ManagerData.Items.First();
                viewModel.Project.CompositionManager.AddItem(compositionModel);
                TestServiceProviderFactory.Pump();

                Assert.Equal(2, viewModel.ControlRepository.Textures.Count);

                viewModel.MainMenu.NewProjectCommand.Execute(null);
                TestServiceProviderFactory.Pump();

                Assert.Empty(viewModel.ControlRepository.Compositions);
                Assert.Empty(viewModel.ControlRepository.Textures);
                Assert.Empty(viewModel.ControlRepository.Controls);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
