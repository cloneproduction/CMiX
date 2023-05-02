// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mathematics;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class ColorHueToDoubleConverter : IValueConverter
    {
        double hue, sat, val;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color colorIn = (Color)value;
            ColorExtensions.ColorToHSV(colorIn, out hue, out sat, out val);
            return hue / 360;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double newhue = (double)value;
            Color colorOut = ColorExtensions.ColorFromHSV(newhue * 360, sat, val);
            return colorOut;
        }
    }
}