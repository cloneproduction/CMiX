// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Panels;

namespace CMiX.Studio.Avalonia.Views
{
    public sealed class TreeScrollAnchor
    {
        private readonly TreeView treeView;
        private readonly TreeRowIndex rows;
        private readonly TreeViewContext context;

        internal TreeScrollAnchor(TreeView treeView, TreeRowIndex rows, TreeViewContext context)
        {
            this.treeView = treeView;
            this.rows = rows;
            this.context = context;
        }

        public AnchoredRow? CaptureAnchor(TreeNode? dragged)
        {
            if (context.Scroller is not { } view)
                return null;

            // A block that ends above the view has no row in view, so the rows under it are skipped too.
            bool Skip(TreeViewItem item) =>
                (dragged != null && item.DataContext is TreeNode node && ReferenceEquals(node, dragged))
                || (item.TranslatePoint(new Point(0, item.Bounds.Height), view) is { } bottom && bottom.Y < 0);

            foreach (var item in rows.InRowOrder(Skip))
            {
                if (item.DataContext is not TreeNode node || (dragged != null && node.IsOrIsUnder(dragged)))
                    continue;

                var top = TreeRows.RowTopIn(item, view);
                if (top is >= 0)
                    return new AnchoredRow(node, top.Value);
            }

            return null;
        }

        public AnchoredRow? CaptureRow(TreeNode node)
        {
            if (context.Scroller is not { } view || rows.FindItem(node) is not { } item || TreeRows.RowTopIn(item, view) is not { } top)
                return null;

            // The item holds the rows of its children too, so the height is the one of the row itself.
            var height = TreeRows.RowBorderOf(item)?.Bounds.Height ?? 0;
            return top >= 0 && top + height <= view.Viewport.Height ? new AnchoredRow(node, top) : null;
        }

        public void RestoreAnchor(AnchoredRow? anchor)
        {
            if (anchor is not { } kept || context.Scroller is not { } view)
                return;

            treeView.UpdateLayout();
            Place(kept, view);
        }

        private void Place(AnchoredRow kept, ScrollViewer view)
        {
            var item = rows.FindItem(kept.Node);
            if (item == null || TreeRows.RowTopIn(item, view) is not { } top)
                return;

            // At a scale of 125% or 150% the scroll rounds to pixels, so a change under half a pixel
            // would change the scroll again at each frame.
            var maxOffset = Math.Max(0, view.Extent.Height - view.Viewport.Height);
            var offset = Math.Clamp(view.Offset.Y + top - kept.Top, 0, maxOffset);
            if (Math.Abs(offset - view.Offset.Y) >= MinMove)
                view.Offset = view.Offset.WithY(offset);
        }

        private const double MinMove = 0.5;

        // A new hold ends the old one, so two holds never set two places.
        private readonly FrameHold hold = new();

        // A row closes over a few frames, so the anchor row is put back in each frame.
        // A scroll change in a layout asks for another layout and can stop the window (infinite layout loop).
        public void HoldWhileClosing(AnchoredRow? anchor, IReadOnlyList<TreeNode> closed)
        {
            hold.Stop();
            if (anchor is not { } kept || closed.Count == 0 || context.Scroller is not { } view || TopLevel.GetTopLevel(treeView) is not { } top)
                return;

            var panels = closed.Select(node => rows.FindItem(node)?.GetVisualDescendants().OfType<CollapsePanel>().FirstOrDefault(panel => panel.Name == "PART_Collapse"))
                               .OfType<CollapsePanel>().ToList();
            if (panels.Count == 0)
                return;

            hold.Run(top, () => Place(kept, view), () => panels.Any(panel => panel.Progress > 0));
        }

        public void EndHold() => hold.Stop();
    }
}
