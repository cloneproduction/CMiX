// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The pointer drag of a row of the new tree: the start, the pick, the cancel and the drop. The points are in the panel of the rows.
    public class ListTreeRowDragTests : IDisposable
    {
        private const double PressX = 120;
        private const double PixelsPerLevel = 24;

        private readonly Scene scene;
        private readonly Window window;
        private readonly ListTreeProject view;

        private double pressY;
        private Point last;

        // Layer 1 of A has two entities, a filter and modifiers. Layer 2 of A is empty. Every card with rows is open.
        public ListTreeRowDragTests()
        {
            scene = CreateScene([2, 1], s => AddKinds(s.Layer));
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose() => window.Close();

        private PanelListTreeRows Panel => view.Rows;

        private ListTreeDrag Drag => Assert.IsType<ListTreeDrag>(view.RowDrag.Drag);

        private ListTreeCandidate Picked => Drag.Look!.Candidate;

        private TreeNode Entity(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private TreeNode Modifier(int index) => scene.Group<ModifiersGroup>(0).Children[index];

        private Point InWindow(double x, double y) => Panel.TranslatePoint(new Point(x, y), window)!.Value;

        private void Press(TreeNode node)
        {
            TreeDrag.Render(window);
            pressY = Panel.Layout.PlaceOf(ListTreeKeys.Of(node))!.Y + 16;
            window.MouseDown(InWindow(PressX, pressY), MouseButton.Left);
        }

        private void MoveTo(double x, double y)
        {
            last = InWindow(x, y);
            TreeDrag.MoveTo(window, last);
        }

        private void Start(TreeNode node)
        {
            Press(node);
            MoveTo(PressX, pressY + 8);
        }

        // The point where the candidate is at the distance zero.
        private double XOf(int depth) => PressX + (depth - Drag.Press.Depth) * PixelsPerLevel;

        private double YOf(ListTreeCandidate candidate) => candidate.Place.Y + Drag.Press.GrabOffsetY;

        private void MoveTo(ListTreeCandidate candidate) => MoveTo(XOf(candidate.Place.Depth), YOf(candidate));

        private void Release() => TreeDrag.Release(window, last);

        private ListTreeCandidate InsideOf(TreeNode card) =>
            Drag.Candidates.First(candidate => candidate.Slot.Position == DropPosition.Inside && ReferenceEquals(candidate.Slot.ShownTarget.Model, card.Item));

        private void AssertRowsAt(ListTreeLayout layout) =>
            Assert.All(layout.Places, place => Assert.Equal(place.Y, PanelListTreeRows.GetTop(Assert.IsType<RowListTree>(Panel.RowOf(place.Key)))));

        [AvaloniaFact]
        public void Move_PastTheThreshold_StartsTheDragAndShowsTheDraggedRow()
        {
            Start(Entity(0));

            Assert.True(scene.Tree.IsDragging);
            Assert.Contains(RowListTree.DraggedClass, Panel.RowOf(Drag.Look!.Dragged)!.Classes);
            Assert.True(view.DragNote.IsVisible);
        }

        [AvaloniaFact]
        public void Move_InsideTheThreshold_StartsNoDrag()
        {
            Press(Entity(0));
            MoveTo(PressX + 2, pressY + 2);

            Assert.False(view.RowDrag.IsDragging);
            Assert.False(scene.Tree.IsDragging);
        }

        [AvaloniaFact]
        public void Move_OverACandidate_PutsTheRowsAtTheirPlacesInItAndNamesTheTarget()
        {
            Start(Entity(0));
            var target = InsideOf(scene.LayerNodeOfA(1));

            MoveTo(target);

            Assert.Same(target, Picked);
            AssertRowsAt(target.Layout);
            Assert.Equal($"Move into {scene.LayerNodeOfA(1).Header}. Grey rows can not take it.", view.DragNote.Text);
        }

        [AvaloniaFact]
        public void Move_ToTheXOfALevel_PicksACandidateAtThatDepth()
        {
            Start(Modifier(0));
            var pair = Drag.Candidates.SelectMany(a => Drag.Candidates.Where(b => b.Place.Depth > a.Place.Depth), (a, b) => new { A = a, B = b })
                .MinBy(pair => Math.Abs(pair.A.Place.Y - pair.B.Place.Y))!;
            var y = (YOf(pair.A) + YOf(pair.B)) / 2;

            MoveTo(XOf(pair.A.Place.Depth), y);
            var shallow = Picked.Place.Depth;
            MoveTo(XOf(pair.B.Place.Depth), y);

            Assert.Equal(pair.A.Place.Depth, shallow);
            Assert.Equal(pair.B.Place.Depth, Picked.Place.Depth);
        }

        [AvaloniaFact]
        public void Move_LessThanTheHysteresisTowardAnotherCandidate_KeepsTheCurrent()
        {
            Start(Entity(0));
            var current = InsideOf(scene.LayerNodeOfA(1));
            var next = Drag.Candidates.Where(candidate => candidate.Place.Depth == current.Place.Depth && candidate.Place.Y != current.Place.Y)
                .MinBy(candidate => Math.Abs(candidate.Place.Y - current.Place.Y))!;
            var half = (next.Place.Y - current.Place.Y) / 2;
            var toward = Math.Sign(half);
            MoveTo(current);

            MoveTo(XOf(current.Place.Depth), YOf(current) + half + toward * 2);
            var kept = Picked.Place.Y;
            MoveTo(XOf(current.Place.Depth), YOf(current) + half + toward * 4);

            Assert.Equal(current.Place.Y, kept);
            Assert.Equal(next.Place.Y, Picked.Place.Y);
        }

        [AvaloniaFact]
        public void Move_OverTheRows_ShowsNoHoverAndNoSelection()
        {
            Start(Entity(0));

            MoveTo(PressX, Panel.Layout.PlaceOf(ListTreeKeys.Of(scene.Group<ModifiersGroup>(0)))!.Y + 16);

            Assert.DoesNotContain(Panel.Children.OfType<RowListTree>(), row => row.IsPointerOver);
            Assert.NotNull(scene.Tree.SelectedNode);
            Assert.DoesNotContain(Panel.Children.OfType<RowListTree>(), row => row.IsSelected);
        }

        [AvaloniaFact]
        public void Escape_DuringADrag_PutsEveryRowBackAndMovesNothing()
        {
            var before = Panel.Layout;
            Start(Entity(0));
            MoveTo(InsideOf(scene.LayerNodeOfA(1)));

            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            TreeTestScreen.Settle(window);

            Assert.False(scene.Tree.IsDragging);
            Assert.False(view.DragNote.IsVisible);
            AssertRowsAt(before);
            Assert.Equal(2, scene.Layer.ModelEntityManager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void Release_OnACandidate_MovesTheItemSelectsItAndMakesOneUndoStep()
        {
            var entity = Entity(0).Item!;
            Start(Entity(0));
            MoveTo(InsideOf(scene.LayerNodeOfA(1)));

            Release();
            TreeTestScreen.Settle(window);

            Assert.False(scene.Tree.IsDragging);
            Assert.Contains(entity, scene.LayerOfA(1).ModelEntityManager.ManagerData.Items);
            Assert.Same(entity, scene.Tree.SelectedNode?.Item);
            AssertRowsAt(Panel.Layout);

            scene.Undo.Undo();

            Assert.Contains(entity, scene.Layer.ModelEntityManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Release_AtTheOwnPlace_MovesNothing()
        {
            var entities = scene.Layer.ModelEntityManager.ManagerData.Items.ToArray();
            Start(Entity(0));

            Release();

            Assert.False(scene.Tree.IsDragging);
            Assert.Equal(entities, scene.Layer.ModelEntityManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void CancelDrag_OfTheTree_EndsTheDragOfTheView()
        {
            Start(Entity(0));

            scene.Tree.Views.CancelDrag();

            Assert.False(view.RowDrag.IsDragging);
            Assert.False(scene.Tree.IsDragging);
        }

        [AvaloniaFact]
        public void CaptureLost_DuringADrag_CancelsTheDrag()
        {
            IPointer? pointer = null;
            window.AddHandler(InputElement.PointerMovedEvent, (_, e) => pointer = e.Pointer, handledEventsToo: true);
            Start(Entity(0));

            pointer!.Capture(null);

            Assert.False(view.RowDrag.IsDragging);
            Assert.False(scene.Tree.IsDragging);
        }
    }
}
