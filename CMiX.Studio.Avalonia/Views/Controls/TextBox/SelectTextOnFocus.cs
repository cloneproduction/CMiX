// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Attached property that selects the whole text when a TextBox receives focus.
    public static class SelectTextOnFocus
    {
        public static readonly AttachedProperty<bool> ActiveProperty =
            AvaloniaProperty.RegisterAttached<TextBox, bool>(
                "Active",
                typeof(SelectTextOnFocus));

        public static bool GetActive(TextBox obj) => obj.GetValue(ActiveProperty);
        public static void SetActive(TextBox obj, bool value) => obj.SetValue(ActiveProperty, value);

        static SelectTextOnFocus()
        {
            ActiveProperty.Changed.AddClassHandler<TextBox>(ActivePropertyChanged);
        }

        private static void ActivePropertyChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
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

        private static void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static void OnGotFocusSelectText(object sender, GotFocusEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.SelectAll();
        }
    }
}
