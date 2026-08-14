// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace CMiX.Studio.Avalonia.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double b = (double)value;
            return String.Format("{0:0.000}", b);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            double resultDouble;

            if (double.TryParse(strValue, out resultDouble))
                return resultDouble;
            else
                return AvaloniaProperty.UnsetValue;
        }
    }
}
