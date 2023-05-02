// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mathematics;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class ColorToDoubleConverter : IValueConverter
    {
        double hue, sat, val;
        double output = 0.0;
        Color colorOut = new Color();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color colorIn = (Color)value;
            System.Drawing.Color col = System.Drawing.Color.FromArgb(255, colorIn.R, colorIn.G, colorIn.B);
            ColorExtensions.ColorToHSV(colorIn, out hue, out sat, out val);

            if ((string)parameter == "Hue")
                output = col.GetHue(); ;
            if ((string)parameter == "Sat")
                output = col.GetSaturation();
            if ((string)parameter == "Val")
                output = col.GetBrightness();

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double newvalue = (double)value;

            if ((string)parameter == "Hue")
                colorOut = ColorExtensions.ColorFromHSV(newvalue, sat, val);
            if ((string)parameter == "Sat")
                colorOut = ColorExtensions.ColorFromHSV(hue, newvalue, val);
            if ((string)parameter == "Val")
                colorOut = ColorExtensions.ColorFromHSV(hue, sat, newvalue);

            return colorOut;
        }
    }
}