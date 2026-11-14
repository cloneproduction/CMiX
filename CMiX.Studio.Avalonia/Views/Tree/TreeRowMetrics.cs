// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // Sets the sizes of the rows of the project tree in its resources, and the room under the last row.
    public sealed class TreeRowMetrics
    {
        // The space between two rows, one above the other, at a screen of 100%. The drop line (SplitterIndicatorSize) sits in it.
        // It is the token TreeRowGap. A test pins the two.
        public const double RowGap = 3;

        private readonly TreeView treeView;
        private readonly IResourceDictionary resources;
        private readonly Func<ScrollViewer?> scroller;

        // resources is the dictionary that the rows read. scroller gives the scroll viewer of the tree.
        public TreeRowMetrics(TreeView treeView, IResourceDictionary resources, Func<ScrollViewer?> scroller)
        {
            this.treeView = treeView;
            this.resources = resources;
            this.scroller = scroller;
        }

        // The height of a row plus the space under it.
        // Before SetRowMetrics, the view has no TreeRowGap of its own, and the gap is RowGap.
        public double RowPitch =>
            Convert.ToDouble(resources["TreeRowHeight"]) + (resources.TryGetValue("TreeRowGap", out var gap) ? Convert.ToDouble(gap) : RowGap);

        // The height of a row and the space between two rows, in whole pixels of the screen.
        // On a screen with a scaling like 125%, a size of 24 or of 1 is not a whole number of pixels, and every panel rounds its
        // size up. The rows of a long tree are then slightly more than one pitch apart, and the error adds up. With whole pixels
        // there is nothing to round, and every row is exactly one pitch from the next, whatever is open.
        // The height goes up to the next pixel, and the space is one pixel or more.
        public static (double Height, double Spacing) SnapToPixels(double height, double spacing, double scale)
        {
            var heightPixels = Math.Ceiling(height * scale - 1e-6);
            var spacingPixels = Math.Max(1, Math.Round(spacing * scale, MidpointRounding.AwayFromZero));
            return (heightPixels / scale, spacingPixels / scale);
        }

        public void SetRowMetrics(double scale)
        {
            var height = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));
            var spacing = Convert.ToDouble(Application.Current!.FindResource("SpacingXXS"));
            var (snappedHeight, snappedSpacing) = SnapToPixels(height, spacing, scale);
            resources["TreeRowHeight"] = snappedHeight;
            resources["TreeRowSpacing"] = snappedSpacing;
            resources["TreeToggleWidth"] = RowToggles.WidthFor(snappedHeight, snappedSpacing);

            // The space between rows is a space of its own, not the space between a row and its toggles.
            var (_, snappedGap) = SnapToPixels(height, RowGap, scale);
            resources["TreeRowGap"] = snappedGap;

            // The drop line is one pixel of the screen, from the left edge of the tree to its right edge, in the middle of the space under a
            // row (dropgap), or in the middle of the space above the first row (droptop): the same room above and below it.
            var line = 1 / scale;
            var free = (snappedGap - line) / 2;
            resources["TreeDropLineThickness"] = line;
            resources["TreeDropLineGapMargin"] = new Thickness(0, snappedHeight + free, 0, 0);
            resources["TreeDropLineTopMargin"] = new Thickness(0, -(free + line), 0, 0);

            // The frame of a blocked row (BlockedFrame) is one pixel of the screen, so it is as thin on a screen of 150% as on a screen of 100%.
            resources["TreeSelectionThickness"] = new Thickness(1 / scale);
        }

        // A tree that is higher than its view can scroll until its last row is near the top of the view. When a drop takes rows away,
        // the view can then stay where it was, since there is always room under the rows. A tree that fits in its view has no room.
        public void SetEndRoom()
        {
            if (scroller() is not { } view)
                return;

            var content = view.Extent.Height - treeView.Padding.Bottom;
            var room = content > view.Viewport.Height + 0.5 ? Math.Max(0, view.Viewport.Height - 3 * RowPitch) : 0;
            if (Math.Abs(treeView.Padding.Bottom - room) > 0.5)
                treeView.Padding = new Thickness(0, 0, 0, room);
        }
    }
}
