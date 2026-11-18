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
        private readonly TreeViewContext context;

        // resources is the dictionary that the rows read.
        public TreeRowMetrics(TreeView treeView, IResourceDictionary resources, TreeViewContext context)
        {
            this.treeView = treeView;
            this.resources = resources;
            this.context = context;
        }

        // The height of a row plus the space under it.
        public double RowPitch => context.RowPitch;

        public static double ReadRowPitch(Control view) =>
            ReadSize(view, TreeRowResources.RowHeightKey) + ReadSize(view, TreeRowResources.RowGapKey);

        // A view out of a window has no parent, so its search stops before the tokens of the application.
        private static double ReadSize(Control view, string key) =>
            Convert.ToDouble(view.TryFindResource(key, out var value) ? value : Application.Current!.FindResource(key));

        // The height of a row and the space between two rows, in whole pixels of the screen.
        // On a screen with a scaling like 125%, a size of 24 or of 1 is not a whole number of pixels, and every panel rounds its
        // size up. The rows of a long tree are then slightly more than one pitch apart, and the error adds up. With whole pixels
        // there is nothing to round, and every row is exactly one pitch from the next, whatever is open.
        // The height goes up to the next pixel, and the space is one pixel or more.
        public static RowSizes SnapToPixels(double height, double spacing, double scale)
        {
            var heightPixels = Math.Ceiling(height * scale - 1e-6);
            var spacingPixels = Math.Max(1, Math.Round(spacing * scale, MidpointRounding.AwayFromZero));
            return new RowSizes(heightPixels / scale, spacingPixels / scale);
        }

        public void SetRowMetrics(double scale) => ComputeRowResources(scale).Write(resources);

        private static TreeRowResources ComputeRowResources(double scale)
        {
            var height = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));
            var spacing = Convert.ToDouble(Application.Current!.FindResource("SpacingXXS"));
            var row = SnapToPixels(height, spacing, scale);

            // The space between rows is a space of its own, not the space between a row and its toggles.
            var gap = SnapToPixels(height, RowGap, scale).Spacing;

            // The drop line is one pixel of the screen, in the middle of the space under a row (dropgap) or above the first row (droptop).
            var line = 1 / scale;
            var free = (gap - line) / 2;

            return new TreeRowResources(
                row.Height,
                row.Spacing,
                RowToggles.WidthFor(row.Height, row.Spacing),
                gap,
                line,
                new Thickness(0, row.Height + free, 0, 0),
                new Thickness(0, -(free + line), 0, 0),
                new Thickness(1 / scale));
        }

        // A tree that is higher than its view can scroll until its last row is near the top of the view. When a drop takes rows away,
        // the view can then stay where it was, since there is always room under the rows. A tree that fits in its view has no room.
        public void SetEndRoom()
        {
            if (context.Scroller is not { } view)
                return;

            var content = view.Extent.Height - treeView.Padding.Bottom;
            var room = content > view.Viewport.Height + 0.5 ? Math.Max(0, view.Viewport.Height - 3 * RowPitch) : 0;
            if (Math.Abs(treeView.Padding.Bottom - room) > 0.5)
                treeView.Padding = new Thickness(0, 0, 0, room);
        }
    }
}
