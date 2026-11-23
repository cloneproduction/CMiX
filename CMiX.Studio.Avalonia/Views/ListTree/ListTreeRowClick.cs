// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // A press on a row of the new tree selects its node, as in the old tree. A toggle, a text box and the icon of a card keep their own click.
    // A middle click selects and inspects. The focus stays in the tree, as TreePointerInput keeps it in the old tree.
    internal sealed class ListTreeRowClick
    {
        private readonly Control owner;
        private readonly PanelListTreeRows rows;
        private readonly ListTreePressFocus pressFocus;

        private ProjectTree? tree;

        public ListTreeRowClick(Control owner, PanelListTreeRows rows, ListTreePressFocus pressFocus)
        {
            this.owner = owner;
            this.rows = rows;
            this.pressFocus = pressFocus;
        }

        public void Attach(ProjectTree attached)
        {
            tree = attached;
            owner.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            owner.AddHandler(InputElement.PointerPressedEvent, OnPointerPressedDone, RoutingStrategies.Bubble, handledEventsToo: true);
        }

        public void Detach()
        {
            owner.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            owner.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressedDone);
            tree = null;
        }

        private void OnPointerPressedDone(object? sender, PointerPressedEventArgs e) => pressFocus.EndPress();

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressFocus.BeginPress();
            if (tree == null)
                return;

            var properties = e.GetCurrentPoint(rows).Properties;
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
                    pressFocus.RememberFocusBeforeRowButton();
                return;
            }

            var row = RowOf(e.Source);
            if (row?.DataContext is not TreeNode node)
            {
                PressBesideRows(clicked);
                return;
            }

            var action = ListTreeRowPressRule.ActionOf(e.KeyModifiers.HasFlag(KeyModifiers.Control), row.IsCardIcon(e.Source));
            if (action == ListTreeRowPressAction.Select)
                clicked.SelectByClick(node);
            else if (action == ListTreeRowPressAction.Stop)
                e.Handled = true;
        }

        // The focus moves after the press, because the default handling of the press would take it back.
        private void PressBesideRows(ProjectTree clicked)
        {
            clicked.EnsureCursor();
            if (clicked.CursorNode != null)
                pressFocus.FocusCursorRowAfterPress();
        }

        private static bool IsEditor(object? source) => TreeRows.HasAncestor<ToggleButton>(source) || TreeRows.HasAncestor<TextBox>(source);

        private static RowListTree? RowOf(object? source) => (source as Visual)?.FindAncestorOfType<RowListTree>(includeSelf: true);
    }
}
