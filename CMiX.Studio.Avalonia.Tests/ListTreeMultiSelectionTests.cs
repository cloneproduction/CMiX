// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    // Ctrl and Shift clicks of the new list tree: the selected rows, the selected node, and their look.
    public class ListTreeMultiSelectionTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], AddEntities);
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeMultiSelectionTests()
        {
            foreach (var node in scene.All())
                node.IsExpanded = true;
            scene.Tree.SelectedNode = null;
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose() => window.Close();

        private static void AddEntities(Scene scene)
        {
            for (var index = 0; index < 4; index++)
                scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
        }

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode EntityAt(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private TreeNode EntityOfLayer2 => scene.Group<EntitiesGroup>(1).Children[0];

        private ProjectTree Tree => scene.Tree;

        private void Settle() => TreeTestScreen.Settle(window);

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(ListTreeKeys.Of(node)));

        private FrameListTreeCard AreaOf(TreeNode node) => Assert.IsType<FrameListTreeCard>(view.Rows.Surfaces.CardAreaOf(ListTreeKeys.Of(node)));

        private static Color FillOf(IBrush? brush) => brush is ISolidColorBrush solid ? solid.Color : Colors.Transparent;

        private static Color Token(string key) => FillOf((IBrush)Application.Current!.FindResource(key)!);

        private static Border Named(Control root, string name) => root.GetVisualDescendants().OfType<Border>().First(border => border.Name == name);

        private static Color TextOf(RowListTree row) =>
            FillOf(row.GetVisualDescendants().OfType<TextBoxEditable>().First(box => box.IsVisible).Foreground);

        private static ListTreeKindColors KindOf(TreeNode node) => CategoryPalette.FromApplication().ForListTree(node.Category!.Value, node.ComposableDepth);

        private Point BlankOf(RowListTree row) => row.TranslatePoint(new Point(row.Bounds.Width - 100, row.Bounds.Height / 2), window)!.Value;

        private Point CenterOf(Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        private void Click(TreeNode node, RawInputModifiers modifiers = RawInputModifiers.None) => ClickAt(BlankOf(RowOf(node)), modifiers);

        private void ClickAt(Point point, RawInputModifiers modifiers)
        {
            TreeDrag.Render(window);
            window.MouseDown(point, MouseButton.Left, modifiers);
            window.MouseUp(point, MouseButton.Left, modifiers);
            Settle();
        }

        [AvaloniaFact]
        public void Click_CtrlOnTwoMoreRows_SelectsAllThreeAndTheLastIsTheSelectedNode()
        {
            Click(EntityAt(0));
            Click(EntityAt(2), RawInputModifiers.Control);
            Click(EntityOfLayer2, RawInputModifiers.Control);

            Assert.Equal(new[] { EntityAt(0), EntityAt(2), EntityOfLayer2 }, Tree.SelectedNodes);
            Assert.Same(EntityOfLayer2, Tree.SelectedNode);
            Assert.Same(EntityOfLayer2, Tree.CursorNode);
        }

        [AvaloniaFact]
        public void Click_CtrlOnTheSelectedNodeOfASet_TakesItOutAndTheRowBeforeItBecomesTheSelectedNode()
        {
            Click(EntityAt(0));
            Click(EntityAt(1), RawInputModifiers.Control);
            Click(EntityAt(3), RawInputModifiers.Control);

            Click(EntityAt(3), RawInputModifiers.Control);

            Assert.Equal(new[] { EntityAt(0), EntityAt(1) }, Tree.SelectedNodes);
            Assert.Same(EntityAt(1), Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_CtrlOnTheOnlySelectedRow_KeepsTheRowSelected()
        {
            Click(EntityAt(1));

            Click(EntityAt(1), RawInputModifiers.Control);

            Assert.Equal(new[] { EntityAt(1) }, Tree.SelectedNodes);
            Assert.Same(EntityAt(1), Tree.SelectedNode);
            Assert.Same(EntityAt(1), Tree.CursorNode);
        }

        [AvaloniaFact]
        public void Click_ShiftAfterAPlainClick_SelectsTheShownRowsFromTheAnchor()
        {
            Click(EntityAt(3));

            Click(EntityAt(1), RawInputModifiers.Shift);

            Assert.Equal(new[] { EntityAt(1), EntityAt(2), EntityAt(3) }, Tree.SelectedNodes);
            Assert.Same(EntityAt(1), Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_ShiftAfterACtrlClick_StartsTheRangeAtTheCtrlClickedRow()
        {
            Click(EntityAt(0));
            Click(EntityAt(1), RawInputModifiers.Control);

            Click(EntityAt(3), RawInputModifiers.Shift);

            Assert.Equal(new[] { EntityAt(1), EntityAt(2), EntityAt(3) }, Tree.SelectedNodes);
            Assert.Same(EntityAt(3), Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_APlainClickOnARowOfASet_SelectsOnlyThatRow()
        {
            Click(EntityAt(0));
            Click(EntityAt(2), RawInputModifiers.Control);

            Click(EntityAt(0));

            Assert.Equal(new[] { EntityAt(0) }, Tree.SelectedNodes);
            Assert.Same(EntityAt(0), Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_TheIconOfACard_KeepsTheSet()
        {
            Click(EntityAt(0));
            Click(EntityAt(2), RawInputModifiers.Control);

            ClickAt(CenterOf(RowOf(scene.LayerNodeOfA(1)).IconHit), RawInputModifiers.None);

            Assert.Equal(new[] { EntityAt(0), EntityAt(2) }, Tree.SelectedNodes);
        }

        [AvaloniaFact]
        public void Show_ARowOfTheSetThatIsNotTheSelectedNode_HasTheSelectedColours()
        {
            Click(EntityAt(0));
            Click(EntityAt(2), RawInputModifiers.Control);
            var row = RowOf(EntityAt(0));

            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(row.Background));
            Assert.Equal(KindOf(EntityAt(0)).SelectionBorder, FillOf(Named(row, "mark").Background));
            Assert.True(Named(row, "mark").IsEffectivelyVisible);
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
            Assert.Equal(Colors.Transparent, FillOf(RowOf(EntityAt(1)).Background));
        }

        [AvaloniaFact]
        public void Show_ACardOfTheSet_LightsItsArea()
        {
            Click(Layer);
            Click(EntityOfLayer2, RawInputModifiers.Control);

            Assert.Same(EntityOfLayer2, Tree.SelectedNode);
            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(AreaOf(Layer).Surface.Background));
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(RowOf(EntityAt(0))));
        }

        [AvaloniaFact]
        public void SelectedNode_SetFromOutside_LeavesOnlyTheSelectedNode()
        {
            Click(EntityAt(0));
            Click(EntityAt(2), RawInputModifiers.Control);

            Tree.SelectedNode = EntityAt(3);
            Settle();

            Assert.Equal(new[] { EntityAt(3) }, Tree.SelectedNodes);
            Assert.Equal(Colors.Transparent, FillOf(RowOf(EntityAt(0)).Background));
        }

        [AvaloniaFact]
        public void AddToSelection_TwoRows_LeavesTheOldViewWithOnlyTheSelectedNode()
        {
            using var old = TreeTestScreen.Show(Tree, 400, 900);
            Click(EntityAt(0));

            Tree.AddToSelection(EntityAt(2));
            old.Settle();

            var selected = old.View.GetVisualDescendants().OfType<TreeViewItem>().Where(item => item.IsSelected).Select(item => item.DataContext);
            Assert.Equal(new object[] { EntityAt(2) }, selected);
            Assert.Equal(2, Tree.SelectedNodes.Count);
        }

        [AvaloniaFact]
        public void SelectedNode_SetByThePeerThroughTheList_LeavesOnlyTheSelectedNode()
        {
            Click(EntityAt(0));
            Click(EntityAt(2), RawInputModifiers.Control);

            scene.Layer.ModelEntityManager.SelectedItem = EntityAt(1).Item;

            Assert.Equal(new[] { EntityAt(1) }, Tree.SelectedNodes);
        }
    }
}
