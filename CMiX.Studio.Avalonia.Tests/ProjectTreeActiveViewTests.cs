using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.String;
using CMiX.UI.Panels;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Echo = CMiX.Core.Texturing.Filters.Echo;

namespace CMiX.Studio.Avalonia.Tests
{
    // Both side panels can show the same tree. One request of the tree has one answer: the active view answers it
    // (ProjectTree.IsActiveView: the view with the focus in its rows, else the view with the focus in its side panel, else the view
    // under the pointer, else the first view).
    public class ProjectTreeActiveViewTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        // Two views of the tree side by side. Each view has its own height.
        private (ProjectTreeView First, ProjectTreeView Second) Show(Scene scene, double firstHeight = 600, double secondHeight = 600)
        {
            var first = new ProjectTreeView { DataContext = scene.Tree, Height = firstHeight, VerticalAlignment = VerticalAlignment.Top };
            var second = new ProjectTreeView { DataContext = scene.Tree, Height = secondHeight, VerticalAlignment = VerticalAlignment.Top };
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
            Grid.SetColumn(second, 1);
            grid.Children.Add(first);
            grid.Children.Add(second);
            window = new Window { Content = grid, Width = 800, Height = 900 };
            window.Show();
            foreach (var panel in window.GetVisualDescendants().OfType<PanelCollapse>())
                panel.Duration = TimeSpan.Zero;
            TreeTestScreen.Settle(window);
            return (first, second);
        }

        private static ProjectTreeView Pick(ProjectTreeView first, ProjectTreeView second, int index) => index == 0 ? first : second;

        // The focus goes to a row of the view, so the view is the active view.
        private void FocusView(ProjectTreeView view, TreeNode node)
        {
            Assert.True(TreeTestScreen.ItemOf(view, node).Focus(NavigationMethod.Tab));
            Dispatcher.UIThread.RunJobs();
            Assert.True(view.IsActiveView);
        }

        private static TreeView TreeViewOf(ProjectTreeView view) => view.GetVisualDescendants().OfType<TreeView>().First();

        private static TextBoxEditable NameBoxOf(ProjectTreeView view, TreeNode node) =>
            TreeTestScreen.RowOf(view, node).GetVisualDescendants().OfType<TextBoxEditable>().First();

