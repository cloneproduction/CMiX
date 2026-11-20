// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace CMiX.UI.AttachedProperties
{
    // A menu item with this property runs its command after its menu has closed, not while the popup is still open.
    public static class RunCommandAfterClose
    {
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<MenuItem, bool>("IsEnabled", typeof(RunCommandAfterClose));

        static RunCommandAfterClose()
        {
            IsEnabledProperty.Changed.AddClassHandler<MenuItem>((item, e) =>
            {
                if (e.NewValue is true)
                    AddHandlers(item);
                else
                    RemoveHandlers(item);
            });
        }

        public static bool GetIsEnabled(MenuItem item) => item.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(MenuItem item, bool value) => item.SetValue(IsEnabledProperty, value);

        private static void AddHandlers(MenuItem item)
        {
            item.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            item.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        private static void RemoveHandlers(MenuItem item)
        {
            item.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            item.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        }

        private static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (sender is MenuItem item && e.InitialPressMouseButton == MouseButton.Left)
                CloseThenRun(item, e);
        }

        private static void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (sender is MenuItem item && e.Key == Key.Enter)
                CloseThenRun(item, e);
        }

        // Takes the input from the menu, closes the menu, and posts the command as the menu click would run it.
        private static void CloseThenRun(MenuItem item, RoutedEventArgs e)
        {
            if (item.HasSubMenu || !item.IsEffectivelyEnabled)
                return;

            e.Handled = true;
            var (command, parameter) = (item.Command, item.CommandParameter);
            item.FindLogicalAncestorOfType<MenuBase>()?.Close();
            Dispatcher.UIThread.Post(() =>
            {
                if (command?.CanExecute(parameter) == true)
                    command.Execute(parameter);
            });
        }
    }
}
