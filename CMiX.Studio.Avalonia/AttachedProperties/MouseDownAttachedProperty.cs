// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.AttachedProperties
{
    public static class MouseDownHelper
    {
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsEnabled",
                typeof(MouseDownHelper));

        public static void SetIsEnabled(Control element, bool value) => element.SetValue(IsEnabledProperty, value);
        public static bool GetIsEnabled(Control element) => element.GetValue(IsEnabledProperty);

        // Avalonia has no read only attached properties; the setters stay internal by convention.
        public static readonly AttachedProperty<bool> IsMouseDownProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsMouseDown",
                typeof(MouseDownHelper));

        internal static void SetIsMouseDown(Control element, bool value) => element.SetValue(IsMouseDownProperty, value);
        public static bool GetIsMouseDown(Control element) => element.GetValue(IsMouseDownProperty);

        public static readonly AttachedProperty<bool> IsMouseLeftButtonDownProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsMouseLeftButtonDown",
                typeof(MouseDownHelper));

        internal static void SetIsMouseLeftButtonDown(Control element, bool value) => element.SetValue(IsMouseLeftButtonDownProperty, value);
        public static bool GetIsMouseLeftButtonDown(Control element) => element.GetValue(IsMouseLeftButtonDownProperty);

        static MouseDownHelper()
        {
            IsEnabledProperty.Changed.AddClassHandler<Control>(OnIsEnabledChanged);
        }

        private static void OnIsEnabledChanged(Control element, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.GetNewValue<bool>())
                Register(element);
            else
                UnRegister(element);
        }

        private static void Register(Control element)
        {
            element.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            element.AddHandler(InputElement.PointerExitedEvent, OnPointerExited);
            element.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
        }

        private static void UnRegister(Control element)
        {
            element.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            element.RemoveHandler(InputElement.PointerExitedEvent, OnPointerExited);
            element.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
        }

        private static void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var element = (Control)sender;
            SetIsMouseDown(element, true);
            if (e.GetCurrentPoint(element).Properties.IsLeftButtonPressed)
                SetIsMouseLeftButtonDown(element, true);
        }

        private static void OnPointerExited(object sender, PointerEventArgs e)
        {
            var element = (Control)sender;
            SetIsMouseDown(element, false);
            SetIsMouseLeftButtonDown(element, false);
        }

        private static void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var element = (Control)sender;
            SetIsMouseDown(element, false);
            SetIsMouseLeftButtonDown(element, false);
        }
    }
}
