// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace CMiX.Studio.ValueConverters
{
    public class ByteToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            byte b = (byte)value;

            return b.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var st = (string)value;

            if(!string.IsNullOrEmpty(st))
            {
                int b = System.Convert.ToInt32(value);
                if(b < 0)
                    return System.Convert.ToByte(0);
                else if(b > 255)
                    return System.Convert.ToByte(255);
                else
                    return System.Convert.ToByte(value);
            }
            else
                return System.Convert.ToByte(0);
        }
    }
}