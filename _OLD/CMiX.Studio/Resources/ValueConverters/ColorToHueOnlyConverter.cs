// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using ColorMine.ColorSpaces;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class ColorToHueOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hue = (double)value;

            if (!double.IsNaN(hue))
            {
                var hsv = new Hsv() { H = 0.0, S = 1.0, V = 1.0 };
                hsv.H = hue;
                var rgb = hsv.To<Rgb>();
                return Color.FromRgb((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
            }
            else return Colors.Red;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
