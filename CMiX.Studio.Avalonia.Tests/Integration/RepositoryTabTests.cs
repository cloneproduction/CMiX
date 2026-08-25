using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Views.Managers;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // Covers the two bindings the repository tabs depend on and that no compiler checks.
    // TabControl presents TabItem content through its own presenter, outside the TabItem
    // template, so that content does not inherit the tab DataContext on its own and
    // RepositoryTab rebinds it explicitly by ElementName. A regression there leaves the
    // repository manager on the window DataContext, where every manager binding silently
    // resolves to nothing. The second test covers the opposite idiom, the window relative
    // path the layer view uses to reach the same repository.
    public class RepositoryTabTests
    {
        [AvaloniaFact]
        public void TexturesTab_PresentsItsManagerAndTheEditingPanelOfTheSelectedTexture()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            var tabControl = TestServiceProviderFactory.MainTabControl(window);
            var texturesTab = tabControl.Items.OfType<RepositoryTab>()
                .Single(tab => ReferenceEquals(tab.DataContext, viewModel.TextureManager));

            tabControl.SelectedItem = texturesTab;
            Dispatcher.UIThread.RunJobs();

            // Only the selected tab realizes its content, so this is the textures manager.
            var repositoryManager = Assert.Single(window.GetVisualDescendants().OfType<RepositoryManager>());
            Assert.Same(viewModel.TextureManager, repositoryManager.DataContext);
            Assert.Same(viewModel.ControlRepository.Textures, repositoryManager.ItemsSource);

            viewModel.TextureManager.AddItem(typeof(CheckerBoard));
            var texture = Assert.Single(viewModel.TextureManager.ManagerData.Items);
            viewModel.TextureManager.SelectedItem = texture;
            TestServiceProviderFactory.Pump();

            Assert.Contains(viewModel.ControlRepository.Textures, item => ReferenceEquals(item, texture));

            // The editing panel is a logical child of the tab, so its bindings evaluate once
            // against the manager at load, which is where the startup accessor transients the
            // binding integrity test counts come from. Selecting an item must scope it to that
            // item, which is what makes those first evaluations transient rather than broken.
            var editingPanel = Assert.Single(window.GetVisualDescendants().OfType<Views.Texture>());
            Assert.Same(texture, editingPanel.DataContext);
        }

        [AvaloniaFact]
        public void LayerView_ReachesTheRepositoryThroughTheWindowDataContext()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(CMiX.Core.Compositing.Composition));
            var composition = (CMiX.Core.Compositing.Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(CMiX.Core.Compositing.Layer));

            TestServiceProviderFactory.MainTabControl(window).SelectedIndex = 1;
            TestServiceProviderFactory.Pump();

            var layerView = Assert.Single(window.GetVisualDescendants().OfType<Views.Layer>());
            Assert.True(
                layerView.GetVisualDescendants().OfType<PrefabSlotManager>()
                    .Any(slotManager => ReferenceEquals(slotManager.ItemsSource, viewModel.ControlRepository.EntitiesAndTexts)),
                "The layer entity slots did not resolve the repository through the window DataContext.");
        }
    }
}
