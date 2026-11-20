// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    internal sealed class TreePointerInput
    {
        private readonly Control owner;
        private readonly TreeViewContext context;
        private readonly TreeRowDrag rowDrag;
        private readonly TreeFocusKeeper focusKeeper;

        private TreeNode? pressedDraggableNode;

        // A drag clears it.
        private TreeNode? chosenNode;

        internal TreePointerInput(Control owner, TreeViewContext context, TreeRowDrag rowDrag, TreeFocusKeeper focusKeeper)
        {
            this.owner = owner;
            this.context = context;
            this.rowDrag = rowDrag;
            this.focusKeeper = focusKeeper;
        }

        internal Point LastPointer { get; private set; }

        private TreeView TreeView => context.TreeView;

        internal void Attach()
        {
            TreeView.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            TreeView.AddHandler(InputElement.PointerPressedEvent, OnPointerPressedDone, RoutingStrategies.Bubble, handledEventsToo: true);
            TreeView.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
            TreeView.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            TreeView.AddHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost);
            TreeView.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
            TreeView.AddHandler(InputElement.DoubleTappedEvent, OnDoubleTapped, RoutingStrategies.Bubble);
            owner.AddHandler(InputElement.PointerMovedEvent, OnPointerTracked, RoutingStrategies.Tunnel, handledEventsToo: true);
        }

        private void OnPointerTracked(object? sender, PointerEventArgs e) => LastPointer = e.GetPosition(TreeView);

        // TreeViewItem toggles on a double tap only for PART_HeaderPresenter, which the template does not have,
        // so the tree does it here. The name and the toggles keep their own double tap.
        private void OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (IsEditor(e.Source) || TreeRows.ItemOf(e.Source)?.DataContext is not TreeNode { Children.Count: > 0 } node)
                return;

            node.IsExpanded = !node.IsExpanded;
            e.Handled = true;
        }

        private void OnPointerPressedDone(object? sender, PointerPressedEventArgs e) => focusKeeper.EndPress();

        private static bool IsRowButton(object? source) =>
            (source as Visual)?.FindAncestorOfType<ToggleButton>(true) is { Focusable: false } button && TreeRows.ItemOf(button) != null;

        private static bool IsInToggles(object? source) =>
            (source as Visual)?.FindAncestorOfType<TogglesRow>(true) != null;

        private static bool IsEditor(object? source) =>
            TreeRows.HasAncestor<ToggleButton>(source) || TreeRows.HasAncestor<TextBox>(source);

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressedDraggableNode = null;
            chosenNode = null;
            focusKeeper.BeginPress();
            // A middle click selects and inspects, and starts no drag.
            if (e.GetCurrentPoint(TreeView).Properties.IsMiddleButtonPressed)
            {
                if (!rowDrag.IsDragging && !IsEditor(e.Source) && TreeRows.ItemOf(e.Source)?.DataContext is TreeNode middle && context.Tree is { } inspecting)
                {
                    inspecting.SelectByClick(middle);
                    inspecting.Inspect(middle);
                    e.Handled = true;
                }
                return;
            }

            if (rowDrag.IsDragging || !e.GetCurrentPoint(TreeView).Properties.IsLeftButtonPressed)
                return;

            if (IsEditor(e.Source))
            {
                if (IsRowButton(e.Source))
                    focusKeeper.RememberFocusBeforeRowButton();
                return;
            }

            var node = TreeRows.ItemOf(e.Source)?.DataContext as TreeNode;
            // The focus moves after the press, because the default handling of the press would take it back.
            if (node is null && context.Tree is { } beside)
            {
                beside.CursorNode ??= beside.SelectedNode ?? beside.Roots.FirstOrDefault();
                if (beside.CursorNode != null)
                    focusKeeper.FocusCursorRowAfterPress();
            }

            // The tree has one selection, so a Ctrl click on a row does nothing.
            if (node != null && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                e.Handled = true;
                return;
            }

            // IsEditor stops a click on the chevron and the toggles above, so they do not move the cursor.
            if (node != null && context.Tree is { } clicked)
                clicked.SelectByClick(node);

            // The toggles and the space around them never start a drag.
            pressedDraggableNode = node != null && !IsInToggles(e.Source) && context.Tree is { } tree && tree.CanDrag(node) ? node : null;
            chosenNode = node;
            rowDrag.Press(pressedDraggableNode, e.GetPosition(TreeView));
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!e.GetCurrentPoint(TreeView).Properties.IsLeftButtonPressed)
                return;

            rowDrag.Move(e.GetPosition(TreeView), e);
            if (rowDrag.IsDragging)
            {
                pressedDraggableNode = null;
                chosenNode = null;
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var node = chosenNode;
            pressedDraggableNode = null;
            chosenNode = null;
            rowDrag.Release(e.GetPosition(TreeView));
            if (node == null || e.InitialPressMouseButton != MouseButton.Left || !IsOverRow(node, e))
                return;

            owner.RaiseEvent(new NodeChosenEventArgs(ProjectTreeView.NodeChosenEvent, owner, node));
        }

        // The drag ends without a drop when the window loses the pointer.
        private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => rowDrag.Cancel();

        // The scroll viewer does not see the wheel during a drag.
        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (!rowDrag.IsDragging)
                return;

            rowDrag.Wheel(TreeRows.WheelSteps(e));
            e.Handled = true;
        }

        private bool IsOverRow(TreeNode node, PointerEventArgs e) =>
            context.RowIndex.FindItem(node) is { } item
            && TreeRows.RowBorderOf(item) is { } border
            && new Rect(border.Bounds.Size).Contains(e.GetPosition(border));
    }
}
