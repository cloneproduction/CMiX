// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Studio.Views.BaseControl;
using System.Windows;

namespace CMiX.Studio.AttachedProperties
{
    public static class PositionedControl
    {
        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.RegisterAttached("Position", typeof(ControlPosition), typeof(PositionedControl), new FrameworkPropertyMetadata(ControlPosition.Default));

        public static void SetPosition(DependencyObject element, ControlPosition value)
        {
            element.SetValue(PositionProperty, value);
        }

        public static ControlPosition GetPosition(DependencyObject element)
        {
            return (ControlPosition)element.GetValue(PositionProperty);
        }
    }
}