        [AvaloniaTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void IsActiveView_TwoViews_OnlyTheFocusedViewIsActive(int active)
        {
            var scene = CreateScene();
            var (first, second) = Show(scene);

            FocusView(Pick(first, second, active), scene.NodeA);

            Assert.False(Pick(first, second, 1 - active).IsActiveView);
        }

        [AvaloniaTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void AddRequested_TwoViews_OpensTheAddPanelOfTheActiveViewOnly(int active)
        {
            var scene = CreateScene();
            var (first, second) = Show(scene);
            FocusView(Pick(first, second, active), scene.NodeA);

            scene.Tree.AddAtTargetCommand.Execute(null);
            TreeTestScreen.Settle(window!);

            Assert.True(Pick(first, second, active).MenuAddPanel.IsOpen);
            Assert.False(Pick(first, second, 1 - active).MenuAddPanel.IsOpen);
            Pick(first, second, active).MenuAddPanel.Close();
        }

        [AvaloniaTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void MenuRequested_TwoViews_OpensTheMenuOfTheActiveViewOnly(int active)
        {
            var scene = CreateScene();
            var (first, second) = Show(scene);
            scene.Tree.SelectedNode = scene.NodeA;
            TreeTestScreen.Settle(window!);
            FocusView(Pick(first, second, active), scene.NodeA);

            scene.Tree.OpenMenuAtTargetCommand.Execute(null);
            TreeTestScreen.Settle(window!);

            var menu = TreeViewOf(Pick(first, second, active)).ContextMenu;
            Assert.True(menu?.IsOpen);
            Assert.NotEqual(true, TreeViewOf(Pick(first, second, 1 - active)).ContextMenu?.IsOpen);
            menu!.Close();
        }

        [AvaloniaTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void RenameRequested_TwoViews_EditsTheRowOfTheActiveViewOnly(int active)
        {
            var scene = CreateScene();
            var (first, second) = Show(scene);
            scene.Tree.SelectedNode = scene.NodeA;
            TreeTestScreen.Settle(window!);
            FocusView(Pick(first, second, active), scene.NodeA);

            scene.Tree.RenameSelectedCommand.Execute(null);
            TreeTestScreen.Settle(window!);

            Assert.True(NameBoxOf(Pick(first, second, active), scene.NodeA).IsEditing);
            Assert.False(NameBoxOf(Pick(first, second, 1 - active), scene.NodeA).IsEditing);
        }

        // No view has the focus and the pointer is over no view, so the first view is the active view.
        [AvaloniaFact]
        public void TreeFocusRequested_TwoViewsAndNoFocus_FocusesTheFirstView()
        {
            var scene = CreateScene();
            var (first, second) = Show(scene);
            Assert.Null(window!.FocusManager!.GetFocusedElement());

            scene.Tree.FocusTree();
            Dispatcher.UIThread.RunJobs();

            Assert.True(first.IsKeyboardFocusWithin);
            Assert.False(second.IsKeyboardFocusWithin);
        }

        // The target row is out of view in the short first view. A view that answers the request brings the target row into view, so
        // the first view must not scroll.
        [AvaloniaFact]
        public void CollapseOthersRequested_TwoViews_OnlyTheActiveViewMovesItsRows()
        {
            var scene = CreateScene([12], s => s.Layer.TextureModifierManager.AddItem(typeof(Echo)));
            for (var i = 1; i < 12; i++)
                scene.LayerOfA(i).TextureModifierManager.AddItem(typeof(Echo));
            foreach (var node in scene.All())
                node.IsExpanded = true;
            var (first, second) = Show(scene, firstHeight: 150, secondHeight: 880);
            var target = scene.All().Last(node => node.Item is Echo);
            scene.Tree.SelectedNode = target;
            TreeTestScreen.Settle(window!);
            var scroller = first.GetVisualDescendants().OfType<ScrollViewer>().First();
            scroller.Offset = scroller.Offset.WithY(0);
            TreeTestScreen.Settle(window!);
            FocusView(second, scene.NodeA);

            scene.Tree.CollapseOthersCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();
            for (var i = 0; i < 10; i++)
                TreeTestScreen.Settle(window!);

            Assert.False(scene.LayerNodeOfA(0).IsExpanded);
            Assert.Equal(0, scroller.Offset.Y);
        }

        // Both side panels of the main window show the tree. Tree.Search focuses the search box of the panel of the active view.
        [AvaloniaTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void SearchFocusRequested_TreeInBothPanels_FocusesTheBoxOfTheActivePanel(int active)
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.Project.CompositionManager.AddItem(typeof(CMiX.Core.Compositing.Composition));
            viewModel.RightPanel = PanelChoice.Tree;
            TestServiceProviderFactory.Pump();
            var hosts = mainWindow.GetVisualDescendants().OfType<SidePanelHost>().ToList();
            Assert.Equal(2, hosts.Count);
            var view = hosts[active].GetVisualDescendants().OfType<ProjectTreeView>().Single();
            Assert.True(TreeTestScreen.ItemOf(view, viewModel.ProjectTree.Roots[0]).Focus(NavigationMethod.Tab));
            Dispatcher.UIThread.RunJobs();

            viewModel.ProjectTree.FocusSearch();
            TestServiceProviderFactory.Pump();

            var focused = mainWindow.FocusManager!.GetFocusedElement() as Visual;
            Assert.NotNull(focused);
            Assert.True(hosts[active].GetVisualDescendants().OfType<TreeSearchBox>().Single().IsVisualAncestorOf(focused!));
        }

        // Both side panels of the main window show the tree. The pointer is over the tree of the first panel, and the search box of
        // the second panel has the focus.
        private (Window Window, List<SidePanelHost> Hosts, MainViewModel ViewModel) ShowFocusInSecondSearchBox()
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.Project.CompositionManager.AddItem(typeof(CMiX.Core.Compositing.Composition));
            viewModel.RightPanel = PanelChoice.Tree;
            TestServiceProviderFactory.Pump();
            var hosts = mainWindow.GetVisualDescendants().OfType<SidePanelHost>().ToList();
            Assert.Equal(2, hosts.Count);
            var firstView = ViewOf(hosts[0]);
            mainWindow.MouseMove(firstView.TranslatePoint(new Point(firstView.Bounds.Width / 2, firstView.Bounds.Height / 2), mainWindow)!.Value);
            Dispatcher.UIThread.RunJobs();
            Assert.True(firstView.IsPointerOver);
            BoxOf(hosts[1]).Field.FocusInput();
            Dispatcher.UIThread.RunJobs();
            Assert.True(BoxOf(hosts[1]).IsKeyboardFocusWithin);
            return (mainWindow, hosts, viewModel);
        }

        private static ProjectTreeView ViewOf(SidePanelHost host) => host.GetVisualDescendants().OfType<ProjectTreeView>().Single();

        private static TreeSearchBox BoxOf(SidePanelHost host) => host.GetVisualDescendants().OfType<TreeSearchBox>().Single();

        [AvaloniaFact]
        public void FocusTree_FocusInTheSearchBoxOfTheSecondPanel_FocusesTheRowsOfTheSecondPanelOnly()
        {
            var (_, hosts, viewModel) = ShowFocusInSecondSearchBox();

            viewModel.ProjectTree.FocusTree();
            TestServiceProviderFactory.Pump();

            Assert.True(ViewOf(hosts[1]).IsKeyboardFocusWithin);
            Assert.False(ViewOf(hosts[0]).IsKeyboardFocusWithin);
        }

        [AvaloniaFact]
        public void FocusSearch_PointerOverTheFirstTreeAndFocusInTheSecondPanel_FocusesTheSecondSearchBox()
        {
            var (_, hosts, viewModel) = ShowFocusInSecondSearchBox();

            viewModel.ProjectTree.FocusSearch();
            TestServiceProviderFactory.Pump();

            Assert.True(BoxOf(hosts[1]).IsKeyboardFocusWithin);
            Assert.False(BoxOf(hosts[0]).IsKeyboardFocusWithin);
        }
    }
}
