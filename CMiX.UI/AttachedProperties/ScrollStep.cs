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
    // VisibleItemCount, when it is above zero, makes the ScrollViewer as high as that many items.
    public static class ScrollStep
    {
        public static readonly AttachedProperty<double> ItemHeightProperty =
            AvaloniaProperty.RegisterAttached<ScrollViewer, double>(
                "ItemHeight",
                typeof(ScrollStep));

        public static double GetItemHeight(ScrollViewer obj) => obj.GetValue(ItemHeightProperty);
        public static void SetItemHeight(ScrollViewer obj, double value) => obj.SetValue(ItemHeightProperty, value);

        public static readonly AttachedProperty<int> VisibleItemCountProperty =
            AvaloniaProperty.RegisterAttached<ScrollViewer, int>(
                "VisibleItemCount",
                typeof(ScrollStep));

        public static int GetVisibleItemCount(ScrollViewer obj) => obj.GetValue(VisibleItemCountProperty);
        public static void SetVisibleItemCount(ScrollViewer obj, int value) => obj.SetValue(VisibleItemCountProperty, value);

        static ScrollStep()
        {
            ItemHeightProperty.Changed.AddClassHandler<ScrollViewer>(OnItemHeightChanged);
            VisibleItemCountProperty.Changed.AddClassHandler<ScrollViewer>((scroll, _) => UpdateMaxHeight(scroll));
        }

        private static void UpdateMaxHeight(ScrollViewer scroll)
        {
            var step = GetItemHeight(scroll);
            var count = GetVisibleItemCount(scroll);

            if (step > 0 && count > 0)
                scroll.MaxHeight = count * step;
        }

        private static void OnItemHeightChanged(ScrollViewer scroll, AvaloniaPropertyChangedEventArgs e)
        {
            UpdateMaxHeight(scroll);

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

            var target = Math.Clamp(row * step, 0, maximum);

            // At the start or at the end, and in a list that does not scroll, the wheel goes on to the
            // panel around the list.
            if (Math.Abs(target - scroll.Offset.Y) < 0.5)
                return;

            scroll.Offset = scroll.Offset.WithY(target);
            e.Handled = true;
        }
    }
}
