// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The pointer drag of a row of the new tree. The scroller captures the pointer, so the moves and the release come on every platform.
    internal sealed class ListTreeRowDrag : ListTreeBehavior
    {
        public static readonly TimeSpan RestDelay = TimeSpan.FromMilliseconds(500);

        private readonly ListTreePick pick = new(ListTreeDragNumbers.Default);
        private readonly Cursor moveCursor = new(TreeDragSession.MoveCursorType);
        private readonly DispatcherTimer restTimer = new() { Interval = RestDelay };

        private ProjectTree? tree;
        private PressedRow? pressed;
        private ListTreeDrag? drag;
        private IPointer? pointer;
        private Cursor? cursorBefore;
        private ListTreePoint lastPoint = new(0, 0);
        private Point lastInScroller;
        private TreeNode? resting;

        public bool IsDragging => drag != null;

        internal ListTreeDrag? Drag => drag;

        internal TreeNode? RestingNode => resting;

        internal bool IsRestTimerRunning => restTimer.IsEnabled;

        private PanelListTreeRows Rows => Context.Rows;

        private ScrollViewer Scroller => Context.Scroller;

        private ListTreeAutoScroll AutoScroll => Part<ListTreeAutoScroll>();

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            restTimer.Tick += OnRested;
            Scroller.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Bubble, handledEventsToo: true);
            Scroller.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Bubble, handledEventsToo: true);
            Scroller.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);
            Scroller.AddHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost);
            Scroller.AddHandler(InputElement.KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);
            Scroller.ScrollChanged += OnScrollChanged;
        }

        protected override void OnUnwatch()
        {
            Cancel();
            Scroller.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            Scroller.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
            Scroller.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            Scroller.RemoveHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost);
            Scroller.RemoveHandler(InputElement.KeyDownEvent, OnKeyDownTunnel);
            Scroller.ScrollChanged -= OnScrollChanged;
            restTimer.Tick -= OnRested;
            tree = null;
        }

        // Every row goes back to its place, and nothing moves.
        public void Cancel()
        {
            pressed = null;
            if (drag != null)
                Finish();
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressed = null;
            if (tree is not { } pressedTree || drag != null || !e.GetCurrentPoint(Rows).Properties.IsLeftButtonPressed || ListTreeRowClick.IsEditor(e.Source))
                return;

            if (ListTreeRowClick.RowOf(e.Source) is { DataContext: TreeNode node, Place: { } place } && pressedTree.CanDrag(node))
                pressed = new PressedRow(node, PressOf(PointOf(e), place));
        }

        private static ListTreePress PressOf(ListTreePoint point, ListTreePlace place) => new(point, point.Y - place.Y, place.Depth);

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var point = PointOf(e);
            lastInScroller = e.GetPosition(Scroller);
            if (drag != null)
            {
                Track(point);
                AutoScroll.Track(lastInScroller.Y);
            }
            else if (pressed is { } press && pick.HasPassedThreshold(press.Press, point))
                Begin(press, e.Pointer, point);
        }

        private void Begin(PressedRow press, IPointer captured, ListTreePoint point)
        {
            pressed = null;
            if (tree is not { } dragged)
                return;

            var service = dragged.DropService;
            drag = new ListTreeDrag(new ListTreeCandidates(new ListTreeSlots(service), service, Rows.Sizes), pick, press.Node, press.Press);
            pointer = captured;
            captured.Capture(Scroller);
            dragged.BeginDrag(press.Node);
            cursorBefore = Scroller.Cursor;
            Scroller.Cursor = moveCursor;
            drag.MakeCandidates(ListTreeSnapshot.Of(dragged.Roots));
            Track(point);
        }

        private void Track(ListTreePoint point)
        {
            lastPoint = point;
            var shown = drag?.Look;
            if (drag is not { } dragging || dragging.PickAt(point) is not { } look)
            {
                Rows.ShowLiveTree();
                Rest(null);
                ShowNote(null);
                return;
            }

            if (!ReferenceEquals(look, shown))
                Rows.ShowDrag(look);

            var near = ListTreeNearCard.Of(look, dragging.Press, point);
            Rest(near);
            ShowNote(NoteOf(look, near));
        }

        private string NoteOf(ListTreeDragLook look, TreeNode? near) =>
            near != null && tree?.Shortcuts["Tree.Expand"] is { } key ? ListTreeDragNote.OfNearCard(look, near, key.ToString()) : ListTreeDragNote.Of(look);

        // A rest of the pointer next to a closed card opens it.
        private void Rest(TreeNode? near)
        {
            if (ReferenceEquals(near, resting))
                return;

            restTimer.Stop();
            resting = near;
            if (near != null)
                restTimer.Start();
        }

        private void OnRested(object? sender, EventArgs e) => OpenNearCard();

        // The timer and the key Tree.Expand open the card. It stays open after the drop and after a cancel.
        public void OpenNearCard()
        {
            var card = resting;
            Rest(null);
            if (drag == null || tree is not { } opened || card == null)
                return;

            card.IsExpanded = true;
            Rows.ApplyTreeChanges();
            drag.MakeCandidates(ListTreeSnapshot.Of(opened.Roots));
            Track(lastPoint);
        }

        // The release can come at another point than the last move, so the place is read again.
        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            pressed = null;
            if (drag is not { } ended || tree is not { } project)
                return;

            Track(PointOf(e));
            var drop = ended.Look is { } look && !ended.IsAtItsOwnPlace()
                ? project.DropAsync(ended.Node, look.Candidate.Slot.Target, look.Candidate.Slot.Position)
                : Task.CompletedTask;
            Finish();
            _ = ObserveAsync(drop);
        }

        private static async Task ObserveAsync(Task drop)
        {
            try
            {
                await drop;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"ListTreeProject: the drop failed. {exception}");
            }
        }

        // A scroll moves the rows under a pointer that stays still.
        private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (drag != null)
                Track(new ListTreePoint(lastInScroller.X - Rows.Bounds.X, lastInScroller.Y + Scroller.Offset.Y - Rows.Bounds.Y));
        }

        private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => Cancel();

        private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape || drag == null)
                return;

            Cancel();
            e.Handled = true;
        }

        // The release of the capture comes back as a capture lost, which then does nothing.
        private void Finish()
        {
            drag = null;
            Rest(null);
            AutoScroll.Stop();
            Rows.ShowLiveTree();
            ShowNote(null);
            tree?.EndDrag();
            Scroller.Cursor = cursorBefore;
            cursorBefore = null;

            var captured = pointer;
            pointer = null;
            if (ReferenceEquals(captured?.Captured, Scroller))
                captured.Capture(null);
        }

        private void ShowNote(string? text)
        {
            Context.DragNote.Text = text;
            Context.DragNote.IsVisible = text != null;
        }

        private ListTreePoint PointOf(PointerEventArgs e)
        {
            var point = e.GetPosition(Rows);
            return new ListTreePoint(point.X, point.Y);
        }

        private sealed record PressedRow(TreeNode Node, ListTreePress Press);
    }
}
