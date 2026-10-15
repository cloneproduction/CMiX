// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.AttachedProperties
{
    // The name that a menu of a control shows as its title.
    // A control with a short Caption, such as "Y" in "Position Y", sets Text to the full name.
    public static class ControlTitle
    {
        public static readonly AttachedProperty<string?> TextProperty =
            AvaloniaProperty.RegisterAttached<Control, string?>("Text", typeof(ControlTitle));

        public static void SetText(Control element, string? value) => element.SetValue(TextProperty, value);
        public static string? GetText(Control element) => element.GetValue(TextProperty);

        // The Text of the control. Without a Text, its Caption. The spaces at the ends are removed.
        public static string Of(Control control)
        {
            var text = GetText(control);
            if (string.IsNullOrWhiteSpace(text))
                text = (control as ICaptioned)?.Caption;

            return text?.Trim() ?? string.Empty;
        }
    }
}
