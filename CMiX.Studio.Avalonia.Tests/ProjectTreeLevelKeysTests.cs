using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Echo = CMiX.Core.Texturing.Filters.Echo;
using Layer = CMiX.Core.Compositing.Layer;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // Tree.ExpandBranch (Shift+E), Tree.CloseLevel (Q), Tree.OpenLevel (Shift+Q) and Tree.CollapseAll (Shift+Alt+E) change the open
    // state of the rows only. A row that closes over the cursor takes the cursor, so the cursor stays on a row that shows.
    public class ProjectTreeLevelKeysTests
    {
        // A with three layers and B with one layer. Each layer of A holds an Echo filter. Every row is open.
        private static Scene Nested()
        {
            var scene = CreateScene([3, 1], s =>
            {
                for (var i = 0; i < 3; i++)
                    s.LayerOfA(i).TextureModifierManager.AddItem(typeof(Echo));
            });
            foreach (var node in scene.All())
                node.IsExpanded = true;
            return scene;
        }

        private static TreeNode EchoOf(Scene scene, int layer) =>
            scene.All().Single(node => node.Item is Echo && ReferenceEquals(node.Item, scene.LayerOfA(layer).TextureModifierManager.ManagerData.Items[0]));

        private static int DepthOf(Scene scene, TreeNode node)
        {
            var depth = 0;
            for (var parent = node.Parent; parent != null && scene.All().Contains(parent); parent = parent.Parent)
                depth++;
            return depth;
        }

        private static IEnumerable<TreeNode> WithChildren(IEnumerable<TreeNode> nodes) => nodes.Where(node => node.Children.Count > 0);

        private static void CloseAll(Scene scene)
        {
            foreach (var node in scene.All())
                node.IsExpanded = false;
        }

        private static void AssertCursorShows(ProjectTree tree) =>
            Assert.Contains(tree.CursorNode!, tree.VisibleNodes());

        // ---------------------------------------------------------------- Tree.ExpandBranch

        [Fact]
        public void ExpandBranch_AClosedBranch_OpensTheTargetAndEveryRowUnderIt()
        {
            var scene = Nested();
            CloseAll(scene);
            scene.NodeA.Parent!.IsExpanded = true;
            scene.Tree.CursorNode = scene.NodeA;

            scene.Tree.ExpandBranch();

            Assert.All(WithChildren(ListTreeTestScreen.All(new[] { scene.NodeA })), node => Assert.True(node.IsExpanded));
            Assert.False(scene.NodeB.IsExpanded);
            AssertCursorShows(scene.Tree);
        }

        [Fact]
        public void ExpandBranch_DuringASearch_KeepsTheBranchOpenAfterTheSearch()
        {
            var scene = Nested();
            CloseAll(scene);
            scene.Tree.SearchText = "Echo";
            var target = scene.LayerNodeOfA(1);
            scene.Tree.CursorNode = target;

            scene.Tree.ExpandBranch();
            scene.Tree.SearchText = string.Empty;

            Assert.All(WithChildren(ListTreeTestScreen.All(new[] { target })), node => Assert.True(node.IsExpanded));
        }

        // ---------------------------------------------------------------- Tree.CloseLevel

        [Fact]
        public void CloseLevel_EveryRowOpen_ClosesOnlyTheDeepestOpenRows()
        {
            var scene = Nested();
            var open = WithChildren(scene.All()).ToList();
            var deepest = open.Max(node => DepthOf(scene, node));

            scene.Tree.CloseLevel();

            Assert.All(open, node => Assert.Equal(DepthOf(scene, node) != deepest, node.IsExpanded));
        }

        [Fact]
        public void CloseLevel_TwiceWithTheCursorDeep_ClosesTwoLevelsAndKeepsTheCursorOnARowThatShows()
        {
            var scene = Nested();
            var echo = EchoOf(scene, 1);
            scene.Tree.CursorNode = echo;
            var deepest = WithChildren(scene.All()).Max(node => DepthOf(scene, node));

            scene.Tree.CloseLevel();
            AssertCursorShows(scene.Tree);
            scene.Tree.CloseLevel();

            Assert.All(WithChildren(scene.All()), node => Assert.Equal(DepthOf(scene, node) < deepest - 1, node.IsExpanded));
            AssertCursorShows(scene.Tree);
            Assert.True(echo.IsOrIsUnder(scene.Tree.CursorNode!));
        }

        // ---------------------------------------------------------------- Tree.OpenLevel

        [Fact]
        public void OpenLevel_EveryRowClosed_OpensTheTopRowOnly()
        {
            var scene = Nested();
            CloseAll(scene);
            var top = scene.Tree.Roots[0];

            scene.Tree.OpenLevel();

            Assert.True(top.IsExpanded);
            Assert.All(WithChildren(scene.All()).Where(node => !ReferenceEquals(node, top)), node => Assert.False(node.IsExpanded));
        }

        [Fact]
        public void OpenLevel_ClosedRowsAtTwoDepths_OpensTheShallowerRowsOnly()
        {
            var scene = Nested();
            var deep = scene.LayerNodeOfA(1);
            deep.IsExpanded = false;
            scene.NodeB.IsExpanded = false;

            scene.Tree.OpenLevel();

            Assert.True(scene.NodeB.IsExpanded);
            Assert.False(deep.IsExpanded);
        }

        // ---------------------------------------------------------------- Tree.CollapseAll

        [Fact]
        public void CollapseAll_ACursorDeepInTheTree_ClosesEveryRowAndMovesTheCursorToTheTopRow()
        {
            var scene = Nested();
            scene.Undo.Clear();
            scene.Tree.CursorNode = EchoOf(scene, 2);

            scene.Tree.CollapseAll();

            Assert.All(WithChildren(scene.All()), node => Assert.False(node.IsExpanded));
            Assert.Same(scene.Tree.Roots[0], scene.Tree.CursorNode);
            Assert.False(scene.Undo.CanUndo);
        }

        [Fact]
        public void CollapseAll_DuringADrag_ChangesNothing()
        {
            var scene = Nested();
            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));

            scene.Tree.CollapseAll();
            scene.Tree.CloseLevel();

            Assert.All(WithChildren(scene.All()), node => Assert.True(node.IsExpanded));
            scene.Tree.EndDrag();
        }

        // ---------------------------------------------------------------- the keys of the main window
    }
}
