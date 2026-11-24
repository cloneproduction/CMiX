// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using CMiX.UI.Panels;
using Avalonia.Controls.Primitives;
using CMiX.Studio.Avalonia.Notices;
using CMiX.UI.Popups;
using Microsoft.Extensions.DependencyInjection;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tree of the project stands in its own column, left of the tabs, and is always visible.
    // The tree has the whole height of that column. The master beat is under the tabs only.
    // The tree and the tabs have no gap between them. The splitter between them lies over their edge, takes no room,
    // and shows the same bar as the splitters of the rows (GridSplitterCMiX).
    // The cards stand out from the surface behind them. That surface is the color of the main window.
    // The open window takes the panel keys and shows the notifications.
    public class MainWindowLayoutTests : IDisposable
    {
        [AvaloniaFact]
        public void Show_TheTree_StandsLeftOfTheTabsAtTheirTop()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            var tree = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var tabs = TestServiceProviderFactory.MainTabControl(window);

            Assert.Same(viewModel.ProjectTree, tree.DataContext);
            Assert.True(tree.IsVisible);
            Assert.True(tree.Bounds.Width > 0);

            var treeLeft = tree.TranslatePoint(new(0, 0), window)!.Value;
            var tabsLeft = tabs.TranslatePoint(new(0, 0), window)!.Value;
            Assert.True(treeLeft.X + tree.Bounds.Width <= tabsLeft.X, "The tree is not left of the tabs.");
            Assert.Equal(tabsLeft.Y, treeLeft.Y);
        }

        [AvaloniaFact]
        public void Show_TheTreeAndTheTabs_HaveTheSameWidth()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var tree = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var columns = tree.GetVisualAncestors().OfType<Grid>().First(grid => grid.ColumnDefinitions.Count == 2).ColumnDefinitions;

            Assert.True(columns[0].ActualWidth > 0);
            Assert.Equal(columns[0].ActualWidth, columns[1].ActualWidth);
        }

        [AvaloniaFact]
        public void Show_TheMasterBeat_IsUnderTheTabsOnly()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var tree = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var tabs = TestServiceProviderFactory.MainTabControl(window);
            var beat = window.GetVisualDescendants().OfType<PanelMasterBeat>().Single();

            var tabsOrigin = tabs.TranslatePoint(new(0, 0), window)!.Value;
            var beatOrigin = beat.TranslatePoint(new(0, 0), window)!.Value;
            var treeOrigin = tree.TranslatePoint(new(0, 0), window)!.Value;

            // The beat is in the right column: it starts and ends inside the width of the tabs.
            Assert.True(beatOrigin.X >= tabsOrigin.X, "The master beat starts left of the tabs.");
            Assert.True(beatOrigin.X + beat.Bounds.Width <= tabsOrigin.X + tabs.Bounds.Width, "The master beat ends right of the tabs.");

            // It is under the tabs, and the tabs end where it starts.
            Assert.True(beatOrigin.Y >= tabsOrigin.Y + tabs.Bounds.Height - 0.5, "The master beat is not under the tabs.");

            // The tree goes down to the bottom of the box of the beat (the box has a padding of 8 around the beat).
            var beatBox = beat.GetVisualAncestors().OfType<Border>().First();
            var beatBoxBottom = beatBox.TranslatePoint(new(0, beatBox.Bounds.Height), window)!.Value.Y;
            Assert.Equal(beatBoxBottom, treeOrigin.Y + tree.Bounds.Height, 0.5);
            Assert.True(tree.Bounds.Height > tabs.Bounds.Height, "The tree is not higher than the tabs.");
        }

        [AvaloniaFact]
        public void Show_TheTabs_KeepTheirWidthOfBefore()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();

            var tabs = TestServiceProviderFactory.MainTabControl(window);

            Assert.True(tabs.Bounds.Width >= 400, "The tabs are narrower than before.");
        }

        [AvaloniaFact]
        public void Add_ACompositionAndALayer_ShowsThemInTheTree()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var tree = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var before = tree.Rows.RealizedRows.Count();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            Support.ListTreeTestScreen.Settle(window);

            // The group Compositions, the composition, its group Layers and the layer. The tree does not show an empty group.
            Assert.Equal(before + 4, tree.Rows.RealizedRows.Count());
        }

        private static (Window Window, GridSplitter Splitter, Grid Grid, ListTreeProject Tree, TabControl Tabs) Show()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var tree = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var grid = tree.GetVisualAncestors().OfType<Grid>().First(g => g.ColumnDefinitions.Count == 2);
            var splitter = grid.Children.OfType<GridSplitter>().Single();
            return (window, splitter, grid, tree, TestServiceProviderFactory.MainTabControl(window));
        }

        private static double Left(Visual control, Visual root) => control.TranslatePoint(new Point(0, 0), root)!.Value.X;

        [AvaloniaFact]
        public void Show_TheTreeAndTheTabs_TouchEachOther()
        {
            var (window, _, _, tree, tabs) = Show();

            Assert.Equal(Left(tabs, window), Left(tree, window) + tree.Bounds.Width, 0.5);
        }

        [AvaloniaFact]
        public void Show_TheColumns_FillTheWholeWidth()
        {
            var (_, _, grid, _, _) = Show();

            Assert.Equal(grid.Bounds.Width, grid.ColumnDefinitions.Sum(column => column.ActualWidth), 0.5);
        }

        [AvaloniaFact]
        public void Show_TheSplitter_LiesOverTheEdgeOfBothPanels()
        {
            var (window, splitter, grid, tree, tabs) = Show();
            var edge = Left(tabs, window);
            var splitterLeft = Left(splitter, window);

            Assert.Equal(Convert.ToDouble(Application.Current!.FindResource("SplitterSize")), splitter.Bounds.Width);
            Assert.True(splitterLeft < edge && splitterLeft + splitter.Bounds.Width > edge, "The splitter is not over the edge.");
            Assert.Same(GridResizeDirection.Columns.GetType(), splitter.ResizeDirection.GetType());
            Assert.Equal(GridResizeDirection.Columns, splitter.ResizeDirection);
            Assert.Same(Application.Current!.FindResource("GridSplitterCMiX"), splitter.Theme);

            // It is on top: it comes after the two side panels in the grid.
            Assert.All(grid.Children.OfType<PanelSide>(), host => Assert.True(grid.Children.IndexOf(splitter) > grid.Children.IndexOf(host)));
        }

        [AvaloniaFact]
        public void Drag_TheSplitter_MovesTheEdgeByTheDistanceOfTheMouse()
        {
            var (window, splitter, grid, _, _) = Show();
            var before = grid.ColumnDefinitions[0].ActualWidth;
            var start = splitter.TranslatePoint(new Point(splitter.Bounds.Width / 2, 100), window)!.Value;

            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(40, 0), RawInputModifiers.LeftMouseButton);
            window.MouseUp(start + new Vector(40, 0), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            Assert.Equal(before + 40, grid.ColumnDefinitions[0].ActualWidth, 1);
            Assert.Equal(grid.Bounds.Width, grid.ColumnDefinitions.Sum(column => column.ActualWidth), 0.5);
        }

        [AvaloniaFact]
        public void Drag_TheSplitter_StopsAtTheLeastWidthOfAPanel()
        {
            var (window, splitter, grid, _, _) = Show();
            var start = splitter.TranslatePoint(new Point(splitter.Bounds.Width / 2, 100), window)!.Value;

            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(-2000, 0), RawInputModifiers.LeftMouseButton);
            window.MouseUp(start + new Vector(-2000, 0), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.Equal(grid.ColumnDefinitions[0].MinWidth, grid.ColumnDefinitions[0].ActualWidth, 0.5);

            var left = splitter.TranslatePoint(new Point(splitter.Bounds.Width / 2, 100), window)!.Value;
            window.MouseDown(left, MouseButton.Left);
            window.MouseMove(left + new Vector(4000, 0), RawInputModifiers.LeftMouseButton);
            window.MouseUp(left + new Vector(4000, 0), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            Assert.Equal(grid.ColumnDefinitions[1].MinWidth, grid.ColumnDefinitions[1].ActualWidth, 0.5);
        }

        [AvaloniaFact]
        public void Show_TheSplitter_LeavesTheScrollBarOfTheTreeFree()
        {
            var (window, splitter, _, tree, _) = Show();
            var bar = tree.GetVisualDescendants().OfType<global::Avalonia.Controls.Primitives.ScrollBar>().First(b => b.Orientation == global::Avalonia.Layout.Orientation.Vertical);

            var barRight = Left(bar, window) + bar.Bounds.Width;

            Assert.True(barRight <= Left(splitter, window) + 0.5, "The splitter is over the scroll bar of the tree.");
        }

        private static int Brightness(IBrush? brush)
        {
            var color = ((ISolidColorBrush)brush!).Color;
            return color.R + color.G + color.B;
        }

        [AvaloniaFact]
        public void Show_TheWindowSurface_IsDarkerThanTheCards()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var cards = Brightness((IBrush)Application.Current!.FindResource("PanelBackgroundLevel1Brush")!);

            Assert.True(Brightness(window.Background) < cards, "The window is not darker than the cards.");

            var mainPanel = window.GetVisualDescendants().OfType<DockPanel>().First(d => d.Background != null);
            Assert.True(Brightness(mainPanel.Background) < cards, "The main panel is not darker than the cards.");
        }

        // Where the card of a ModelEntity is shown: the Entities tab, and the entity tab of a layer.
        [AvaloniaTheory]
        [InlineData("TabEntities")]
        [InlineData("TabLayers")]
        public void Show_NothingBehindTheCards_IsLighterThanTheWindow(string tabName)
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
        public void Show_TheTabStripOfALayer_IsLighterThanTheWindowAndFillsTheHeight()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var mainTabs = TestServiceProviderFactory.MainTabControl(window);
            mainTabs.SelectedItem = mainTabs.Items.OfType<TabItem>().First(t => t.Content?.GetType().Name == "TabLayers");
            TestServiceProviderFactory.Pump();

            var layerTabs = window.GetVisualDescendants().OfType<Views.Layer>().First().GetVisualDescendants().OfType<TabControl>().First();
            var strip = layerTabs.GetVisualDescendants().OfType<Border>().First(b => b.Child is ItemsPresenter);

            Assert.True(Brightness(strip.Background) > Brightness(window.Background), "The tab strip is not lighter than the window.");
            Assert.Equal(layerTabs.Bounds.Height, strip.Bounds.Height);
        }

        // Every tab sits on the surface of the main window. The network tab had a lighter surface of its own.
        [AvaloniaTheory]
        [InlineData("Network", "PanelSync")]
        public void Show_TheNetworkTab_SitsOnTheSurfaceOfTheWindow(string icon, string viewName)
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

        // The assets are a panel of a side now. They sit on the surface of the window too.
        [AvaloniaFact]
        public void Show_TheAssetsPanel_SitsOnTheSurfaceOfTheWindow()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.RightPanel = CMiX.Studio.Avalonia.ViewModels.PanelChoice.Assets;
            TestServiceProviderFactory.Pump();

            var view = window.GetVisualDescendants().OfType<UserControl>().First(control => control.GetType().Name == "AssetManager");
            var surfaces = new List<IBrush?> { view.Background };
            surfaces.AddRange(view.GetVisualDescendants().OfType<Border>().Where(border => border.Name == "dropBorder").Select(border => border.Background));
            var windowBrightness = Brightness(window.Background);

            foreach (var brush in surfaces.OfType<ISolidColorBrush>().Where(brush => brush.Color.A > 0))
                Assert.True(Brightness(brush) <= windowBrightness, "AssetManager paints a color lighter than the window.");
        }

        private Views.MainWindow? window;

        public void Dispose()
        {
            window?.Close();
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Show_TheMainWindow_AttachesThePanelFocusAndTheNotification()
        {
            var (provider, shown, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = shown;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var layerNode = viewModel.ProjectTree.CompositionNodes()[0].Children[0].Children[0];
            viewModel.ProjectTree.SelectedNode = layerNode;
            Settle(window);
            var sides = window.GetVisualDescendants().OfType<PanelSide>()
                .OrderBy(side => side.TranslatePoint(default, window)!.Value.X).ToList();
            var layerItem = sides[0].GetVisualDescendants().OfType<ListTreeProject>().Single().Rows.RowOf(layerNode)!;
            layerItem.Focus();
            Settle(window);
            sides[1].FindControl<ToggleButton>("pickerToggle")!.Focus();
            Settle(window);

            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            Settle(window);
            provider.GetRequiredService<NotificationService>().Show("Opened");

            Assert.Same(layerItem, window.FocusManager!.GetFocusedElement());
            Assert.Equal("Opened", window.FindControl<PanelNotification>("panelNotification")!.Notification.Text);
        }
    }
}
