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
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeLook;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    // Ctrl and Shift clicks of the list tree: the selected rows, the selected node, and their look.
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

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(ListTreeKeys.Of(node)));

        private FrameListTreeCard AreaOf(TreeNode node) => Assert.IsType<FrameListTreeCard>(view.Rows.Surfaces.CardAreaOf(ListTreeKeys.Of(node)));

        private Point BlankOf(RowListTree row) => row.TranslatePoint(new Point(row.Bounds.Width - 100, row.Bounds.Height / 2), window)!.Value;

        private void Click(TreeNode node, RawInputModifiers modifiers = RawInputModifiers.None) =>
            window.Click(BlankOf(RowOf(node)), modifiers: modifiers);

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

            window.Click(RowOf(scene.LayerNodeOfA(1)).IconHit);

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
            window.Settle();

            Assert.Equal(new[] { EntityAt(3) }, Tree.SelectedNodes);
            Assert.Equal(Colors.Transparent, FillOf(RowOf(EntityAt(0)).Background));
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
