using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Composition = CMiX.Core.Compositing.Composition;
using Echo = CMiX.Core.Texturing.Filters.Echo;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // Tree.Reveal (F) reveals the selected row: its closed parents open, the cursor goes to it, and the view scrolls it to the middle.
    // The selection does not change. The opened parents leave the search.
    public class ProjectTreeRevealTests : IDisposable
    {
        private TreeTestScreen? screen;
        private Window? window;

        public void Dispose()
        {
            screen?.Dispose();
            window?.Close();
        }

        private static IEnumerable<TreeNode> ParentsOf(Scene scene, TreeNode node)
        {
            for (var parent = node.Parent; parent != null && scene.All().Contains(parent); parent = parent.Parent)
                yield return parent;
        }

        // A with a number of layers. Each layer holds an Echo filter. Every row is open.
        private static Scene Scene(int layers)
        {
            var scene = CreateScene([layers], s => s.Layer.TextureModifierManager.AddItem(typeof(Echo)));
            for (var i = 1; i < layers; i++)
                scene.LayerOfA(i).TextureModifierManager.AddItem(typeof(Echo));
            foreach (var node in scene.All())
                node.IsExpanded = true;
            return scene;
        }

        private static TreeNode EchoOf(Scene scene, int layer) =>
            scene.All().Single(node => node.Item is Echo && ReferenceEquals(node.Item, scene.LayerOfA(layer).TextureModifierManager.ManagerData.Items[0]));

        // Selects the Echo of the layer and closes its parents under the composition. The cursor goes to another row.
        private static TreeNode SelectUnderClosedParents(Scene scene, int layer)
        {
            var target = EchoOf(scene, layer);
            scene.Tree.SelectedNode = target;
            foreach (var parent in ParentsOf(scene, target).Where(parent => parent.Item is not Composition && parent.Parent?.Parent != null))
                parent.IsExpanded = false;
            scene.Tree.CursorNode = scene.NodeA;
            return target;
        }

        [Fact]
        public void RevealSelected_ARowUnderClosedParents_OpensTheParentsAndPutsTheCursorOnIt()
        {
            var scene = Scene(3);
            var target = SelectUnderClosedParents(scene, 1);
            Assert.Contains(ParentsOf(scene, target), parent => !parent.IsExpanded);
            scene.Undo.Clear();
            TreeNode? requested = null;
            scene.Tree.RevealRequested += (_, node) => requested = node;

            Assert.True(scene.Tree.TryRevealSelected());

            Assert.All(ParentsOf(scene, target), parent => Assert.True(parent.IsExpanded));
            Assert.Same(target, scene.Tree.CursorNode);
            Assert.Same(target, scene.Tree.SelectedNode);
            Assert.Same(target, requested);
            Assert.False(scene.Undo.CanUndo);
        }

        // The search opened the parents of the selected row. After the reveal they belong to the user: a search that no longer needs them,
        // and the end of the search, do not close them.
        [Fact]
        public void RevealSelected_DuringASearch_TheSearchDoesNotCloseTheParents()
        {
            var scene = Scene(3);
            foreach (var node in scene.All())
                node.IsExpanded = false;
            scene.Tree.SearchText = "Echo";
            var target = EchoOf(scene, 1);
            Assert.All(ParentsOf(scene, target), parent => Assert.True(parent.IsExpanded));
            scene.Tree.SelectedNode = target;

            scene.Tree.RevealSelected();
            scene.Tree.SearchText = "no row has this name";

            Assert.All(ParentsOf(scene, target), parent => Assert.True(parent.IsExpanded));
            Assert.False(scene.LayerNodeOfA(0).IsExpanded);

            scene.Tree.SearchText = string.Empty;

            Assert.All(ParentsOf(scene, target), parent => Assert.True(parent.IsExpanded));
        }

        [Fact]
        public void RevealSelected_NoSelection_DoesNothing()
        {
            var scene = Scene(3);
            scene.Tree.SelectedNode = null;
            scene.LayerNodeOfA(1).IsExpanded = false;
            scene.Tree.CursorNode = scene.LayerNodeOfA(2);
            var requested = false;
            scene.Tree.RevealRequested += (_, _) => requested = true;

            Assert.False(scene.Tree.TryRevealSelected());

            Assert.False(scene.Tree.RevealSelectedCommand.CanExecute(null));
            Assert.False(scene.LayerNodeOfA(1).IsExpanded);
            Assert.Same(scene.LayerNodeOfA(2), scene.Tree.CursorNode);
            Assert.False(requested);
        }

        // ---------------------------------------------------------------- the view

        private static ScrollViewer ScrollerOf(TreeTestScreen screen) => screen.View.GetVisualDescendants().OfType<ScrollViewer>().First();

        // The middle of the row of the node, in the coordinates of the scroller.
        private static double MiddleOf(TreeTestScreen screen, TreeNode node)
        {
            var row = TreeTestScreen.RowOf(screen.View, node);
            return row.TranslatePoint(new Point(0, row.Bounds.Height / 2), ScrollerOf(screen))!.Value.Y;
        }

        [AvaloniaFact]
        public void RevealSelected_ARowFarBelowTheView_ScrollsTheRowToTheMiddle()
        {
            var scene = Scene(12);
            var target = SelectUnderClosedParents(scene, 6);
            screen = TreeTestScreen.Show(scene.Tree, 400, 400);
            var scroller = ScrollerOf(screen);
            scroller.Offset = scroller.Offset.WithY(0);
            screen.Settle();

            scene.Tree.RevealSelectedCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();
            for (var i = 0; i < 10; i++)
                screen.Settle();

            Assert.True(scroller.Offset.Y > 0);
            var row = TreeTestScreen.RowOf(screen.View, target).Bounds.Height;
            Assert.InRange(MiddleOf(screen, target), scroller.Viewport.Height / 2 - row, scroller.Viewport.Height / 2 + row);
        }

        // ---------------------------------------------------------------- the key of the main window

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheRevealCommandOnF()
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;

            var binding = mainWindow.KeyBindings.Single(candidate => Equals(candidate.Gesture, viewModel.Shortcuts["Tree.Reveal"]));

            Assert.Equal(new KeyGesture(Key.F), viewModel.Shortcuts["Tree.Reveal"]);
            Assert.Same(viewModel.ProjectTree.RevealSelectedCommand, binding.Command);
        }

        [AvaloniaFact]
        public void Press_FWithThePointerOverTheTree_RevealsTheSelectedRow()
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var tree = viewModel.ProjectTree;
            var compositionNode = tree.CompositionNodes()[0];
            var layer = compositionNode.Children[0].Children[1];
            tree.SelectedNode = layer;
            compositionNode.IsExpanded = false;
            TreeTestScreen.Settle(mainWindow);
            mainWindow.Focus();
            var view = mainWindow.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            var row = TreeTestScreen.RowOf(view, compositionNode);
            mainWindow.MouseMove(row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), mainWindow)!.Value);
            Dispatcher.UIThread.RunJobs();
            Assert.True(view.IsPointerOver);
            Assert.NotSame(layer, tree.CursorNode);

            mainWindow.KeyPressQwerty(PhysicalKey.F, RawInputModifiers.None);
            TreeTestScreen.Settle(mainWindow);

            Assert.True(compositionNode.IsExpanded);
            Assert.Same(layer, tree.CursorNode);
            Assert.Same(layer, tree.SelectedNode);
        }
    }
}
