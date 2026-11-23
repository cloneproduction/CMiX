// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // A press on a row of the new tree selects its node, as in the old tree. A toggle, a text box and the icon of a card keep their own click.
    internal sealed class ListTreeRowClick
    {
        private readonly PanelListTreeRows rows;

        private ProjectTree? tree;

        public ListTreeRowClick(PanelListTreeRows rows)
        {
            this.rows = rows;
        }

        public void Attach(ProjectTree attached)
        {
            tree = attached;
            rows.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
        }

        public void Detach()
        {
            rows.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            tree = null;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (tree == null || !e.GetCurrentPoint(rows).Properties.IsLeftButtonPressed || e.KeyModifiers.HasFlag(KeyModifiers.Control))
                return;

            if (RowOf(e.Source) is not { DataContext: TreeNode node } row || IsOwnClick(row, e.Source))
                return;

            tree.SelectByClick(node);
        }

        private static RowListTree? RowOf(object? source) => (source as Visual)?.FindAncestorOfType<RowListTree>(includeSelf: true);

        private static bool IsOwnClick(RowListTree row, object? source) =>
            TreeRows.HasAncestor<ToggleButton>(source) || TreeRows.HasAncestor<TextBox>(source) || row.IsCardIcon(source);
    }
}
