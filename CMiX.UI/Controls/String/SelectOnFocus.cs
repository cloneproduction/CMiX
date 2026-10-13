// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.UI.Controls.String
{
    // Attached property that selects the whole text when a TextBox receives focus.
    public static class SelectOnFocus
    {
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<TextBox, bool>(
                "IsEnabled",
                typeof(SelectOnFocus));

        public static bool GetIsEnabled(TextBox obj) => obj.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(TextBox obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        static SelectOnFocus()
        {
            IsEnabledProperty.Changed.AddClassHandler<TextBox>(IsEnabledPropertyChanged);
        }

        private static void IsEnabledPropertyChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.GetNewValue<bool>())
            {
                textBox.GotFocus += OnGotFocusSelectText;
                textBox.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            }
            else
            {
                textBox.GotFocus -= OnGotFocusSelectText;
                textBox.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            }
        }

        private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var textBox = (TextBox)sender!;
            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static void OnGotFocusSelectText(object? sender, GotFocusEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.SelectAll();
        }
    }
}
