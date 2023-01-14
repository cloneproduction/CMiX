// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class ColorToColorComponentConverter : IValueConverter
    {
        float Red, Green, Blue, Alpha;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double componentout = 0.0;

            Color colorIn = (Color)value;
            Red = colorIn.ScR;
            Green = colorIn.ScG;
            Blue = colorIn.ScB;
            Alpha = colorIn.ScA;

            string param = (string)parameter;

            if (param == "Red")
                componentout = (double)Red;

            if (param == "Green")
                componentout = (double)Green;

            if (param == "Blue")
                componentout = (double)Blue;

            if (param == "Alpha")
                componentout = (double)Alpha;

            return componentout;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double newcomponent = (double)value;
            string param = (string)parameter;
            Color colorOut = new Color();

            if (param == "Red")
                colorOut = Color.FromScRgb(Alpha, (float)newcomponent, Green, Blue);

            if (param == "Green")
                colorOut = Color.FromScRgb(Alpha, Red, (float)newcomponent, Blue);

            if (param == "Blue")
                colorOut = Color.FromScRgb(Alpha, Red, Green, (float)newcomponent);

            if (param == "Alpha")
                colorOut = Color.FromScRgb((float)newcomponent, Red, Green, Blue);

            return colorOut;
        }
    }
}