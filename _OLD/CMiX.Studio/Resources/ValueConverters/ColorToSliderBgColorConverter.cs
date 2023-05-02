// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class ColorToSliderBgColorConverter : IValueConverter
    {
        float Red, Green, Blue, Alpha;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color colorOut = new Color();
            Color colorIn = (Color)value;

            Red = colorIn.ScR;
            Green = colorIn.ScG;
            Blue = colorIn.ScB;
            Alpha = colorIn.ScA;

            string param = (string)parameter;

            if (param == "RedRight")
                colorOut = Color.FromScRgb(Alpha, 1.0f, Green, Blue) ;

            if (param == "RedLeft")
                colorOut = Color.FromScRgb(Alpha, 0.0f, Green, Blue);

            if (param == "GreenRight")
                colorOut = Color.FromScRgb(Alpha, Red, 1.0f, Blue);

            if (param == "GreenLeft")
                colorOut = Color.FromScRgb(Alpha, Red, 0.0f, Blue);

            if (param == "BlueRight")
                colorOut = Color.FromScRgb(Alpha, Red, Green, 1.0f);

            if (param == "BlueLeft")
                colorOut = Color.FromScRgb(Alpha, Red, Green, 0.0f);

            if (param == "Alpha")
                colorOut = Color.FromScRgb(1.0f, Red, Green, Blue);

            return colorOut;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}