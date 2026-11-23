// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using CMiX.Core;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    // A drag of several selected rows of the new list tree: the rows move as one block and one undo step.
    public class ListTreeBlockDragTests : IDisposable
    {
        private const double PressX = 120;

        private readonly Scene scene = CreateScene([2], AddEntities);
        private readonly Window window;
        private readonly ListTreeProject view;
        private Point last;

        // Layer 1 of A has four entities, layer 2 of A has one. Every card is open.
        public ListTreeBlockDragTests()
        {
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            scene.Tree.SelectedNode = null;
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose()
        {
            view.Grab.DetachFromWindow();
            window.Close();
        }

        private static void AddEntities(Scene scene)
        {
            for (var index = 0; index < 4; index++)
                scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
        }

        private ProjectTree Tree => scene.Tree;

        private PanelListTreeRows Panel => view.Rows;

        private ListTreeDrag Drag => Assert.IsType<ListTreeDrag>(view.RowDrag.Drag);

        private TreeNode EntityAt(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private TreeNode EntityOfLayer2 => scene.Group<EntitiesGroup>(1).Children[0];

        private IReadOnlyList<IControl> ItemsOfLayer(int index) => scene.LayerOfA(index).ModelEntityManager.ManagerData.Items;

        private void Settle() => TreeTestScreen.Settle(window);

        private Point InWindow(double x, double y) => Panel.TranslatePoint(new Point(x, y), window)!.Value;

        private double YOf(TreeNode node) => Panel.Layout.PlaceOf(ListTreeKeys.Of(node))!.Y + 16;

        private void Click(TreeNode node, RawInputModifiers modifiers)
        {
            TreeDrag.Render(window);
            var point = InWindow(PressX, YOf(node));
            window.MouseDown(point, MouseButton.Left, modifiers);
            window.MouseUp(point, MouseButton.Left, modifiers);
            Settle();
        }

        private void MoveTo(double x, double y)
        {
            last = InWindow(x, y);
            TreeDrag.MoveTo(window, last);
        }

        private void Press(TreeNode node)
        {
            TreeDrag.Render(window);
            var y = YOf(node);
            window.MouseDown(InWindow(PressX, y), MouseButton.Left);
            MoveTo(PressX, y + 8);
        }

        private void MoveOnto(ListTreeCandidate candidate) =>
            MoveTo(PressX + (candidate.Place.Depth - Drag.Press.Depth) * 24, candidate.Place.Y + Drag.Press.GrabOffsetY);

        private void Release()
        {
            TreeDrag.Release(window, last);
            Settle();
        }

        private static bool IsBefore(ListTreeCandidate candidate, TreeNode node) =>
            ReferenceEquals(candidate.Slot.Target, node) && candidate.Slot.Position == DropPosition.Before;

        // Entities 1, 2 and 4 of layer 1, and entity 2 is the selected node.
        private IControl[] SelectThreeEntities()
        {
            Click(EntityAt(0), RawInputModifiers.None);
            Click(EntityAt(3), RawInputModifiers.Control);
            Click(EntityAt(1), RawInputModifiers.Control);
            scene.ClearUndo();
            return new[] { EntityAt(0).Item!, EntityAt(1).Item!, EntityAt(3).Item! };
        }

        [AvaloniaFact]
        public void Release_ABlockOfThreeEntitiesInAnotherLayer_LandsThemInTreeOrderAsOneUndoStep()
        {
            var block = SelectThreeEntities();
            var kept = EntityAt(2).Item!;
            var other = EntityOfLayer2.Item!;

            Press(EntityAt(1));
            MoveOnto(Drag.Candidates.First(candidate => IsBefore(candidate, EntityOfLayer2)));
            Release();

            Assert.Equal(block.Append(other), ItemsOfLayer(1));
            Assert.Equal(new[] { kept }, ItemsOfLayer(0));
            Assert.Equal(block, Tree.SelectedNodes.Select(node => node.Item!));
            Assert.Same(block[1], Tree.SelectedNode!.Item);
            Assert.All(Tree.SelectedNodes, node => Assert.Contains(RowListTree.FlashClass, Panel.RowOf(node)!.Classes));

            scene.Undo.Undo();

            Assert.Equal(new[] { block[0], block[1], kept, block[2] }, ItemsOfLayer(0));
            Assert.Equal(new[] { other }, ItemsOfLayer(1));
            Assert.False(scene.Undo.CanUndo);
        }

        [AvaloniaFact]
        public void Move_ABlockDuringTheDrag_ShowsEveryRowOfTheBlockAsDraggedOneAfterTheOther()
        {
            SelectThreeEntities();

            Press(EntityAt(1));
            var candidate = Drag.Candidates.First(candidate => IsBefore(candidate, EntityOfLayer2));
            MoveOnto(candidate);

            var look = Drag.Look!;
            var dragged = look.Rows.Where(row => look.LightOf(row.Key) == ListTreeRowLight.Dragged).Select(row => look.Layout.PlaceOf(row.Key)!.Y).ToArray();
            var pitch = ListTreeSizes.Default.Pitch;
            Assert.Equal(new[] { candidate.Place.Y - pitch, candidate.Place.Y, candidate.Place.Y + pitch }, dragged);
            view.RowDrag.Cancel();
        }

        [AvaloniaFact]
        public void Press_ASetOfRowsOfTwoKinds_DragsOnlyThePressedRow()
        {
            Click(EntityAt(0), RawInputModifiers.None);
            Click(scene.LayerNodeOfA(1), RawInputModifiers.Control);
            Click(EntityAt(2), RawInputModifiers.Control);
            var moved = EntityAt(2).Item!;

            Press(EntityAt(2));
            Assert.False(Drag.Nodes.IsBlock);
            MoveOnto(Drag.Candidates.First(candidate => IsBefore(candidate, EntityOfLayer2)));
            Release();

            Assert.Equal(new[] { moved, ItemsOfLayer(1)[1] }, ItemsOfLayer(1));
            Assert.Equal(3, ItemsOfLayer(0).Count);
            Assert.Equal(new[] { Tree.SelectedNode }, Tree.SelectedNodes);
        }

        [AvaloniaFact]
        public void Confirm_AGrabOfABlock_MovesTheBlockAsOneUndoStep()
        {
            var block = SelectThreeEntities();

            view.Grab.Start(Tree.SelectedNode!);
            var grabbed = view.Grab.Drag!;
            Assert.True(grabbed.Nodes.IsBlock);
            var target = grabbed.Candidates.First(candidate => IsBefore(candidate, EntityOfLayer2));
            for (var step = 0; step < grabbed.Candidates.Count && !ReferenceEquals(grabbed.Look?.Candidate, target); step++)
                view.Grab.StepDown();
            view.Grab.Confirm();
            Settle();

            Assert.Equal(block.Append(ItemsOfLayer(1)[3]), ItemsOfLayer(1));
            Assert.Same(block[1], Tree.SelectedNode!.Item);
            scene.Undo.Undo();
            Assert.Equal(4, ItemsOfLayer(0).Count);
            Assert.False(scene.Undo.CanUndo);
        }
    }
}
