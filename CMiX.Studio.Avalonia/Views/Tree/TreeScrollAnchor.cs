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
    // Keeps the first row in view at its place when a drop changes the rows above it.
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

        // The first row that is fully in view and is not the dragged node or under it, and its place in the view. With no dragged node,
        // the first row that is fully in view.
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

        // The row of the node and its place in the view, when the row is fully in view. Null otherwise.
        public AnchoredRow? CaptureRow(TreeNode node)
        {
            if (context.Scroller is not { } view || rows.FindItem(node) is not { } item || TreeRows.RowTopIn(item, view) is not { } top)
                return null;

            // The item holds the rows of its children too, so the height is the one of the row itself.
            var height = TreeRows.RowBorderOf(item)?.Bounds.Height ?? 0;
            return top >= 0 && top + height <= view.Viewport.Height ? new AnchoredRow(node, top) : null;
        }

        // Scrolls so that the anchor row is where it was.
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

            // A change of less than half a pixel is not done: at a screen scale of 125% or 150% the scroll is rounded to pixels of the
            // screen, and a row that never lands exactly would make the scroll change again at each frame.
            var maxOffset = Math.Max(0, view.Extent.Height - view.Viewport.Height);
            var offset = Math.Clamp(view.Offset.Y + top - kept.Top, 0, maxOffset);
            if (Math.Abs(offset - view.Offset.Y) >= MinMove)
                view.Offset = view.Offset.WithY(offset);
        }

        private const double MinMove = 0.5;

        // The hold that runs now. A new hold ends it, so two holds never set the scroll to two different places.
        private int holdId;

        // A row closes over a few frames (CollapsePanel), so the rows above the view get shorter after the close. The anchor row is
        // put back once in each frame, until the list of children of every closed row is shut. The hold runs from the frames of the
        // window, not from the layouts: a change of the scroll in a layout asks for one more layout in the same frame, and a chain of
        // them stops the window (Avalonia: "Infinite layout loop detected").
        public void HoldWhileClosing(AnchoredRow? anchor, IReadOnlyList<TreeNode> closed)
        {
            var id = ++holdId;
            if (anchor is not { } kept || closed.Count == 0 || context.Scroller is not { } view || TopLevel.GetTopLevel(treeView) is not { } top)
                return;

            var panels = closed.Select(node => rows.FindItem(node)?.GetVisualDescendants().OfType<CollapsePanel>().FirstOrDefault(panel => panel.Name == "PART_Collapse"))
                               .OfType<CollapsePanel>().ToList();
            if (panels.Count == 0)
                return;

            // A safe end, in case a panel never reaches its end.
            var frames = 0;
            void Keep(TimeSpan _)
            {
                if (id != holdId)
                    return;

                Place(kept, view);
                if (panels.Any(panel => panel.Progress > 0) && ++frames < MaxHoldFrames)
                    top.RequestAnimationFrame(Keep);
            }

            top.RequestAnimationFrame(Keep);
        }

        private const int MaxHoldFrames = 120;

        // Ends the hold that runs. Its next frame then does nothing and asks for no frame. The view calls it when it leaves the window.
        public void EndHold() => holdId++;
    }
}
