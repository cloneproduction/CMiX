using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // Tree.CollapseOthers closes every row of the tree but the path to the row of the arrow keys, else the selected row. The parents of
    // that row open, and the row keeps its own state.
    public class ProjectTreeCollapseOthersTests : IDisposable
    {
        private TreeTestScreen? screen;

        public void Dispose() => screen?.Dispose();

        private static IEnumerable<TreeNode> ParentsOf(Scene scene, TreeNode node)
        {
            for (var parent = node.Parent; parent != null && scene.All().Contains(parent); parent = parent.Parent)
                yield return parent;
        }

        // A with three layers. Each layer holds an Echo filter. Every row is open.
        private static Scene Scene()
        {
            var scene = CreateScene([3], s => s.Layer.TextureModifierManager.AddItem(typeof(Echo)));
            scene.LayerOfA(1).TextureModifierManager.AddItem(typeof(Echo));
            scene.LayerOfA(2).TextureModifierManager.AddItem(typeof(Echo));
            foreach (var node in scene.All())
                node.IsExpanded = true;
            return scene;
        }

        private static TreeNode EchoOf(Scene scene, int layer) =>
            scene.All().Single(node => node.Item is Echo && ReferenceEquals(node.Item, scene.LayerOfA(layer).TextureModifierManager.ManagerData.Items[0]));

        [Fact]
        public void CollapseOthers_ADeepTarget_OpensItsParentsAndClosesEveryOtherRow()
        {
            var scene = Scene();
            var target = EchoOf(scene, 1);
            foreach (var parent in ParentsOf(scene, target))
                parent.IsExpanded = false;
            scene.Tree.SelectedNode = target;

            scene.Tree.CloseOtherRows();

            var path = ParentsOf(scene, target).ToList();
            Assert.All(path, parent => Assert.True(parent.IsExpanded));
            Assert.All(scene.All().Where(node => node.Children.Count > 0 && !path.Contains(node) && !ReferenceEquals(node, target)),
                node => Assert.False(node.IsExpanded));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CollapseOthers_ATargetWithChildren_KeepsItsOwnState(bool open)
        {
            var scene = Scene();
            var target = scene.LayerNodeOfA(1);
            target.IsExpanded = open;
            scene.Tree.SelectedNode = target;

            scene.Tree.CloseOtherRows();

            Assert.Equal(open, target.IsExpanded);
            Assert.False(scene.LayerNodeOfA(0).IsExpanded);
        }

        [Fact]
        public void CollapseOthers_TheRowOfTheArrowKeys_WinsOverTheSelectedRow()
        {
            var scene = Scene();
            scene.Tree.SelectedNode = scene.LayerNodeOfA(0);
            scene.Tree.CursorNode = EchoOf(scene, 2);

            scene.Tree.CloseOtherRows();

            Assert.True(scene.LayerNodeOfA(2).IsExpanded);
            Assert.False(scene.LayerNodeOfA(0).IsExpanded);
        }

        [Fact]
        public void CollapseOthers_NoTarget_ChangesNothing()
        {
            var scene = Scene();
            scene.Tree.SelectedNode = null;
            scene.Tree.CursorNode = null;

            var closed = scene.Tree.CloseOtherRows();

            Assert.Empty(closed);
            Assert.All(scene.All().Where(node => node.Children.Count > 0), node => Assert.True(node.IsExpanded));
            Assert.False(scene.Tree.CollapseOthersCommand.CanExecute(null));
        }

        [Fact]
        public void CollapseOthers_DuringADrag_ChangesNothing()
        {
            var scene = Scene();
            scene.Tree.SelectedNode = EchoOf(scene, 1);
            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));

            var closed = scene.Tree.CloseOtherRows();

            Assert.Empty(closed);
            Assert.All(scene.All().Where(node => node.Children.Count > 0), node => Assert.True(node.IsExpanded));
            scene.Tree.EndDrag();
        }

        // The command closes rows that the search opened. They leave the search, so the end of the search does not open them again.
        [Fact]
        public void CollapseOthers_DuringASearch_KeepsTheClosedRowsClosedAfterTheSearch()
        {
            var scene = Scene();
            foreach (var node in scene.All())
                node.IsExpanded = false;
            scene.Tree.SearchText = "Echo";
            var opened = scene.All().Where(node => node.IsExpanded).ToList();
            Assert.NotEmpty(opened);
            var target = EchoOf(scene, 2);
            scene.Tree.SelectedNode = target;
            // The scene selects the last Echo, so the close above took the cursor to a top row. Put it back on the target.
            scene.Tree.CursorNode = target;

            scene.Tree.CloseOtherRows();
            scene.Tree.SearchText = string.Empty;

            Assert.False(scene.LayerNodeOfA(0).IsExpanded);
            Assert.False(scene.LayerNodeOfA(1).IsExpanded);
            Assert.All(ParentsOf(scene, target), parent => Assert.True(parent.IsExpanded));
        }

        // ---------------------------------------------------------------- the view

        // Many layers with open groups, in a short window, scrolled down. The command runs through the view.
        private (TreeTestScreen Screen, Scene Scene) ShowScrolled()
        {
            var scene = CreateScene([12], s => s.Layer.TextureModifierManager.AddItem(typeof(Echo)));
            for (var i = 1; i < 12; i++)
                scene.LayerOfA(i).TextureModifierManager.AddItem(typeof(Echo));
            foreach (var node in scene.All())
                node.IsExpanded = true;
            screen = TreeTestScreen.Show(scene.Tree, 400, 400);
            return (screen, scene);
        }

        private static ScrollViewer ScrollerOf(TreeTestScreen screen) => screen.View.GetVisualDescendants().OfType<ScrollViewer>().First();

        private static double TopOf(TreeTestScreen screen, TreeNode node)
        {
            var row = TreeTestScreen.RowOf(screen.View, node);
            return row.TranslatePoint(new Point(0, 0), ScrollerOf(screen))!.Value.Y;
        }

        [AvaloniaFact]
        public void CollapseOthers_ATargetInView_StaysAtItsPlaceWhileTheRowsAboveClose()
        {
            var (screen, scene) = ShowScrolled();
            var target = scene.LayerNodeOfA(8);
            var scroller = ScrollerOf(screen);
            scroller.Offset = scroller.Offset.WithY(TopOf(screen, target) + scroller.Offset.Y - 120);
            screen.Settle();
            scene.Tree.SelectedNode = target;
            screen.Settle();
            var before = TopOf(screen, target);
            Assert.InRange(before, 0, scroller.Viewport.Height - 24);

            scene.Tree.CollapseOthersCommand.Execute(null);
            for (var i = 0; i < 10; i++)
                screen.Settle();

            Assert.False(scene.LayerNodeOfA(2).IsExpanded);
            Assert.Equal(before, TopOf(screen, target), 1.0);
        }

        [AvaloniaFact]
        public void CollapseOthers_ATargetOutOfView_BringsItIntoView()
        {
            var (screen, scene) = ShowScrolled();
            var target = EchoOf(scene, 11);
            scene.Tree.SelectedNode = target;
            screen.Settle();
            var scroller = ScrollerOf(screen);
            scroller.Offset = scroller.Offset.WithY(0);
            screen.Settle();
            Assert.True(TopOf(screen, target) > scroller.Viewport.Height);

            scene.Tree.CollapseOthersCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();
            for (var i = 0; i < 10; i++)
                screen.Settle();

            Assert.InRange(TopOf(screen, target), 0, scroller.Viewport.Height - 1);
        }
    }
}
