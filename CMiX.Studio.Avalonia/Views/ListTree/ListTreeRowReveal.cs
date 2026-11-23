// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // Brings a row of the new tree into view, as TreeRowReveal does in the old tree.
    internal sealed class ListTreeRowReveal
    {
        private readonly PanelListTreeRows rows;

        public ListTreeRowReveal(PanelListTreeRows rows)
        {
            this.rows = rows;
        }

        public void RevealRow(TreeNode node) => rows.RowOf(node)?.BringIntoView();

        // A row under a closed card is not shown, so it gives null.
        public RowListTree? ShownRowOf(TreeNode node)
        {
            if (rows.RowOf(node) is not { } row)
                return null;

            row.BringIntoView();
            rows.UpdateLayout();
            return row;
        }
    }
}
