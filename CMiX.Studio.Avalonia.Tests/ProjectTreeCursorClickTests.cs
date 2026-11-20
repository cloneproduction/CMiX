using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // A key acts on the cursor. A click on a row moves the cursor to that row, also when the row is already
    // selected. A click on the chevron or on a toggle of a row does not move the cursor.
    public class ProjectTreeCursorClickTests
    {
        private static (Scene Scene, TreeTestScreen Screen) Show()
        {
            var scene = CreateScene();
            foreach (var node in scene.All())
                node.IsExpanded = true;
            var screen = TreeTestScreen.Show(scene.Tree, 400, 1100);
            return (scene, screen);
        }

        private static void Click(TreeTestScreen screen, Point point)
        {
            screen.Render();
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            screen.Settle();
        }

        // A point on the row of the node, left of the toggles that end the row.
        private static Point RowPoint(TreeTestScreen screen, TreeNode node)
        {
            var row = TreeTestScreen.RowOf(screen.View, node);
            return row.TranslatePoint(new Point(row.Bounds.Width - 80, row.Bounds.Height / 2), screen.Window)!.Value;
        }

        private static Point Middle(TreeTestScreen screen, Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), screen.Window)!.Value;

        private static ToggleButton ChevronOf(TreeTestScreen screen, TreeNode node) =>
            TreeTestScreen.ItemOf(screen.View, node).GetVisualDescendants().OfType<ToggleButton>()
                .First(toggle => toggle.Name == "PART_ExpandCollapseChevron");

        private static ToggleButton FirstToggleOf(TreeTestScreen screen, TreeNode node) =>
            TreeTestScreen.ItemOf(screen.View, node).GetVisualDescendants().OfType<TogglesRow>().First()
                .GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.IsEffectivelyVisible);

        [AvaloniaFact]
        public void Click_TheSelectedRowAfterDown_PutsTheCursorThere()
        {
            var (scene, screen) = Show();
            using var _ = screen;
            var selected = scene.LayerNodeOfA(0);
            scene.Tree.SelectedNode = selected;
            scene.Tree.MoveCursor(2);
            Assert.NotSame(selected, scene.Tree.CursorNode);

            Click(screen, RowPoint(screen, selected));

            Assert.Same(selected, scene.Tree.SelectedNode);
            Assert.Same(selected, scene.Tree.CursorNode);
            Assert.Same(selected, scene.Tree.TargetNode);
        }

        [AvaloniaFact]
        public void Click_TheChevronOfTheSelectedRow_KeepsTheCursor()
        {
            var (scene, screen) = Show();
            using var _ = screen;
            var selected = scene.LayerNodeOfA(0);
            scene.Tree.SelectedNode = selected;
            var cursor = scene.LayerNodeOfA(1);
            scene.Tree.CursorNode = cursor;

            Click(screen, Middle(screen, ChevronOf(screen, selected)));

            Assert.False(selected.IsExpanded);
            Assert.Same(cursor, scene.Tree.CursorNode);
            Assert.Same(selected, scene.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_AToggleOfTheSelectedRow_KeepsTheCursor()
        {
            var (scene, screen) = Show();
            using var _ = screen;
            var selected = scene.LayerNodeOfA(0);
            scene.Tree.SelectedNode = selected;
            var cursor = scene.LayerNodeOfA(1);
            scene.Tree.CursorNode = cursor;
            var toggle = FirstToggleOf(screen, selected);
            var before = toggle.IsChecked;

            Click(screen, Middle(screen, toggle));

            Assert.NotEqual(before, toggle.IsChecked);
            Assert.Same(cursor, scene.Tree.CursorNode);
            Assert.Same(selected, scene.Tree.SelectedNode);
        }
    }
}
