// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.AttachedProperties
{
    public static class ExpanderIndentation
    {
        public static readonly AttachedProperty<int> IndentationProperty =
            AvaloniaProperty.RegisterAttached<Control, int>(
                "Indentation",
                typeof(ExpanderIndentation));

        public static int GetIndentation(Control obj) => obj.GetValue(IndentationProperty);
        public static void SetIndentation(Control obj, int value) => obj.SetValue(IndentationProperty, value);

        static ExpanderIndentation()
        {
            IndentationProperty.Changed.AddClassHandler<Control>(OnIndentationChanged);
        }

        private static void OnIndentationChanged(Control element, AvaloniaPropertyChangedEventArgs e)
        {
            element.Margin = new Thickness(e.GetNewValue<int>(), 0, 0, 0);
        }
    }
}
