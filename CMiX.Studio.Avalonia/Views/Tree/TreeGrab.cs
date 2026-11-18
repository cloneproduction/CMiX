// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The grab of a row (Tree.Grab, G), as in Blender: the drag starts at once, with no pressed button. The pointer moves the mark of
    // the drop, and the arrow keys move it to the next valid place (see TreeDropSlots). The last input wins: after a key, the pointer
    // takes the mark back only when it moves further than MouseTakeOver.
    // A left click or Enter puts the row at the mark. A click where no drop is offered does nothing. A right click, Escape, a focus
    // that goes out of the tree, a window that is not active any more and the end of the view end the grab with no move.
    // While the grab runs it takes every click and every key of the window, so nothing reaches the rows or the other panels.
    // The grab uses the same drag as a drag with the button (see TreeDragSession). The drop is one undo step.
    public sealed class TreeGrab
    {
        // The pointer takes the mark back from the keys when it moves this far, in pixels (owner decision D13).
        public const double MouseTakeOver = 14;

        private readonly Control owner;
        private readonly TreeView treeView;
        private readonly TreeDragSession session;
        private readonly TreeViewContext context;
        private readonly Action<TreeNode> reveal;
        private readonly Action changed;

        // The window of the grab while it runs. Null when no grab runs.
        private TopLevel? top;

        // The place of the mark that the keys move, as an index in the valid places of the drop.
        private int slotIndex = -1;

        // True after a key moved the mark: the pointer takes it back only after a move past MouseTakeOver from keyPoint.
        private bool keyMode;
        private Point keyPoint;

        // True from a click of the grab to the release of its button, so the release reaches no row.
        private bool releasePending;

        // True after a click of the grab: the next request for a menu does not open it (a right click asks on the release).
        private bool menuBlocked;

        // owner is the view of the tree: a focus that goes out of it ends the grab. reveal brings the row of a node into view. changed runs
        // when the grab starts and when it ends.
        internal TreeGrab(Control owner, TreeView treeView, TreeDragSession session, TreeViewContext context, Action<TreeNode> reveal,
            Action changed)
        {
            this.changed = changed;
            this.owner = owner;
            this.treeView = treeView;
            this.session = session;
            this.context = context;
            this.reveal = reveal;
        }

        // True from the start of the grab to its end.
        public bool IsGrabbing => top != null && session.IsRunning;

        // True when the view must not open a menu now: a click of the grab asked for it. It is true one time only.
        internal bool TakeMenuBlock()
        {
            var blocked = menuBlocked || IsGrabbing;
            menuBlocked = false;
            return blocked;
        }

        // Starts the grab of the node. The position is the last place of the pointer, in the tree view. The label of the row shows
        // there. Returns false when the node cannot move or a drag already runs.
        public bool Start(TreeNode node, Point position)
        {
            if (top != null || session.IsRunning || context.Tree is not { } project || !project.CanDrag(node) || TopLevel.GetTopLevel(treeView) is not { } window)
                return false;

            top = window;
            session.Start(node, position);
            // The list of places needs the drag: BeginDrag tells the rows which of them block the node.
            slotIndex = new TreeDropSlots(project, node).StartIndex;
            keyMode = true;
            keyPoint = position;
            lastPointer = position;
            menuBlocked = false;

            window.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheel, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.GotFocusEvent, OnGotFocus, RoutingStrategies.Bubble, handledEventsToo: true);
            if (window is WindowBase windowBase)
                windowBase.Deactivated += OnDeactivated;
            if (window is Window closable)
                closable.Closed += OnDeactivated;
            changed();
            return true;
        }

        // Ends the grab with no move. The view calls it when it goes, so no handler stays on the window.
        public void Cancel() => End(TreeDragEnd.Cancel);

        // Puts the row at the mark. With no valid place at the mark nothing happens and the grab goes on, except at the start place:
        // there the grab ends with no move, as a confirm in Blender.
        private void Confirm()
        {
            if (session.Slot != null)
                End(TreeDragEnd.Drop);
            else if (keyMode && context.Tree is { } project && session.Source is { } source && slotIndex == new TreeDropSlots(project, source).StartIndex)
                End(TreeDragEnd.Cancel);
        }

        // Moves the mark to the next or the previous valid place. The list is made again each time, because Tree.Expand can open or
        // close a row during the grab.
        private void Step(int direction)
        {
            if (context.Tree is not { } project || session.Source is not { } source)
                return;

            var slots = new TreeDropSlots(project, source);
            if (session.Slot is { } marked && slots.IndexOf(marked.Target, marked.Position) is var index and >= 0)
                slotIndex = index;
            else if (slotIndex >= slots.Slots.Count)
                slotIndex = slots.StartIndex;

            slotIndex = direction > 0 ? slots.Next(slotIndex) : slots.Previous(slotIndex);
            if (slotIndex < 0)
                return;

            var slot = slots.Slots[slotIndex];
            keyMode = true;
            keyPoint = lastPointer;
            reveal(slot.Target);
            session.AimAt(slot.Target, slot.Position);
        }

        // The last place of the pointer during the grab, in the tree view.
        private Point lastPointer;

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (!IsGrabbing)
                return;

            switch (e.Key)
            {
                case Key.Up when e.KeyModifiers == KeyModifiers.None:
                    Step(-1);
                    e.Handled = true;
                    break;
                case Key.Down when e.KeyModifiers == KeyModifiers.None:
                    Step(1);
                    e.Handled = true;
                    break;
                case Key.Enter:
                    Confirm();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;
                case Key.E:
                    // Tree.Expand opens or closes the row at the mark (see TreeDragSession.ToggleHovered). It goes on to the key binding.
                    break;
                default:
                    // No other key acts during the grab: a key of the tree or of the window would change the rows under the grab.
                    e.Handled = true;
                    break;
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!IsGrabbing)
                return;

            var position = e.GetPosition(treeView);
            lastPointer = position;
            if (keyMode && Distance(position, keyPoint) < MouseTakeOver)
                return;

            keyMode = false;
            // Out of the tree the mark stays where it was, so a short move out and back does not lose the place.
            if (new Rect(treeView.Bounds.Size).Contains(position))
                session.Track(position);
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!IsGrabbing)
                return;

            e.Handled = true;
            releasePending = true;
            menuBlocked = true;
            var properties = e.GetCurrentPoint(treeView).Properties;
            if (properties.IsRightButtonPressed)
            {
                Cancel();
                return;
            }

            if (!properties.IsLeftButtonPressed)
                return;

            // The drop reads the mark, not the pointer: the last move set the mark, and the rows can move between that move and the click.
            if (session.Slot != null)
                End(TreeDragEnd.Drop);
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!releasePending)
                return;

            e.Handled = true;
            releasePending = false;
            if (!IsGrabbing)
                RemoveHandlers();
        }

        private void OnPointerWheel(object? sender, PointerWheelEventArgs e)
        {
            if (!IsGrabbing)
                return;

            var steps = (int)Math.Round(e.Delta.Y);
            session.ScrollByWheel(steps != 0 ? steps : Math.Sign(e.Delta.Y));
            e.Handled = true;
        }

        // A focus that goes out of the view of the tree ends the grab.
        private void OnGotFocus(object? sender, GotFocusEventArgs e)
        {
            if (IsGrabbing && e.Source is Visual focused && !ReferenceEquals(focused, owner) && !owner.IsVisualAncestorOf(focused))
                Cancel();
        }

        // The window is not active any more, or it closes: the grab ends with no move, and no handler stays on the window.
        private void OnDeactivated(object? sender, EventArgs e) => Detach();

        private void End(TreeDragEnd end)
        {
            if (top == null)
                return;

            if (session.IsRunning)
                session.End(end);

            slotIndex = -1;
            keyMode = false;
            changed();
            // A click ended the grab: its release must not reach a row. The handlers go when the release comes.
            if (!releasePending)
                RemoveHandlers();
        }

        private void RemoveHandlers()
        {
            if (top is not { } window)
                return;

            window.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            window.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
            window.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            window.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            window.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheel);
            window.RemoveHandler(InputElement.GotFocusEvent, OnGotFocus);
            if (window is WindowBase windowBase)
                windowBase.Deactivated -= OnDeactivated;
            if (window is Window closable)
                closable.Closed -= OnDeactivated;
            top = null;
            releasePending = false;
        }

        // The view goes: every handler goes with it, also when a release is still awaited.
        internal void Detach()
        {
            Cancel();
            RemoveHandlers();
        }

        private static double Distance(Point a, Point b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
    }
}
