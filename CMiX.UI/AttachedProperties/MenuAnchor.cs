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
    // A row is known by its Id, never by its label. The label is for people and can change or be translated.
    // An Id has a prefix for its menu, such as "value.copy", so the ids of two menus never meet.
    public static class MenuAnchor
    {
        public static readonly AttachedProperty<bool> IsAnchorProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("IsAnchor", typeof(MenuAnchor));

        public static void SetIsAnchor(Control element, bool value) => element.SetValue(IsAnchorProperty, value);
        public static bool GetIsAnchor(Control element) => element.GetValue(IsAnchorProperty);

        // The Id of a row. The window remembers it and the next menu looks for the row with the same Id.
        public static readonly AttachedProperty<string?> IdProperty =
            AvaloniaProperty.RegisterAttached<Control, string?>("Id", typeof(MenuAnchor));

        public static void SetId(Control row, string? value) => row.SetValue(IdProperty, value);
        public static string? GetId(Control row) => row.GetValue(IdProperty);

        // The Id of the row that was used last in a menu of this window. It is null before the first use.
        public static readonly AttachedProperty<string?> LastUsedProperty =
            AvaloniaProperty.RegisterAttached<Control, string?>("LastUsed", typeof(MenuAnchor));

        public static void SetLastUsed(Control window, string? value) => window.SetValue(LastUsedProperty, value);
        public static string? GetLastUsed(Control window) => window.GetValue(LastUsedProperty);

        static MenuAnchor()
        {
            // A new anchor moves the menu, so the frame around the row arranges again.
            IsAnchorProperty.Changed.AddClassHandler<Control>((row, _) => row.FindAncestorOfType<PopupFrame>()?.InvalidateArrange());
        }
    }
}
