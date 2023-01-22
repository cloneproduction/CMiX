// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using ColorMine.ColorSpaces;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.Converters
{
    public class HueToColorConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var rgb = new Hsv() { H = (double)value, S = 1.0, V = 1.0 }.To<Rgb>();
            return new Color() {R = (byte)rgb.R, G = (byte)rgb.G, B = (byte)rgb.B, ScA=1.0f };
        }
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}