// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using ColorMine.ColorSpaces;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class ColorToHueValueOnlyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var rgb = new Rgb();
            if (values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
            {
                if (!double.IsNaN((double)values[0]) && !double.IsNaN((double)values[1]))
                {

                    rgb = new Hsv() { H = (double)values[0], S = 1.0, V = (double)values[1] }.To<Rgb>();
                }
            }

            return Color.FromRgb((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}