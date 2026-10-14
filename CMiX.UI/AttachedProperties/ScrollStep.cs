// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.UI.AttachedProperties
{
    // Makes the mouse wheel move a ScrollViewer by whole items. The default step is 50 pixels, which
    // is not a multiple of the height of an item, so a row is cut at the top and at the bottom.
    // ItemHeight is the height of one item. Zero, the default, leaves the wheel as it is.
    public static class ScrollStep
    {
        public static readonly AttachedProperty<double> ItemHeightProperty =
            AvaloniaProperty.RegisterAttached<ScrollViewer, double>(
                "ItemHeight",
                typeof(ScrollStep));

        public static double GetItemHeight(ScrollViewer obj) => obj.GetValue(ItemHeightProperty);
        public static void SetItemHeight(ScrollViewer obj, double value) => obj.SetValue(ItemHeightProperty, value);

        static ScrollStep()
        {
            ItemHeightProperty.Changed.AddClassHandler<ScrollViewer>(OnItemHeightChanged);
        }

        private static void OnItemHeightChanged(ScrollViewer scroll, AvaloniaPropertyChangedEventArgs e)
        {
            scroll.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);

            if (e.GetNewValue<double>() > 0)
                scroll.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
        }

        private static void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            var scroll = (ScrollViewer)sender!;
            var step = GetItemHeight(scroll);

            if (step <= 0 || e.Delta.Y == 0)
                return;

            var maximum = Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height);
            var row = Math.Round(scroll.Offset.Y / step) + (e.Delta.Y > 0 ? -1 : 1);

            scroll.Offset = scroll.Offset.WithY(Math.Clamp(row * step, 0, maximum));
            e.Handled = true;
        }
    }
}
