// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Studio.Avalonia.Views.Controls;

namespace CMiX.Studio.Avalonia.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double b)
                return AvaloniaProperty.UnsetValue;

            return NumericText.Format(b);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string strValue)
                return AvaloniaProperty.UnsetValue;

            if (NumericText.TryParse(strValue, out double resultDouble))
                return resultDouble;
            else
                return AvaloniaProperty.UnsetValue;
        }
    }
}
