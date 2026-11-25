using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // A row that closes over the cursor takes the cursor, so the cursor is never on a hidden row.
    // The selection does not change.
    public class ProjectTreeCursorCloseTests
    {
        // A with two layers. Each layer holds an Echo filter. Every row is open.
        private static Scene Scene()
        {
            var scene = CreateScene([2], s => s.Layer.TextureModifierManager.AddItem(typeof(Echo)));
            scene.LayerOfA(1).TextureModifierManager.AddItem(typeof(Echo));
            foreach (var node in scene.All())
                node.IsExpanded = true;
            return scene;
        }

        private static TreeNode EchoOf(Scene scene, int layer) =>
            scene.All().Single(node => node.Item is Echo && ReferenceEquals(node.Item, scene.LayerOfA(layer).TextureModifierManager.ManagerData.Items[0]));

        private static void AssertCursorShows(Scene scene) =>
            Assert.Contains(scene.Tree.CursorNode!, scene.Tree.VisibleNodes());

        [Fact]
        public void Collapse_CursorUnder_MovesCursorToTheRow()
        {
            var scene = Scene();
            var selected = scene.LayerNodeOfA(0);
            scene.Tree.SelectedNode = selected;
            scene.Tree.CursorNode = EchoOf(scene, 1);

            scene.LayerNodeOfA(1).IsExpanded = false;

            Assert.Same(scene.LayerNodeOfA(1), scene.Tree.CursorNode);
            Assert.Same(selected, scene.Tree.SelectedNode);
        }

        [Fact]
        public void Collapse_CursorNotUnder_KeepsTheCursor()
        {
            var scene = Scene();
            var cursor = EchoOf(scene, 0);
            scene.Tree.CursorNode = cursor;

            scene.LayerNodeOfA(1).IsExpanded = false;

            Assert.Same(cursor, scene.Tree.CursorNode);
        }

        [Fact]
        public void Collapse_TheCursorRow_KeepsTheCursor()
        {
            var scene = Scene();
            var cursor = scene.LayerNodeOfA(1);
            scene.Tree.CursorNode = cursor;

            cursor.IsExpanded = false;

            Assert.Same(cursor, scene.Tree.CursorNode);
        }

        // Two parents of the cursor close, in either order. The cursor ends on the higher one, the row that shows.
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Collapse_NestedRows_MovesCursorToTheHighestClosedRow(bool innerFirst)
        {
            var scene = Scene();
            var echo = EchoOf(scene, 1);
            var inner = echo.Parent!;
            var outer = scene.LayerNodeOfA(1);
            Assert.NotSame(inner, outer);
            scene.Tree.CursorNode = echo;

            foreach (var row in innerFirst ? new[] { inner, outer } : new[] { outer, inner })
                row.IsExpanded = false;

            Assert.Same(outer, scene.Tree.CursorNode);
            AssertCursorShows(scene);
        }

        // The end of the search opens the parents of the cursor row, so the cursor stays on its row.
        [Fact]
        public void Clear_TheSearch_KeepsTheCursorOnItsRow()
        {
            var scene = Scene();
            foreach (var node in scene.All())
                node.IsExpanded = false;
            scene.Tree.SelectedNode = null;
            scene.Tree.SearchText = "Echo";
            var echo = EchoOf(scene, 1);
            scene.Tree.CursorNode = echo;

            scene.Tree.SearchText = string.Empty;

            Assert.Same(echo, scene.Tree.CursorNode);
            AssertCursorShows(scene);
            Assert.Null(scene.Tree.SelectedNode);
        }

        // CollapseOthers keeps the path to the cursor open, so the cursor stays on its row. A cursor under the selected row does
        // not count: the cursor is the target.
        [Fact]
        public void CollapseOthers_ACursorDeepInTheTree_KeepsTheCursorOnItsRow()
        {
            var scene = Scene();
            var selected = EchoOf(scene, 0);
            scene.Tree.SelectedNode = selected;
            var cursor = EchoOf(scene, 1);
            scene.Tree.CursorNode = cursor;

            var closed = scene.Tree.CloseOtherRows();

            Assert.NotEmpty(closed);
            Assert.Same(cursor, scene.Tree.CursorNode);
            Assert.Same(selected, scene.Tree.SelectedNode);
            AssertCursorShows(scene);
        }
    }
}
