// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // Shows the place of a drop on the rows of the project tree with TreeDropMark. It keeps the marked row and its place.
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

        // The item that shows the line of a drop between rows, and true when the line is in the space under its row (false: at its top).
        // Null when no line shows. The label of the drag keeps clear of the line (see TreeRowDrag).
        internal DropLine? Line => barItem == null ? null : new DropLine(barItem, TreeDropMark.GetMark(barItem) == DropMark.GapBelow);

        // Marks the item for a drop at the position. A null item or position removes the mark.
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

            // The bar is in the space under the row above the place: the last visible row of the block for a drop after the node.
            var above = position == DropPosition.After ? TreeDropService.LastVisible(node) : TreeDropService.RowAbove(node);
            var aboveItem = above == null ? null : TreeRows.ItemOfNode(item, above) ?? rows.FindItem(above);
            barItem = aboveItem ?? item;
            TreeDropMark.SetMark(barItem, aboveItem != null ? DropMark.GapBelow : DropMark.GapTop);
        }
    }
}
