// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class ColorToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hexCode = System.Convert.ToString(value);
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hexCode);
                return color;
            }
            catch
            {
                return null;
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hexCode = System.Convert.ToString(value);
            try
            {
                return hexCode;
            }
            catch
            {
                return null;
            }
        }
    }
}