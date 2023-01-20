// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace CMiX.Studio.Converters
{
    public class IntToNormalizedDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0.00;
            double range = System.Convert.ToDouble(value);
            if (value != null)
            {
                result = (range * 100);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0.00;
            double range = System.Convert.ToDouble(value);
            if (value != null)
            {
                result = (range / 100);
            }
            return result;
        }
    }
}