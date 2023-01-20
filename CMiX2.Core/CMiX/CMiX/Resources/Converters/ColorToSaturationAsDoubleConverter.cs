// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using ColorMine.ColorSpaces;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.Converters
{
    public class ColorToSaturationAsDoubleConverter : IValueConverter
    {

        double Sat;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorIn = (Color)value;
            var hsv = new Rgb() { R = colorIn.R, G = colorIn.G, B = colorIn.B }.To<Hsv>();

            if (hsv.V >= 0.03)
            {
                Sat = hsv.S;
            }
            return Sat;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}