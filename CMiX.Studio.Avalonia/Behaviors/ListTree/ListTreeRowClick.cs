// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // A press on a row of the new tree selects its node, as in the old tree. A toggle, a text box and the icon of a card keep their own click.
    // A middle click selects and inspects. The focus stays in the tree, as TreePointerInput keeps it in the old tree.
    // Ctrl adds a row to the selection or takes it out, Shift selects the shown rows from the anchor.
    // A plain press on a row of a selection of several rows selects only that row on the release, so a drag can take them all.
    internal sealed class ListTreeRowClick : ListTreeBehavior
    {
        private ProjectTree? tree;
        private TreeNode? pressedInSelection;

        private ListTreePressFocus PressFocus => Part<ListTreePressFocus>();

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            Context.Scroller.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            Context.Scroller.AddHandler(InputElement.PointerPressedEvent, OnPointerPressedDone, RoutingStrategies.Bubble, handledEventsToo: true);
            Context.Scroller.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel, handledEventsToo: true);
            Part<ListTreeDragSession>().Began += OnDragBegan;
        }

        protected override void OnUnwatch()
        {
            Context.Scroller.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            Context.Scroller.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressedDone);
            Context.Scroller.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            Part<ListTreeDragSession>().Began -= OnDragBegan;
            pressedInSelection = null;
            tree = null;
        }

        private void OnPointerPressedDone(object? sender, PointerPressedEventArgs e) => PressFocus.EndPress();

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressedInSelection = null;
            PressFocus.BeginPress();
            if (tree == null)
                return;

            var properties = e.GetCurrentPoint(Context.Rows).Properties;
            if (properties.IsMiddleButtonPressed)
                InspectByMiddleClick(tree, e);
            else if (properties.IsLeftButtonPressed)
                SelectByLeftClick(tree, e);
        }

        private static void InspectByMiddleClick(ProjectTree clicked, PointerPressedEventArgs e)
        {
            if (IsEditor(e.Source) || RowOf(e.Source) is not { DataContext: TreeNode node })
                return;

            clicked.SelectByClick(node);
            clicked.Inspect(node);
            e.Handled = true;
        }

        private void SelectByLeftClick(ProjectTree clicked, PointerPressedEventArgs e)
        {
            if (IsEditor(e.Source))
            {
                if (TreeRows.HasAncestor<ToggleButton>(e.Source))
                    PressFocus.RememberFocusBeforeRowButton();
                return;
            }

            var row = RowOf(e.Source);
            if (row?.DataContext is not TreeNode node)
            {
                PressBesideRows(clicked);
                return;
            }

            var modifiers = e.KeyModifiers;
            var action = ListTreeRowPressRule.ActionOf(modifiers.HasFlag(KeyModifiers.Control), modifiers.HasFlag(KeyModifiers.Shift), row.IsCardIcon(e.Source));
            if (action == ListTreeRowPressAction.Select)
                SelectByPress(clicked, node);
            else if (action == ListTreeRowPressAction.AddOrRemove)
                AddOrRemove(clicked, node);
            else if (action == ListTreeRowPressAction.SelectRange)
                clicked.SelectRange(node, ShownNodesOf(clicked));
            else if (action == ListTreeRowPressAction.Stop)
                e.Handled = true;
        }

        private void SelectByPress(ProjectTree clicked, TreeNode node)
        {
            if (clicked.SelectedNodes.Count > 1 && clicked.SelectedNodes.Contains(node))
                pressedInSelection = node;
            else
                clicked.SelectByClick(node);
        }

        private static void AddOrRemove(ProjectTree clicked, TreeNode node)
        {
            if (clicked.SelectedNodes.Contains(node))
                clicked.RemoveFromSelection(node);
            else
                clicked.AddToSelection(node);
        }

        private static IReadOnlyList<TreeNode> ShownNodesOf(ProjectTree clicked) =>
            ListTreeSnapshot.Of(clicked.Roots).VisibleRows().Select(row => row.Node).OfType<TreeNode>().ToArray();

        // A drag took the press, so the selection stays.
        private void OnDragBegan(object? sender, EventArgs e) => pressedInSelection = null;

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var pressed = pressedInSelection;
            pressedInSelection = null;
            if (pressed != null && tree != null && !Part<ListTreeDragSession>().IsRunning)
                tree.SelectByClick(pressed);
        }

        // The focus moves after the press, because the default handling of the press would take it back.
        private void PressBesideRows(ProjectTree clicked)
        {
            clicked.EnsureCursor();
            if (clicked.CursorNode != null)
                PressFocus.FocusCursorRowAfterPress();
        }

        internal static bool IsEditor(object? source) => TreeRows.HasAncestor<ToggleButton>(source) || TreeRows.HasAncestor<TextBox>(source);

        internal static RowListTree? RowOf(object? source) => (source as Visual)?.FindAncestorOfType<RowListTree>(includeSelf: true);
    }
}
