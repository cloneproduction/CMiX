// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using CMiX.UI.Behaviors;

namespace CMiX.UI.AttachedProperties
{
    // Closes a context menu when the pointer goes away from it. The theme ContextMenuDefault switches it on,
    // so every context menu with the default theme has it. Set IsEnabled to false on a menu to switch it off.
    public static class ContextMenuPointerAway
    {
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<ContextMenu, ContextMenu, bool>("IsEnabled");

        public static readonly AttachedProperty<double> DistanceProperty =
            AvaloniaProperty.RegisterAttached<ContextMenu, ContextMenu, double>("Distance", PointerAwayCloser.DefaultDistance);

        public static void SetIsEnabled(ContextMenu menu, bool value) => menu.SetValue(IsEnabledProperty, value);
        public static bool GetIsEnabled(ContextMenu menu) => menu.GetValue(IsEnabledProperty);

        public static void SetDistance(ContextMenu menu, double value) => menu.SetValue(DistanceProperty, value);
        public static double GetDistance(ContextMenu menu) => menu.GetValue(DistanceProperty);

        private static readonly ConditionalWeakTable<ContextMenu, Hook> hooks = new();

        static ContextMenuPointerAway()
        {
            IsEnabledProperty.Changed.AddClassHandler<ContextMenu>(OnIsEnabledChanged);
        }

        private static void OnIsEnabledChanged(ContextMenu menu, AvaloniaPropertyChangedEventArgs e)
        {
            if (hooks.TryGetValue(menu, out var oldHook))
            {
                oldHook.Detach();
                hooks.Remove(menu);
            }

            if (e.GetNewValue<bool>())
                hooks.Add(menu, new Hook(menu));
        }

        // Watches the opening and the closing of one menu. The table of hooks keeps it as long as the menu lives.
        private sealed class Hook
        {
            private readonly ContextMenu menu;
            private readonly PointerAwayCloser closer;

            public Hook(ContextMenu menu)
            {
                this.menu = menu;
                closer = new PointerAwayCloser(() => GetDistance(menu), () => menu, () => Owner, menu.Close);
                menu.Opened += OnOpened;
                menu.Closed += OnClosed;
            }

            // A menu opened by a right click has no PlacementTarget. The popup that holds the menu has it.
            private Control? Owner => menu.PlacementTarget ?? (menu.Parent as Popup)?.PlacementTarget;

            public void Detach()
            {
                menu.Opened -= OnOpened;
                menu.Closed -= OnClosed;
                closer.Stop();
            }

            private void OnOpened(object? sender, RoutedEventArgs e)
            {
                var owner = Owner;
                closer.Start(owner == null ? null : TopLevel.GetTopLevel(owner));
            }

            private void OnClosed(object? sender, RoutedEventArgs e) => closer.Stop();
        }
    }
}
