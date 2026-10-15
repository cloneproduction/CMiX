// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CMiX.UI.Behaviors;
using AvaloniaInteraction = Avalonia.Xaml.Interactivity.Interaction;

namespace CMiX.UI.AttachedProperties
{
    // The standard way to open a Popup with a ToggleButton. Set it on the Popup:
    //     <Popup ui:TogglePopup.Toggle="{Binding ElementName=MyToggle}">
    // The popup is open while the toggle is checked. The popup has the toggle as its placement target,
    // no light dismiss, and the behavior ClosePopupBehavior, which closes it when the pointer goes away.
    // The content of the popup is at least PopupMinWidth wide. With MatchesToggleWidth the popup is as wide as the toggle instead:
    //     <Popup ui:TogglePopup.Toggle="{Binding ElementName=MyToggle}" ui:TogglePopup.MatchesToggleWidth="True">
    public static class TogglePopup
    {
        public static readonly AttachedProperty<ToggleButton?> ToggleProperty =
            AvaloniaProperty.RegisterAttached<Popup, ToggleButton?>("Toggle", typeof(TogglePopup));

        public static void SetToggle(Popup popup, ToggleButton? value) => popup.SetValue(ToggleProperty, value);
        public static ToggleButton? GetToggle(Popup popup) => popup.GetValue(ToggleProperty);

        // True: the popup is as wide as its toggle, also when the toggle changes its width.
        // False: the content of the popup is at least PopupMinWidth wide.
        public static readonly AttachedProperty<bool> MatchesToggleWidthProperty =
            AvaloniaProperty.RegisterAttached<Popup, bool>("MatchesToggleWidth", typeof(TogglePopup));

        public static void SetMatchesToggleWidth(Popup popup, bool value) => popup.SetValue(MatchesToggleWidthProperty, value);
        public static bool GetMatchesToggleWidth(Popup popup) => popup.GetValue(MatchesToggleWidthProperty);

        private static readonly ConditionalWeakTable<Popup, Hook> hooks = new();

        static TogglePopup()
        {
            ToggleProperty.Changed.AddClassHandler<Popup>(OnToggleChanged);
        }

        private static void OnToggleChanged(Popup popup, AvaloniaPropertyChangedEventArgs e)
        {
            if (hooks.TryGetValue(popup, out var oldHook))
            {
                oldHook.Detach();
                hooks.Remove(popup);
            }

            if (e.GetNewValue<ToggleButton?>() is { } toggle)
                hooks.Add(popup, new Hook(popup, toggle));
        }

        // Keeps one popup and one toggle in step. The table of hooks keeps it as long as the popup lives.
        private sealed class Hook
        {
            private readonly Popup popup;
            private readonly ToggleButton toggle;

            public Hook(Popup popup, ToggleButton toggle)
            {
                this.popup = popup;
                this.toggle = toggle;

                // ClosePopupBehavior closes the popup, so the light dismiss of Avalonia must not.
                popup.IsLightDismissEnabled = false;
                popup.PlacementTarget = toggle;

                var behaviors = AvaloniaInteraction.GetBehaviors(popup);
                if (!behaviors.OfType<ClosePopupBehavior>().Any())
                    behaviors.Add(new ClosePopupBehavior());

                toggle.IsCheckedChanged += OnToggleChecked;
                toggle.PropertyChanged += OnToggleChanged;
                popup.Opened += OnPopupOpened;
                popup.Closed += OnPopupClosed;
                popup.IsOpen = toggle.IsChecked == true;
            }

            public void Detach()
            {
                toggle.IsCheckedChanged -= OnToggleChecked;
                toggle.PropertyChanged -= OnToggleChanged;
                popup.Opened -= OnPopupOpened;
                popup.Closed -= OnPopupClosed;
                if (ReferenceEquals(popup.PlacementTarget, toggle))
                    popup.PlacementTarget = null;
            }

            private void OnToggleChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
                popup.IsOpen = toggle.IsChecked == true;

            private void OnPopupOpened(object? sender, System.EventArgs e)
            {
                toggle.IsChecked = true;
                ApplyWidth();
            }

            private void OnToggleChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
            {
                if (e.Property == Visual.BoundsProperty && GetMatchesToggleWidth(popup))
                    ApplyWidth();
            }

            // Read when the popup opens, so the order of the properties in the AXAML does not matter.
            private void ApplyWidth()
            {
                if (GetMatchesToggleWidth(popup))
                {
                    popup.Width = toggle.Bounds.Width;
                    return;
                }

                popup.Width = double.NaN;
                if (popup.Child is Control child && popup.TryFindResource("PopupMinWidth", out var minWidth) && minWidth is double width)
                    child.MinWidth = width;
            }

            private void OnPopupClosed(object? sender, System.EventArgs e) => toggle.IsChecked = false;
        }
    }
}
