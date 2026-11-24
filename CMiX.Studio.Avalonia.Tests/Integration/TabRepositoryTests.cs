using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Managers;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // Covers the two bindings the repository tabs depend on and that no compiler checks.
    // TabControl presents TabItem content through its own presenter, outside the TabItem
    // template, so that content does not inherit the tab DataContext on its own and
    // TabRepository rebinds it explicitly by ElementName. A regression there leaves the
    // repository manager on the window DataContext, where every manager binding silently
    // resolves to nothing. The second test covers the layer view, which reaches the same
    // repository through the manager of each slot.
    public class TabRepositoryTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        [AvaloniaFact]
        public void Show_TheTexturesTab_PresentsItsManagerAndThePanelOfTheSelectedTexture()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();

            var tabControl = TestServiceProviderFactory.MainTabControl(window);
            var texturesTab = tabControl.Items.OfType<TabRepository>()
                .Single(tab => ReferenceEquals(tab.DataContext, viewModel.TextureManager));

            tabControl.SelectedItem = texturesTab;
            Dispatcher.UIThread.RunJobs();

            // Only the selected tab realizes its content, so this is the textures manager.
            var repositoryManager = Assert.Single(window.GetVisualDescendants().OfType<ListRepository>());
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
        public void TheDeleteButton_HasTheRemoveEverywhereCommandOfTheManager_AndTheSelectedTexture()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            var tabControl = TestServiceProviderFactory.MainTabControl(window);
            tabControl.SelectedItem = tabControl.Items.OfType<TabRepository>()
                .Single(tab => ReferenceEquals(tab.DataContext, viewModel.TextureManager));
            Dispatcher.UIThread.RunJobs();
            viewModel.TextureManager.AddItem(typeof(CheckerBoard));
            var texture = Assert.Single(viewModel.TextureManager.ManagerData.Items);
            viewModel.TextureManager.SelectedItem = texture;
            TestServiceProviderFactory.Pump();

            var repositoryManager = Assert.Single(window.GetVisualDescendants().OfType<ListRepository>());
            var delete = repositoryManager.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "removeButton");

            Assert.Same(viewModel.TextureManager.RemoveEverywhereCommand, delete.Command);
            Assert.Same(texture, delete.CommandParameter);
        }

        [AvaloniaFact]
        public void Show_TheLayerView_BindsTheEntitySlotsToTheModelEntitiesOfTheRepository()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(CMiX.Core.Compositing.Composition));
            var composition = (CMiX.Core.Compositing.Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(CMiX.Core.Compositing.Layer));

            var tabControl = TestServiceProviderFactory.MainTabControl(window);
            var layersTab = tabControl.Items.OfType<TabItem>()
                .Single(tab => tab.Content is Views.TabLayers);
            tabControl.SelectedItem = layersTab;
            TestServiceProviderFactory.Pump();

            var layerView = Assert.Single(window.GetVisualDescendants().OfType<Views.Layer>());
            Assert.True(
                layerView.GetVisualDescendants().OfType<ListPrefabSlot>()
                    .Any(slotManager => ReferenceEquals(slotManager.ItemsSource, viewModel.ControlRepository.ModelEntities)),
                "The layer entity slots did not resolve the model entities of the repository.");
        }
    }
}
