// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using ColorMine.ColorSpaces;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class ColorToHSVAsDoubleConverter : IValueConverter
    {
        double Hue, Sat, Val;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorIn = (Color)value;
            var hsv = new Rgb() { R = colorIn.R, G = colorIn.G, B = colorIn.B }.To<Hsv>();

            if(hsv.V > 0.008)
            {
                Hue = hsv.H;
                Sat = hsv.S;
            }

            Val = hsv.V;

            if ((string)parameter == "Hue")
                return Hue;
            else if ((string)parameter == "Saturation")
                return Sat;
            else if ((string)parameter == "Value")
                return Val;
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
