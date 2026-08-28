// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CMiX.Studio.Avalonia.Converters
{
    // Returns a bool for IsVisible/IsEnabled bindings. ConverterParameter sets the threshold
    // (count > threshold); defaults to 1 when omitted, matching every usage that predates the
    // parameter.
    public class CountToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var threshold = int.TryParse(parameter?.ToString(), out var t) ? t : 1;
            return value is int count && count > threshold;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
