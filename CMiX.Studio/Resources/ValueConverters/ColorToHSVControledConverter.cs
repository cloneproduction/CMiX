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
    public class ColorToHSVControledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)ColorConverter.ConvertFromString((string)parameter);
            var hsv = new Rgb() { R = color.R, G = color.G, B = color.B }.To<Hsv>();

            if (values[0] != DependencyProperty.UnsetValue)
                hsv.S = (double)values[0];
            if(values[1] != DependencyProperty.UnsetValue)
                hsv.V = (double)values[1];

            var rgb = hsv.To<Rgb>();

            return Color.FromRgb((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}