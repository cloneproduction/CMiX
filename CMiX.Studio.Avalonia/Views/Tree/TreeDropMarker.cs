// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    public sealed class TreeDropMarker
    {
        private readonly TreeRowIndex rows;
        private TreeViewItem? barItem;

        internal TreeDropMarker(TreeRowIndex rows)
        {
            this.rows = rows;
        }

        public TreeViewItem? Marked { get; private set; }

        public DropPosition? MarkedPosition { get; private set; }

        internal DropLine? Line => barItem == null ? null : new DropLine(barItem, TreeDropMark.GetMark(barItem) == DropMark.GapBelow);

        public void Mark(TreeViewItem? item, DropPosition? position)
        {
            if (Marked != null)
                TreeDropMark.SetMark(Marked, DropMark.None);

            if (barItem != null)
            {
                TreeDropMark.SetMark(barItem, DropMark.None);
                barItem = null;
            }

            Marked = item;
            MarkedPosition = position;
            if (item == null || position == null)
                return;

            if (position == DropPosition.Inside)
            {
                TreeDropMark.SetMark(item, DropMark.Target);
                return;
            }

            if (item.DataContext is not TreeNode node)
                return;

            // For a drop after the node, the bar is under the last visible row of its block.
            var above = position == DropPosition.After ? TreeRowOrder.LastVisible(node) : TreeRowOrder.RowAbove(node);
            var aboveItem = above == null ? null : TreeRows.ItemOfNode(item, above) ?? rows.FindItem(above);
            barItem = aboveItem ?? item;
            TreeDropMark.SetMark(barItem, aboveItem != null ? DropMark.GapBelow : DropMark.GapTop);
        }
    }
}
