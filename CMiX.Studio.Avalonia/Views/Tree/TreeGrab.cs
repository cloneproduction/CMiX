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
    // The grab of a row (Tree.Grab), as in Blender: the drag starts with no pressed button.
    // While it runs, it takes every click and every key of the window.
    public sealed class TreeGrab
    {
        // The value is owner decision D13.
        public const double MouseTakeOver = 14;

        private readonly Control owner;
        private readonly TreeView treeView;
        private readonly TreeDragSession session;
        private readonly TreeViewContext context;
        private readonly Action<TreeNode> reveal;
        private readonly Action changed;

        private TopLevel? top;

        private int slotIndex = -1;

        private bool isMarkFromKey;
        private Point keyMovePoint;

        // The release of a grab click must not reach a row.
        private bool releasePending;

        // A right click asks for a menu on the release.
        private bool menuBlocked;

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

        public bool IsGrabbing => top != null && session.IsRunning;

        internal bool TakeMenuBlock()
        {
            var blocked = menuBlocked || IsGrabbing;
            menuBlocked = false;
            return blocked;
        }

        public bool Start(TreeNode node, Point position)
        {
            if (top != null || session.IsRunning || context.Tree is not { } project || !project.CanDrag(node) || TopLevel.GetTopLevel(treeView) is not { } window)
                return false;

            top = window;
            session.Start(node, position);
            // TreeDropSlots needs the started drag: BeginDrag marks the blocked rows.
            slotIndex = new TreeDropSlots(project, node).StartIndex;
            isMarkFromKey = true;
            keyMovePoint = position;
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

        public void Cancel() => End(TreeDragEnd.Cancel);

        // At the start place a confirm ends the grab with no move, as in Blender.
        private void Confirm()
        {
            if (session.Slot != null)
                End(TreeDragEnd.Drop);
            else if (isMarkFromKey && context.Tree is { } project && session.Source is { } source && slotIndex == new TreeDropSlots(project, source).StartIndex)
                End(TreeDragEnd.Cancel);
        }

        // Make the list again each time, because Tree.Expand can open or close a row.
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
            isMarkFromKey = true;
            keyMovePoint = lastPointer;
            reveal(slot.Target);
            session.AimAt(slot.Target, slot.Position);
        }

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
                    // Tree.Expand goes on to the key binding.
                    break;
                default:
                    // Another key would change the rows under the grab.
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
            if (isMarkFromKey && ((Vector)(position - keyMovePoint)).Length < MouseTakeOver)
                return;

            isMarkFromKey = false;
            // Out of the tree the mark stays, so a short move out and back keeps the place.
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

            // The rows can move between the last move and the click, so the drop reads the mark.
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

            session.ScrollByWheel(TreeRows.WheelSteps(e));
            e.Handled = true;
        }

        private void OnGotFocus(object? sender, GotFocusEventArgs e)
        {
            if (IsGrabbing && e.Source is Visual focused && !ReferenceEquals(focused, owner) && !owner.IsVisualAncestorOf(focused))
                Cancel();
        }

        private void OnDeactivated(object? sender, EventArgs e) => Detach();

        private void End(TreeDragEnd end)
        {
            if (top == null)
                return;

            if (session.IsRunning)
                session.End(end);

            slotIndex = -1;
            isMarkFromKey = false;
            changed();
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

        // Every handler goes, also when a release is still awaited.
        internal void Detach()
        {
            Cancel();
            RemoveHandlers();
        }
    }
}
