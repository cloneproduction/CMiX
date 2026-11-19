// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public sealed class TreeRowMetrics
    {
        // It is the token TreeRowGap. A test pins the two.
        public const double RowGap = 3;

        private readonly IResourceDictionary resources;
        private readonly TreeViewContext context;

        public TreeRowMetrics(IResourceDictionary resources, TreeViewContext context)
        {
            this.resources = resources;
            this.context = context;
        }

        public double RowPitch => context.RowPitch;

        public static double ReadRowPitch(Control view) =>
            ReadSize(view, TreeRowResources.RowHeightKey) + ReadSize(view, TreeRowResources.RowGapKey);

        // A view out of a window has no parent, so its search stops before the tokens of the application.
        private static double ReadSize(Control view, string key) =>
            Convert.ToDouble(view.TryFindResource(key, out var value) ? value : Application.Current!.FindResource(key));

        // With a scale like 125%, sizes are not whole pixels and the rounding error adds up over a long tree.
        // Whole pixels leave nothing to round, so every row is one pitch from the next.
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

            // The drop line is one pixel of the screen, in the middle of the space.
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

        // Room under the rows lets the view stay where it was when a drop takes rows away.
        public void SetEndRoom()
        {
            if (context.Scroller is not { } view)
                return;

            var treeView = context.TreeView;
            var content = view.Extent.Height - treeView.Padding.Bottom;
            var room = content > view.Viewport.Height + 0.5 ? Math.Max(0, view.Viewport.Height - 3 * RowPitch) : 0;
            if (Math.Abs(treeView.Padding.Bottom - room) > 0.5)
                treeView.Padding = new Thickness(0, 0, 0, room);
        }
    }
}
