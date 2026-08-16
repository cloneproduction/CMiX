// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CMiX.Studio.Avalonia.Converters
{
    // Replaces WPF DataTriggers that compared a bound enum value against a fixed
    // value. Usage: Converter={x:Static Converters:EnumEqualsConverter.Instance}
    // ConverterParameter=SomeEnumMemberName
    public class EnumEqualsConverter : IValueConverter
    {
        public static readonly EnumEqualsConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || parameter is null)
                return false;

            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
