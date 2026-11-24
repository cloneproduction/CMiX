// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
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
    // A press selects the node of a row: Ctrl toggles, Shift selects from the anchor, middle click also inspects, and a plain press in a selection waits for the release so a drag can take all.
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
            if (IsEditor(e.Source) || RowListTree.Of(e.Source) is not { DataContext: TreeNode node })
                return;

            clicked.SelectByClick(node);
            clicked.Inspect(node);
            e.Handled = true;
        }

        private void SelectByLeftClick(ProjectTree clicked, PointerPressedEventArgs e)
        {
            if (IsEditor(e.Source))
            {
                if (HasAncestor<ToggleButton>(e.Source))
                    PressFocus.RememberFocusBeforeRowButton();
                return;
            }

            var row = RowListTree.Of(e.Source);
            if (row?.DataContext is not TreeNode node)
            {
                PressBesideRows(clicked);
                return;
            }

            var keys = new ListTreeRowPressRule.Keys(e.KeyModifiers.HasFlag(KeyModifiers.Control), e.KeyModifiers.HasFlag(KeyModifiers.Shift));
            var action = ListTreeRowPressRule.ActionOf(keys, row.PressPartOf(e.Source));
            if (action == ListTreeRowPressRule.Action.Select)
                SelectByPress(clicked, node);
            else if (action == ListTreeRowPressRule.Action.AddOrRemove)
                clicked.AddOrRemoveFromSelection(node);
            else if (action == ListTreeRowPressRule.Action.SelectRange)
                clicked.SelectRange(node, ListTreeSnapshot.Of(clicked.Roots).VisibleNodes());
            else if (action == ListTreeRowPressRule.Action.Stop)
                e.Handled = true;
        }

        private void SelectByPress(ProjectTree clicked, TreeNode node)
        {
            if (clicked.IsOneOfSeveralSelected(node))
                pressedInSelection = node;
            else
                clicked.SelectByClick(node);
        }

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

        internal static bool IsEditor(object? source) => HasAncestor<ToggleButton>(source) || HasAncestor<TextBox>(source);

        // The source itself counts.
        internal static bool HasAncestor<T>(object? source) where T : Visual =>
            (source as Visual)?.FindAncestorOfType<T>(includeSelf: true) != null;
    }
}
