// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace CMiX.Studio.Converters
{
    public class IntToStringWithMaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = null;
            if (!double.IsNaN((double)value))
            {
                var val = (double)value;
                var integer = System.Convert.ToInt32(val);
                result = integer.ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var st = (string)value;
            var max = int.Parse((string)parameter, System.Globalization.CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(st))
            {
                int b = System.Convert.ToInt32(value);
                if (b < 0)
                    b = 0;
                else if (b > max)
                    b = max;
                return b;
            }
            else
                return 0;
        }
    }
}