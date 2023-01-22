// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mathematics;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.Converters
{
    public class DoubleToColorSatConverter : IValueConverter
    {
        double hue, sat, val;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("DoubleToColorSatConverter");
            Color colorIn = (Color)value;
            ColorExtensions.ColorToHSV(colorIn, out hue, out sat, out val);
            return 1 - sat;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double newsat = (double)value;
            return ColorExtensions.ColorFromHSV(hue, newsat, val); ;
        }
    }
}