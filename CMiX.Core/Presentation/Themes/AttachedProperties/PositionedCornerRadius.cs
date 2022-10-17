// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using CMiX.Core.Presentation.Controls;

namespace CMiX.Core.Presentation.Themes.AttachedProperties
{
    public static class PositionedCornerRadius
    {
        public static readonly DependencyProperty RadiusValueProperty =
        DependencyProperty.RegisterAttached("RadiusValue", typeof(CornerRadius), typeof(PositionedCornerRadius), new FrameworkPropertyMetadata(new CornerRadius(3)));

        public static void SetRadiusValue(DependencyObject element, CornerRadius value)
        {
            element.SetValue(RadiusValueProperty, value);
        }

        public static CornerRadius GetRadiusValue(DependencyObject element)
        {
            return (CornerRadius)element.GetValue(RadiusValueProperty);
        }



        public static readonly DependencyProperty RoundedPositionProperty =
        DependencyProperty.RegisterAttached("RoundedPosition", typeof(RoundedPosition), typeof(PositionedCornerRadius), new FrameworkPropertyMetadata(RoundedPosition.All));

        public static void SetRoundedPosition(DependencyObject element, RoundedPosition value)
        {
            element.SetValue(RoundedPositionProperty, value);
        }

        public static RoundedPosition GetRoundedPosition(DependencyObject element)
        {
            return (RoundedPosition)element.GetValue(RoundedPositionProperty);
        }
    }
}
