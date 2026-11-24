// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The pointer drag of a row of the tree. The scroller captures the pointer, so the moves and the release come on every platform.
    internal sealed class ListTreeRowDrag : ListTreeBehavior
    {
        private ProjectTree? tree;
        private PressedRow? pressed;
        private IPointer? pointer;
        private Point lastInScroller;

        public bool IsDragging => pointer != null;

        internal ListTreeDrag? Drag => Session.Drag;

        private PanelListTreeRows Rows => Context.Rows;

        private ScrollViewer Scroller => Context.Scroller;

        private ListTreeDragSession Session => Part<ListTreeDragSession>();

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            Session.Ended += OnSessionEnded;
            Scroller.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Bubble, handledEventsToo: true);
            Scroller.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Bubble, handledEventsToo: true);
            Scroller.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);
            Scroller.AddHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost);
            Scroller.AddHandler(InputElement.KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);
            Scroller.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheel, RoutingStrategies.Tunnel);
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
            Scroller.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheel);
            Scroller.ScrollChanged -= OnScrollChanged;
            Session.Ended -= OnSessionEnded;
            tree = null;
        }

        public void Cancel()
        {
            pressed = null;
            if (IsDragging)
                Session.Cancel();
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressed = null;
            if (tree is not { } pressedTree || Session.IsRunning || Context.InputOwner != ListTreeInputOwner.None || !e.GetCurrentPoint(Rows).Properties.IsLeftButtonPressed || ListTreeRowClick.IsEditor(e.Source))
                return;

            if (RowListTree.Of(e.Source) is { DataContext: TreeNode node, Place: { } place } && pressedTree.CanDrag(node))
                pressed = new PressedRow(node, ListTreePress.At(PointOf(e), place));
        }

        // The scroller holds the pointer during a drag, so its own wheel scroll does not run.
        private void OnPointerWheel(object? sender, PointerWheelEventArgs e)
        {
            if (!IsDragging)
                return;

            Session.ScrollByWheel(e);
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            lastInScroller = e.GetPosition(Scroller);
            if (IsDragging)
            {
                Session.TrackPointer(lastInScroller);
                return;
            }

            var point = Context.ToRows(lastInScroller);
            if (pressed is { } press && Session.Pick.HasPassedThreshold(press.Press, point))
                Begin(press, e.Pointer, point);
        }

        private void Begin(PressedRow press, IPointer captured, ListTreePoint point)
        {
            pressed = null;
            Session.Begin(press.Node, press.Press);
            if (!Session.IsRunning)
                return;

            pointer = captured;
            Context.InputOwner = ListTreeInputOwner.Drag;
            captured.Capture(Scroller);
            Session.Track(point);
        }

        // The release can come at another point than the last move, so the place is read again.
        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            pressed = null;
            if (!IsDragging)
                return;

            Session.Track(PointOf(e));
            Session.DropOrCancel();
        }

        // A scroll moves the rows under a pointer that stays still.
        private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (IsDragging)
                Session.Track(Context.ToRows(lastInScroller));
        }

        private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => Cancel();

        private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape || !IsDragging)
                return;

            Cancel();
            e.Handled = true;
        }

        // The release of the capture comes back as a capture lost, which then does nothing.
        private void OnSessionEnded(object? sender, EventArgs e)
        {
            var captured = pointer;
            pointer = null;
            if (captured != null)
                Context.InputOwner = ListTreeInputOwner.None;
            if (ReferenceEquals(captured?.Captured, Scroller))
                captured.Capture(null);
        }

        private ListTreePoint PointOf(PointerEventArgs e) => Context.ToRows(e.GetPosition(Scroller));

        private sealed record PressedRow(TreeNode Node, ListTreePress Press);
    }
}
