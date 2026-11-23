// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // Keeps one row of the new tree still on the screen while cards close above it, as TreeScrollAnchor does in the old tree.
    // The rows slide to their new places, so the anchor is put back in each frame of the slide.
    internal sealed class ListTreeScrollAnchor : ListTreeBehavior
    {
        // At a scale of 125% or 150% the scroll rounds to pixels, so a smaller change would scroll again at each frame.
        private const double MinMove = 0.5;

        private readonly FrameHold hold = new();

        private PanelListTreeRows Rows => Context.Rows;

        private ScrollViewer Scroller => Context.Scroller;

        // The row of the node, when the whole row is in view.
        public AnchoredRow? CaptureRow(TreeNode node)
        {
            if (Rows.RowOf(node) is not { } row)
                return null;

            var top = TopInView(row);
            return top >= 0 && top + row.Bounds.Height <= Scroller.Viewport.Height ? new AnchoredRow(node, top) : null;
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
            Rows.ApplyTreeChanges();
            if (anchor is not { } kept)
                return;

            Scroller.UpdateLayout();
            Place(kept);
            if (closed.Count > 0 && TopLevel.GetTopLevel(Scroller) is { } top)
                hold.Run(top, () => Place(kept), () => IsSliding(kept));
        }

        public void EndHold() => hold.Stop();

        private void Place(AnchoredRow kept)
        {
            if (Rows.RowOf(kept.Node) is not { } row)
                return;

            var top = TopInView(row);
            var maxOffset = Math.Max(0, Scroller.Extent.Height - Scroller.Viewport.Height);
            var offset = Math.Clamp(Scroller.Offset.Y + top - kept.Top, 0, maxOffset);
            if (Math.Abs(offset - Scroller.Offset.Y) >= MinMove)
                Scroller.Offset = Scroller.Offset.WithY(offset);
        }

        private bool IsSliding(AnchoredRow kept) =>
            Rows.RowOf(kept.Node) is { Place: { } place } row && Math.Abs(PanelListTreeRows.GetTop(row) - place.Y) >= MinMove;

        private IEnumerable<RowListTree> RowsInOrder() =>
            Rows.Layout.Places.Select(place => Rows.RowOf(place.Key)).OfType<RowListTree>();

        // From the top of the row and the offset, not from the screen, so a step before the next layout reads the new offset.
        private double TopInView(RowListTree row) => Rows.Bounds.Y + PanelListTreeRows.GetTop(row) - Scroller.Offset.Y;
    }
}
