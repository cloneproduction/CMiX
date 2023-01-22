// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.Converters
{
    public class ColorToGreenAsDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorIn = (Color)value;
            return ((double)colorIn.G) / 255d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}