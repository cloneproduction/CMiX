// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
