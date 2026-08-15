// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.AttachedProperties
{
    public static class DragHandler
    {
        public static readonly AttachedProperty<bool> IsPressedProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsPressed",
                typeof(DragHandler),
                defaultBindingMode: BindingMode.TwoWay);

        public static bool GetIsPressed(Control obj) => obj.GetValue(IsPressedProperty);
        public static void SetIsPressed(Control obj, bool value) => obj.SetValue(IsPressedProperty, value);

        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsEnabled",
                typeof(DragHandler));

        public static bool GetIsEnabled(Control obj) => obj.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(Control obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        static DragHandler()
        {
            IsEnabledProperty.Changed.AddClassHandler<Control>(OnIsEnabledChanged);
        }

        private static void OnIsEnabledChanged(Control element, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.GetNewValue<bool>())
            {
                element.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
                element.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            }
            else
            {
                element.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
                element.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            }
        }

        private static void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed)
                ((Control)sender).SetValue(IsPressedProperty, true);
        }

        private static void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
                ((Control)sender).SetValue(IsPressedProperty, false);
        }
    }
}
