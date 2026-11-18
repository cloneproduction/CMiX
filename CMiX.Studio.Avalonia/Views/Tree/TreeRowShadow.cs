// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace CMiX.Studio.Avalonia.Views
{
    // The shadow lies in a layer over every row, so it falls on the rows around it whatever their drawing order.
    internal sealed class TreeRowShadow
    {
        private const string RowBorderName = "RowBorder";

        private readonly TreeView tree;
        private readonly Canvas layer;
        private readonly Border shadow;

        public TreeRowShadow(TreeView tree, Canvas layer, Border shadow)
        {
            this.tree = tree;
            this.layer = layer;
            this.shadow = shadow;
            tree.LayoutUpdated += (_, _) => Follow();
            // A layout does not search the rows: a large composition has many.
            TreeViewItem.IsSelectedProperty.Changed.AddClassHandler<TreeViewItem>((item, e) =>
            {
                if (e.GetNewValue<bool>() && item.FindAncestorOfType<TreeView>() == tree)
                    selectedItem = item;
                else if (ReferenceEquals(item, selectedItem))
                    selectedItem = null;
            });
        }

        private TreeViewItem? selectedItem;

        private void Follow()
        {
            var row = SelectedRowBorder();
            var origin = row?.TranslatePoint(new Point(), layer);
            if (row is null || origin is null || !row.IsEffectivelyVisible)
            {
                Fade(0);
                return;
            }

            // A write in LayoutUpdated starts another layout, and a write during a slide restarts it.
            Set(Canvas.LeftProperty, origin.Value.X);
            Set(Canvas.TopProperty, origin.Value.Y);
            Set(Layoutable.WidthProperty, row.Bounds.Width);
            Set(Layoutable.HeightProperty, row.Bounds.Height);
            Fade(1);
        }

        private readonly Dictionary<AvaloniaProperty, double> written = new();

        private void Set(StyledProperty<double> property, double value)
        {
            if (written.TryGetValue(property, out var last) && last == value)
                return;
            written[property] = value;
            shadow.SetValue(property, value);
        }

        // A slide between rows read as a wobble.
        private void Fade(double opacity) => Set(Visual.OpacityProperty, opacity);

        private Border? SelectedRowBorder() =>
            selectedItem?.GetVisualDescendants().OfType<Border>().FirstOrDefault(border => border.Name == RowBorderName && border.TemplatedParent == selectedItem);
    }
}
