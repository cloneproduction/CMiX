// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // Keeps one row of the new tree still on the screen while cards close above it, as TreeScrollAnchor does in the old tree.
    // The rows slide to their new places, so the anchor is put back in each frame of the slide.
    internal sealed class ListTreeScrollAnchor
    {
        // At a scale of 125% or 150% the scroll rounds to pixels, so a smaller change would scroll again at each frame.
        private const double MinMove = 0.5;

        private readonly PanelListTreeRows rows;
        private readonly ScrollViewer scroller;
        private readonly FrameHold hold = new();

        public ListTreeScrollAnchor(PanelListTreeRows rows, ScrollViewer scroller)
        {
            this.rows = rows;
            this.scroller = scroller;
        }

        // The row of the node, when the whole row is in view.
        public AnchoredRow? CaptureRow(TreeNode node)
        {
            if (rows.RowOf(node) is not { } row)
                return null;

            var top = TopInView(row);
            return top >= 0 && top + row.Bounds.Height <= scroller.Viewport.Height ? new AnchoredRow(node, top) : null;
        }

        // The first row with its top in view.
        public AnchoredRow? CaptureFirstRowInView()
        {
            foreach (var row in RowsInOrder())
                if (row.DataContext is TreeNode node && TopInView(row) >= 0)
                    return new AnchoredRow(node, TopInView(row));

            return null;
        }

        public void KeepStill(AnchoredRow? anchor, Func<IReadOnlyList<TreeNode>> close)
        {
            hold.Stop();
            var closed = close();
            rows.ApplyTreeChanges();
            if (anchor is not { } kept)
                return;

            scroller.UpdateLayout();
            Place(kept);
            if (closed.Count > 0 && TopLevel.GetTopLevel(scroller) is { } top)
                hold.Run(top, () => Place(kept), () => IsSliding(kept));
        }

        public void EndHold() => hold.Stop();

        private void Place(AnchoredRow kept)
        {
            if (rows.RowOf(kept.Node) is not { } row)
                return;

            var top = TopInView(row);
            var maxOffset = Math.Max(0, scroller.Extent.Height - scroller.Viewport.Height);
            var offset = Math.Clamp(scroller.Offset.Y + top - kept.Top, 0, maxOffset);
            if (Math.Abs(offset - scroller.Offset.Y) >= MinMove)
                scroller.Offset = scroller.Offset.WithY(offset);
        }

        private bool IsSliding(AnchoredRow kept) =>
            rows.RowOf(kept.Node) is { Place: { } place } row && Math.Abs(PanelListTreeRows.GetTop(row) - place.Y) >= MinMove;

        private IEnumerable<RowListTree> RowsInOrder() =>
            rows.Layout.Places.Select(place => rows.RowOf(place.Key)).OfType<RowListTree>();

        // From the top of the row and the offset, not from the screen, so a step before the next layout reads the new offset.
        private double TopInView(RowListTree row) => rows.Bounds.Y + PanelListTreeRows.GetTop(row) - scroller.Offset.Y;
    }
}
