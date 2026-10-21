using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The cards stand out from the surface behind them. That surface is the color of the main window.
    // The containers of the panels are transparent, so this one color decides.
    public class MainWindowSurfaceTests
    {
        private static int Brightness(IBrush? brush)
        {
            var color = ((ISolidColorBrush)brush!).Color;
            return color.R + color.G + color.B;
        }

        [AvaloniaFact]
        public void TheWindowSurface_IsDarkerThanTheCards()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var cards = Brightness((IBrush)Application.Current!.FindResource("PanelBackgroundLevel1Brush")!);

            Assert.True(Brightness(window.Background) < cards, "The window is not darker than the cards.");

            var mainPanel = window.GetVisualDescendants().OfType<DockPanel>().First(d => d.Background != null);
            Assert.True(Brightness(mainPanel.Background) < cards, "The main panel is not darker than the cards.");
        }

        // Where the card of a ModelEntity is shown: the Entities tab, and the entity tab of a layer.
        [AvaloniaTheory]
        [InlineData("MainWindowEntitiesTab")]
        [InlineData("MainWindowLayersTab")]
        public void NothingBehindTheCards_IsLighterThanTheWindow(string tabName)
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.EntityManager.AddItem(typeof(Entity));
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            ((Layer)composition.LayerManager.SelectedItem).ModelEntityManager.AddItem(typeof(Entity));
            var tabs = TestServiceProviderFactory.MainTabControl(window);
            tabs.SelectedItem = tabs.Items.OfType<TabItem>().First(t => t.Content?.GetType().Name == tabName);
            TestServiceProviderFactory.Pump();

            var card = window.GetVisualDescendants().OfType<ExpanderIndented>().First();
            var window0 = Brightness(window.Background);

            foreach (var ancestor in card.GetVisualAncestors())
            {
                var brush = ancestor switch
                {
                    Border border => border.Background,
                    Panel panel => panel.Background,
                    ContentPresenter presenter => presenter.Background,
                    _ => null,
                } as ISolidColorBrush;

                if (brush != null && brush.Color.A > 0)
                    Assert.True(Brightness(brush) <= window0, $"{ancestor.GetType().Name} {(ancestor as Control)?.Name} paints a color lighter than the window behind the cards.");
            }
        }

        // The tabs of a layer (entity, camera, light, ...) sit on a strip lighter than the window.
        // The strip runs down the whole height, also under the last tab.
        [AvaloniaFact]
        public void TheTabStripOfALayer_IsLighterThanTheWindow_AndFillsTheHeight()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var mainTabs = TestServiceProviderFactory.MainTabControl(window);
            mainTabs.SelectedItem = mainTabs.Items.OfType<TabItem>().First(t => t.Content?.GetType().Name == "MainWindowLayersTab");
            TestServiceProviderFactory.Pump();

            var layerTabs = window.GetVisualDescendants().OfType<Views.Layer>().First().GetVisualDescendants().OfType<TabControl>().First();
            var strip = layerTabs.GetVisualDescendants().OfType<Border>().First(b => b.Child is ItemsPresenter);

            Assert.True(Brightness(strip.Background) > Brightness(window.Background), "The tab strip is not lighter than the window.");
            Assert.Equal(layerTabs.Bounds.Height, strip.Bounds.Height);
        }

        // Every tab sits on the surface of the main window. The assets tab and the network tab had a lighter surface of their own.
        [AvaloniaTheory]
        [InlineData("Resources", "AssetManager")]
        [InlineData("Network", "SyncPanel")]
        public void TheAssetsTabAndTheNetworkTab_SitOnTheSurfaceOfTheWindow(string icon, string viewName)
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var tabs = TestServiceProviderFactory.MainTabControl(window);
            tabs.SelectedItem = tabs.Items.OfType<TabItem>().First(tab => tab.Header is CMiX.UI.Controls.AppIcon appIcon && appIcon.IconKey == icon);
            TestServiceProviderFactory.Pump();

            var view = window.GetVisualDescendants().OfType<UserControl>().First(control => control.GetType().Name == viewName);
            var surfaces = new List<IBrush?> { view.Background };
            surfaces.AddRange(view.GetVisualDescendants().OfType<Border>().Where(border => border.Name == "dropBorder").Select(border => border.Background));
            var windowBrightness = Brightness(window.Background);

            foreach (var brush in surfaces.OfType<ISolidColorBrush>().Where(brush => brush.Color.A > 0))
                Assert.True(Brightness(brush) <= windowBrightness, $"{viewName} paints a color lighter than the window.");
        }
    }
}
