// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using CMiX.Studio.Avalonia.Views.Controls;

namespace CMiX.Studio.Avalonia.AttachedProperties
{
    public static class PositionedControl
    {
        public static readonly AttachedProperty<ControlPosition> PositionProperty =
            AvaloniaProperty.RegisterAttached<Control, ControlPosition>(
                "Position",
                typeof(PositionedControl),
                ControlPosition.All);

        public static void SetPosition(Control element, ControlPosition value) => element.SetValue(PositionProperty, value);
        public static ControlPosition GetPosition(Control element) => element.GetValue(PositionProperty);

        static PositionedControl()
        {
            PositionProperty.Changed.AddClassHandler<Control>(OnPositionChanged);
        }

        private static void OnPositionChanged(Control element, AvaloniaPropertyChangedEventArgs e)
        {
            var position = e.GetNewValue<ControlPosition>();
            var pseudoClasses = (IPseudoClasses)element.Classes;
            pseudoClasses.Set(":position-top", position == ControlPosition.Top);
            pseudoClasses.Set(":position-middle", position == ControlPosition.Middle);
            pseudoClasses.Set(":position-bottom", position == ControlPosition.Bottom);
            pseudoClasses.Set(":position-left", position == ControlPosition.Left);
            pseudoClasses.Set(":position-right", position == ControlPosition.Right);
            pseudoClasses.Set(":position-all", position == ControlPosition.All);
            pseudoClasses.Set(":position-none", position == ControlPosition.None);
        }
    }
}
