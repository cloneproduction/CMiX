// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.AttachedProperties
{
    // Which row of a menu opens under the pointer, as in Blender. The menu that opens at the pointer puts
    // the middle of the anchor row on the pointer (see PopupFrame).
    // The window remembers the row that the user used last, so the next menu can open on the same row.
    public static class MenuAnchor
    {
        public static readonly AttachedProperty<bool> IsAnchorProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("IsAnchor", typeof(MenuAnchor));

        public static void SetIsAnchor(Control element, bool value) => element.SetValue(IsAnchorProperty, value);
        public static bool GetIsAnchor(Control element) => element.GetValue(IsAnchorProperty);

        // The header of the row that was used last in a menu of this window. It is null before the first use.
        public static readonly AttachedProperty<object?> LastUsedProperty =
            AvaloniaProperty.RegisterAttached<Control, object?>("LastUsed", typeof(MenuAnchor));

        public static void SetLastUsed(Control window, object? value) => window.SetValue(LastUsedProperty, value);
        public static object? GetLastUsed(Control window) => window.GetValue(LastUsedProperty);

        static MenuAnchor()
        {
            // A new anchor moves the menu, so the frame around the row arranges again.
            IsAnchorProperty.Changed.AddClassHandler<Control>((row, _) => row.FindAncestorOfType<PopupFrame>()?.InvalidateArrange());
        }
    }
}
