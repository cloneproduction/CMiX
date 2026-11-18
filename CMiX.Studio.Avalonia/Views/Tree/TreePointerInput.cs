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
    // The pointer on one view of the tree: the press, the move and the release on a row, the wheel during a drag and the double tap.
    internal sealed class TreePointerInput
    {
        private readonly Control owner;
        private readonly TreeViewContext context;
        private readonly TreeRowDrag rowDrag;
        private readonly TreeFocusKeeper focusKeeper;

        // The node of the row that the left button pressed and that can be dragged. A drag clears it.
        private TreeNode? pressedNode;

        // The node of the row that the left button pressed. A drag clears it.
        private TreeNode? chosenNode;

        internal TreePointerInput(Control owner, TreeViewContext context, TreeRowDrag rowDrag, TreeFocusKeeper focusKeeper)
        {
            this.owner = owner;
            this.context = context;
            this.rowDrag = rowDrag;
            this.focusKeeper = focusKeeper;
        }

        // The last place of the pointer over the tree, in the coordinates of the tree view. Tree.Add opens there when no target row is shown.
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

        // A double tap on a row, away from its name, expands or collapses the row. The name takes its own double tap (it starts
        // the rename and handles the tap), so that tap never comes here. A toggle or an input keeps its double tap too.
        // TreeViewItem does this by itself for the part PART_HeaderPresenter. The template has no such part (see ProjectTree.axaml),
        // so the tree does it here, and only where it is wanted.
        private void OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (IsEditor(e.Source) || TreeRows.ItemOf(e.Source)?.DataContext is not TreeNode { Children.Count: > 0 } node)
                return;

            node.IsExpanded = !node.IsExpanded;
            e.Handled = true;
        }

        private void OnPointerPressedDone(object? sender, PointerPressedEventArgs e) => focusKeeper.EndPress();

        // A ToggleButton of a row that cannot take the focus: a toggle or the chevron.
        private static bool IsRowButton(object? source) =>
            (source as Visual)?.FindAncestorOfType<ToggleButton>(true) is { Focusable: false } button && TreeRows.ItemOf(button) != null;

        private static bool IsInToggles(object? source) =>
            (source as Visual)?.FindAncestorOfType<RowToggles>(true) != null;

        private static bool IsEditor(object? source)
        {
            for (var visual = source as Visual; visual != null; visual = visual.GetVisualParent())
                if (visual is ToggleButton or TextBox)
                    return true;

            return false;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressedNode = null;
            chosenNode = null;
            focusKeeper.BeginPress();
            // A middle click on a row selects it and sends it to the inspector, and starts no drag. The chevron, the toggles
            // and the name box take no middle click.
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
            // A click beside the rows gives the tree the focus on its cursor row, so the keys work right away. A tree without a cursor
            // gets one first: the selected row, else the first row. The focus moves after the press is done (OnPointerPressedDone): the
            // default handling of the press would take it back.
            if (node is null && context.Tree is { } beside)
            {
                beside.CursorNode ??= beside.SelectedNode ?? beside.Roots.FirstOrDefault();
                if (beside.CursorNode != null)
                    focusKeeper.FocusCursorRowAfterPress();
            }

            // The tree view toggles the selection on a Ctrl click. The tree has one selection, so a Ctrl click on a row does nothing.
            if (node != null && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                e.Handled = true;
                return;
            }

            // The chevron and the toggles are ToggleButtons. IsEditor stops a click on them above, so it does not move the cursor.
            if (node != null && context.Tree is { } clicked)
                clicked.SelectByClick(node);

            // The toggles at the right of a row never start a drag, not even the space around them.
            pressedNode = node != null && !IsInToggles(e.Source) && context.Tree is { } tree && tree.CanDrag(node) ? node : null;
            chosenNode = node;
            rowDrag.Press(pressedNode, e.GetPosition(TreeView));
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!e.GetCurrentPoint(TreeView).Properties.IsLeftButtonPressed)
                return;

            rowDrag.Move(e.GetPosition(TreeView), e);
            if (rowDrag.IsDragging)
            {
                pressedNode = null;
                chosenNode = null;
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var node = chosenNode;
            pressedNode = null;
            chosenNode = null;
            rowDrag.Release(e.GetPosition(TreeView));
            if (node == null || e.InitialPressMouseButton != MouseButton.Left || !IsOverRow(node, e))
                return;

            owner.RaiseEvent(new NodeChosenEventArgs(ProjectTreeView.NodeChosenEvent, owner, node));
        }

        // The capture goes when the window loses the pointer or the tree leaves the window: the drag ends without a drop.
        private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => rowDrag.Cancel();

        // The wheel during a drag of a row scrolls the tree by rows (see DragAutoScroller). The scroll viewer does not see it.
        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (!rowDrag.IsDragging)
                return;

            var steps = (int)Math.Round(e.Delta.Y);
            rowDrag.Wheel(steps != 0 ? steps : Math.Sign(e.Delta.Y));
            e.Handled = true;
        }

        // True when the pointer is on the row border of the node.
        private bool IsOverRow(TreeNode node, PointerEventArgs e) =>
            context.RowIndex.FindItem(node) is { } item
            && TreeRows.RowBorderOf(item) is { } border
            && new Rect(border.Bounds.Size).Contains(e.GetPosition(border));
    }
}
