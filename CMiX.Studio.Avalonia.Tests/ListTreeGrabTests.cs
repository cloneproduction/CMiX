// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Behaviors.ListTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Layer = CMiX.Core.Compositing.Layer;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // Tree.Grab (G) steps through the candidates of the pointer drag with the arrow keys and drops with Enter.
    public class ListTreeGrabTests : IDisposable
    {
        private readonly TestWindows windows = new();
        private ListTreeProject? view;

        // A grab that a test leaves running would take the input of the next tests, so it ends here first.
        public void Dispose() => CloseWindow();

        private void CloseWindow()
        {
            view?.Grab.DetachFromWindow();
            windows.Dispose();
            view = null;
        }

        private sealed record Screen : ListTreeMainScreen
        {
            public Screen(ListTreeMainScreen shown) : base(shown) { }

            public Layer[] Layers => Enumerable.Range(0, 3).Select(Layer).ToArray();
            public int UndoSteps => UndoCounts.StepsOf(Undo);
        }

        // The order of the layers after the keys, as the original indices of the layers.
        private sealed record Outcome(int[] Order, int UndoSteps);

        // One composition with three empty layers, in the tree of the left panel. The layer rows are closed. The composition is selected.
        private Screen Show()
        {
            var screen = new Screen(ListTreeMainScreen.Show(windows, panel: PanelChoice.Tree));
            foreach (var panelCollapse in screen.Window.GetVisualDescendants().OfType<CMiX.UI.Panels.PanelCollapse>())
                panelCollapse.Duration = TimeSpan.Zero;
            for (var i = 0; i < 3; i++)
                screen.LayerNode(i).IsExpanded = false;
            screen.Tree.SelectedNode = screen.CompositionNode;
            Settle(screen.Window);
            screen.Undo.Clear();
            return screen;
        }

        private Screen ShowNewTree()
        {
            var screen = Show();
            view = screen.View;
            Settle(screen.Window);
            return screen;
        }

        private ListTreeProject View => view!;

        private static void Press(Screen screen, string keys)
        {
            foreach (var key in keys.Split(' '))
                screen.Window.Press(key == "Down" ? PhysicalKey.ArrowDown : key == "Up" ? PhysicalKey.ArrowUp : PhysicalKey.Enter);
        }

        // The middle of the row of the node in the tree, in the window.
        private Point On(Screen screen, TreeNode node)
        {
            TreeDrag.Render(screen.Window);
            var row = View.Rows.RowOf(node)!;
            return row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), screen.Window)!.Value;
        }

        // The pointer rests on the composition row first, so the grab starts at a known place.
        private void GrabFirstLayer(Screen screen)
        {
            screen.Window.MouseMove(On(screen, screen.CompositionNode));
            screen.Tree.CursorNode = screen.LayerNode(0);
            Settle(screen.Window);
            View.Rows.RowOf(screen.LayerNode(0))!.Focus();
            Settle(screen.Window);
            screen.Window.Press(PhysicalKey.G);
            Assert.True(View.Grab.IsGrabbing);
        }

        private Outcome NewGrab(string keys)
        {
            var screen = ShowNewTree();
            var before = screen.Layers;
            GrabFirstLayer(screen);

            Press(screen, keys);

            return OutcomeOf(screen, before);
        }

        private static Outcome OutcomeOf(Screen screen, Layer[] before) =>
            new(screen.Layers.Select(layer => Array.IndexOf(before, layer)).ToArray(), screen.UndoSteps);

        // The own place of the layer and the places that leave it where it is are no candidates. The candidates are the start, the place before the last layer and the end.
        [AvaloniaTheory]
        [InlineData("Down Enter", new[] { 1, 0, 2 }, 1)]
        [InlineData("Down Down Enter", new[] { 1, 2, 0 }, 1)]
        [InlineData("Down Down Down Enter", new[] { 1, 2, 0 }, 1)]
        [InlineData("Down Down Down Up Enter", new[] { 1, 0, 2 }, 1)]
        [InlineData("Down Down Down Down Down Enter", new[] { 1, 2, 0 }, 1)]
        [InlineData("Up Enter", new[] { 0, 1, 2 }, 0)]
        public void Enter_AfterTheKeys_GivesTheLayerOrderOfTheShownPlace(string keys, int[] order, int undoSteps)
        {
            var outcome = NewGrab(keys);

            Assert.Equal(order, outcome.Order);
            Assert.Equal(undoSteps, outcome.UndoSteps);
        }

        [AvaloniaFact]
        public void G_OnALayer_StartsTheGrabWithTheFrameTheGrabHintsAndTheRowAtItsPlace()
        {
            var screen = ShowNewTree();

            GrabFirstLayer(screen);

            Assert.True(screen.Tree.IsDragging);
            Assert.True(View.Classes.Contains(ListTreeGrab.GrabbingClass));
            Assert.True(View.GetVisualDescendants().OfType<Border>().Single(border => border.Name == "grabFrame").IsEffectivelyVisible);
            Assert.Equal(TreeHintMode.Grab, View.HintMode);
            Assert.True(View.Grab.Drag!.IsAtItsOwnPlace());
            Assert.Contains(RowListTree.DraggedClass, View.Rows.RowOf(screen.LayerNode(0))!.Classes);
        }

        [AvaloniaFact]
        public void Escape_DuringTheGrab_EndsItAndMovesNothing()
        {
            var screen = ShowNewTree();
            var before = screen.Layers;
            GrabFirstLayer(screen);
            screen.Window.Press(PhysicalKey.ArrowDown);

            screen.Window.Press(PhysicalKey.Escape);

            Assert.False(View.Grab.IsGrabbing);
            Assert.False(screen.Tree.IsDragging);
            Assert.Equal(before, screen.Layers);
            Assert.Equal(0, screen.UndoSteps);
            Assert.False(View.Classes.Contains(ListTreeGrab.GrabbingClass));
            Assert.Equal(TreeHintMode.Idle, View.HintMode);
            Assert.DoesNotContain(View.Rows.Children.OfType<RowListTree>(), row => row.Classes.Contains(RowListTree.DraggedClass));
        }

        [AvaloniaFact]
        public void Down_DuringTheGrab_ShowsTheNextCandidateInTreeOrder()
        {
            var screen = ShowNewTree();
            GrabFirstLayer(screen);
            var drag = View.Grab.Drag!;
            var start = drag.Candidates.ToList().FindIndex(candidate => ReferenceEquals(candidate.Slot.Target, screen.LayerNode(0)));

            screen.Window.Press(PhysicalKey.ArrowDown);

            Assert.Equal(drag.Candidates[start + 1].Slot, drag.Look!.Candidate.Slot);
            Assert.Equal(drag.Candidates[start + 1].Place.Y, PanelListTreeRows.GetTop(View.Rows.RowOf(screen.LayerNode(0))!));
        }

        [AvaloniaFact]
        public void Enter_AfterDown_DropsTheLayerAsOneUndoStepAndSelectsIt()
        {
            var screen = ShowNewTree();
            var before = screen.Layers;
            var grabbed = screen.LayerNode(0).Item;
            GrabFirstLayer(screen);
            screen.Window.Press(PhysicalKey.ArrowDown);
            screen.Window.Press(PhysicalKey.ArrowDown);

            screen.Window.Press(PhysicalKey.Enter);

            Assert.False(View.Grab.IsGrabbing);
            Assert.NotEqual(before, screen.Layers);
            Assert.Equal(1, screen.UndoSteps);
            Assert.Same(grabbed, screen.Tree.SelectedNode!.Item);
            screen.Undo.Undo();
            Assert.Equal(before, screen.Layers);
        }

        [AvaloniaFact]
        public void Enter_AtTheStartPlace_EndsTheGrabWithNoMove()
        {
            var screen = ShowNewTree();
            var before = screen.Layers;
            GrabFirstLayer(screen);

            screen.Window.Press(PhysicalKey.Enter);

            Assert.False(View.Grab.IsGrabbing);
            Assert.Equal(before, screen.Layers);
            Assert.Equal(0, screen.UndoSteps);
            Assert.Same(screen.CompositionNode, screen.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_AfterALongMoveOfThePointer_PutsTheLayerAtThePlaceUnderThePointer()
        {
            var screen = ShowNewTree();
            var before = screen.Layers;
            GrabFirstLayer(screen);
            var target = On(screen, screen.LayerNode(2));

            screen.Window.MouseMove(target);
            Settle(screen.Window);
            Assert.Equal(3, View.Grab.Drag!.Look!.Candidate.Slot.ListPlace.Index);
            screen.Window.MouseDown(target, MouseButton.Left);
            screen.Window.MouseUp(target, MouseButton.Left);
            Settle(screen.Window);

            Assert.False(View.Grab.IsGrabbing);
            Assert.False(View.RowDrag.IsDragging);
            Assert.Equal(new[] { before[1], before[2], before[0] }, screen.Layers);
            Assert.Equal(1, screen.UndoSteps);
        }

        [AvaloniaFact]
        public void ASmallMoveOfThePointer_AfterAKey_KeepsTheMarkOfTheKey()
        {
            var screen = ShowNewTree();
            GrabFirstLayer(screen);
            screen.Window.Press(PhysicalKey.ArrowDown);
            var marked = View.Grab.Drag!.Look!.Candidate.Slot;
            var start = On(screen, screen.CompositionNode);

            screen.Window.MouseMove(start + new Vector(0, ListTreeGrab.MouseTakeOver / 2));
            Settle(screen.Window);

            Assert.Equal(marked, View.Grab.Drag!.Look!.Candidate.Slot);
        }

        [AvaloniaFact]
        public void RightClick_DuringTheGrab_EndsItAndOpensNoMenu()
        {
            var screen = ShowNewTree();
            var before = screen.Layers;
            GrabFirstLayer(screen);
            var point = On(screen, screen.LayerNode(2));

            screen.Window.MouseDown(point, MouseButton.Right);
            screen.Window.MouseUp(point, MouseButton.Right);
            Settle(screen.Window);

            Assert.False(View.Grab.IsGrabbing);
            Assert.Equal(before, screen.Layers);
            Assert.NotEqual(true, View.Menu?.IsOpen);
        }

        [AvaloniaFact]
        public void ContextRequested_DuringTheGrab_OpensNoMenuAndTheGrabGoesOn()
        {
            var screen = ShowNewTree();
            GrabFirstLayer(screen);
            var row = View.Rows.RowOf(screen.LayerNode(2))!;
            var request = new ContextRequestedEventArgs { Source = row };

            row.RaiseEvent(request);
            Settle(screen.Window);

            Assert.True(request.Handled);
            Assert.NotEqual(true, View.Menu?.IsOpen);
            Assert.True(View.Grab.IsGrabbing);
        }
    }
}
