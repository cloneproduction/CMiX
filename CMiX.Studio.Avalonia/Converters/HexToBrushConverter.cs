// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Converters
{
    // Converts the hex color strings of the Core color values into brushes.
    // WPF converted string to Brush implicitly through its type converter.
    public class HexToBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string hex)
            {
                try
                {
                    return new SolidColorBrush(Color.Parse(hex));
                }
                catch
                {
                    return BindingOperations.DoNothing;
                }
            }
            return BindingOperations.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ISolidColorBrush brush)
                return brush.Color.ToString();
            return BindingOperations.DoNothing;
        }
    }
}
