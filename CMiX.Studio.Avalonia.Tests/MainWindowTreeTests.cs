using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tree of the project stands in its own column, left of the tabs, and is always visible.
    // The tree has the whole height of that column. The master beat is under the tabs only.
    public class MainWindowTreeTests
    {
        [AvaloniaFact]
        public void Show_TheTree_StandsLeftOfTheTabsAtTheirTop()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            var tree = window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
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
            var tree = window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            var columns = tree.GetVisualAncestors().OfType<Grid>().First(grid => grid.ColumnDefinitions.Count == 2).ColumnDefinitions;

            Assert.True(columns[0].ActualWidth > 0);
            Assert.Equal(columns[0].ActualWidth, columns[1].ActualWidth);
        }

        [AvaloniaFact]
        public void Show_TheMasterBeat_IsUnderTheTabsOnly()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var tree = window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
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
        public void Show_TheTreeRows_HaveTheSameSpaceOnAllSides()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var tree = window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            var row = tree.GetVisualDescendants().OfType<TreeViewItem>().First();
            var rowOrigin = row.TranslatePoint(new(0, 0), tree)!.Value;

            Assert.Equal(8, rowOrigin.X, 0.5);
            Assert.Equal(8, tree.Bounds.Width - (rowOrigin.X + row.Bounds.Width), 0.5);
            // The toolbar is over the tree, so the space above the first row is counted from the bottom of the toolbar.
            var toolbar = tree.GetVisualDescendants().OfType<ToolbarProjectTree>().Single();
            Assert.Equal(8, rowOrigin.Y - toolbar.Bounds.Bottom, 0.5);

            // The right space of 8 is the room that the scroll bar has (ScrollBarGutter), so the list has no margin on the right.
            // The bottom is 8 as the left and the top, and the list has no padding of its own.
            var treeView = tree.GetVisualDescendants().OfType<TreeView>().First();
            Assert.Equal(new Thickness(8, 8, 0, 8), treeView.Margin);
            Assert.Equal(new Thickness(0), treeView.Padding);
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
            var tree = window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            var before = tree.GetVisualDescendants().OfType<TreeViewItem>().Count();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();

            // A composition and its three groups (layers, filters and modifiers), a layer and its five groups.
            Assert.Equal(before + 10, tree.GetVisualDescendants().OfType<TreeViewItem>().Count());
        }

        [AvaloniaFact]
        public void NodeChosen_Raised_KeepsTheTabThatIsSelected()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            var leftBefore = viewModel.LeftPanel;
            var rightBefore = viewModel.RightPanel;
            TestServiceProviderFactory.SelectMainTab(window, "Settings");
            var tree = window.GetVisualDescendants().OfType<ProjectTreeView>().Single();

            tree.RaiseEvent(new NodeChosenEventArgs(ProjectTreeView.NodeChosenEvent, tree, viewModel.ProjectTree.CompositionNodes()[0]));
            TestServiceProviderFactory.Pump();

            // Each side keeps its content, and the tab that the user picked stays picked.
            Assert.Same(leftBefore, viewModel.LeftPanel);
            Assert.Same(rightBefore, viewModel.RightPanel);
            Assert.Same(tree, window.GetVisualDescendants().OfType<ProjectTreeView>().Single());
            var selected = TestServiceProviderFactory.MainTabControl(window).SelectedItem as TabItem;
            Assert.Equal("Settings", (selected?.Header as CMiX.UI.Controls.AppIcon)?.IconKey);
        }
    }
}
