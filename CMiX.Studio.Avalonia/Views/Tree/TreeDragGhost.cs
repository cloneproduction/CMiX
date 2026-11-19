// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using CMiX.UI.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // Avalonia has no drag image for a pointer drag, so the label is a control on the overlay layer.
    // The label is above the pointer, so the row under the pointer and the drop line stay in sight.
    internal sealed class TreeDragGhost
    {
        private readonly Border ghost = new() { IsHitTestVisible = false, ClipToBounds = true };
        private OverlayLayer? layer;

        private double iconMiddle;

        // The token SpacingSM.
        private double gap = 4;

        public bool IsShown => layer != null;

        internal Border Ghost => ghost;

        public void Show(Border row, string iconKey, string name, Control reference, Point position)
        {
            Hide();
            if (OverlayLayer.GetOverlayLayer(reference) is not { } overlay || row.Bounds.Width <= 0 || row.Bounds.Height <= 0)
                return;

            gap = TreeResources.Read(row, "SpacingSM", 4.0);
            ghost.Opacity = TreeResources.Read(row, "OpacityDragGhost", 1.0);
            Build(row, iconKey, name);
            ghost.Height = row.Bounds.Height;
            ghost.Measure(Size.Infinity);

            layer = overlay;
            overlay.Children.Add(ghost);
            Move(reference, position, null);
        }

        // A drop line just above the pointer would be under the label, so the label goes above the line.
        public void Move(Control reference, Point position, double? lineTop = null)
        {
            if (layer == null || reference.TranslatePoint(position, layer) is not { } point)
                return;

            var width = ghost.DesiredSize.Width;
            var height = ghost.Height;
            var maxLeft = Math.Max(0, layer.Bounds.Width - width);
            var left = Math.Clamp(point.X - iconMiddle, 0, maxLeft);
            var bottom = point.Y - gap;
            if (lineTop is { } y && reference.TranslatePoint(new Point(0, y), layer) is { } line && line.Y < point.Y)
                bottom = Math.Min(bottom, line.Y - gap);

            var top = bottom - height;
            if (top < 0)
                top = point.Y + gap;

            Canvas.SetLeft(ghost, left);
            Canvas.SetTop(ghost, top);
        }

        public void Hide()
        {
            layer?.Children.Remove(ghost);
            layer = null;
            ghost.Background = null;
            ghost.Child = null;
        }

        // The space after the name is the space before the icon, so the label is even at both ends.
        private void Build(Border row, string iconKey, string name)
        {
            ghost.Background = row.Background;
            ghost.CornerRadius = row.CornerRadius;

            var headerMargin = TreeResources.Read(row, "TreeHeaderMargin", new Thickness(4, 0));
            var contentMargin = TreeResources.Read(row, "TreeRowContentMargin", default(Thickness));
            var iconSize = TreeResources.Read(row, "IconSizeXS", 12.0);
            var foreground = TextElement.GetForeground(row);
            ghost.Padding = new Thickness(contentMargin.Left, 0, contentMargin.Left, 0);
            iconMiddle = ghost.Padding.Left + headerMargin.Left + iconSize / 2;

            var content = new StackPanel { Orientation = Orientation.Horizontal, IsHitTestVisible = false };
            content.Children.Add(new AppIcon
            {
                IconKey = iconKey,
                Size = iconSize,
                Width = iconSize,
                Margin = headerMargin,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = foreground
            });
            content.Children.Add(new TextBlock
            {
                Text = name,
                Margin = headerMargin,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = foreground
            });

            ghost.Child = content;
        }
    }
}
