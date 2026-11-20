// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CMiX.UI.AttachedProperties
{
    // Marks a toggle button as part of a group that a drag can paint (see ToggleDragBehavior).
    // A drag that starts on a button paints only the buttons with the same group name.
    // A button with a group toggles when the mouse button goes down, not when it comes up. So the first button
    // of a drag changes at once and the drag can paint the others with its state.
    public static class ToggleDrag
    {
        public static readonly AttachedProperty<string?> GroupProperty =
            AvaloniaProperty.RegisterAttached<ToggleButton, string?>("Group", typeof(ToggleDrag));

        static ToggleDrag()
        {
            GroupProperty.Changed.AddClassHandler<ToggleButton>((button, e) =>
                button.ClickMode = string.IsNullOrEmpty(e.NewValue as string) ? ClickMode.Release : ClickMode.Press);
        }

        public static string? GetGroup(ToggleButton button) => button.GetValue(GroupProperty);

        public static void SetGroup(ToggleButton button, string? value) => button.SetValue(GroupProperty, value);
    }
}
