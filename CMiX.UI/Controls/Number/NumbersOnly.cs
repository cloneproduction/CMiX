// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.UI.Controls.Number
{
    // Attached property that restricts a TextBox to digit input.
    public static class NumbersOnly
    {
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<TextBox, bool>(
                "IsEnabled",
                typeof(NumbersOnly));

        public static bool GetIsEnabled(TextBox obj) => obj.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(TextBox obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        static NumbersOnly()
        {
            IsEnabledProperty.Changed.AddClassHandler<TextBox>(IsEnabledPropertyChanged);
        }

        private static void IsEnabledPropertyChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.GetNewValue<bool>())
            {
                textBox.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
                textBox.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
                textBox.PastingFromClipboard += OnPaste;
            }
            else
            {
                textBox.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
                textBox.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
                textBox.PastingFromClipboard -= OnPaste;
            }
        }

        private static void OnTextInput(object? sender, TextInputEventArgs e)
        {
            if (e.Text != null && e.Text.Any(c => !char.IsDigit(c)))
                e.Handled = true;
        }

        private static void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private static async void OnPaste(object? sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender!;
            var topLevel = TopLevel.GetTopLevel(textBox);
            if (topLevel?.Clipboard == null)
            {
                e.Handled = true;
                return;
            }

            var text = await topLevel.Clipboard.GetTextAsync();
            if (text == null || text.Trim().Any(c => !char.IsDigit(c)))
                e.Handled = true;
        }
    }
}
