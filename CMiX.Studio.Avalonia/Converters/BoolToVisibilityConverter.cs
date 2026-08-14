// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CMiX.Studio.Avalonia.Converters
{
    // Avalonia has no Visibility type; the converter returns a bool for IsVisible bindings.
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool b)
                return false;

            bool invert = parameter is string s &&
                          s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            return invert ? !b : b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool isVisible)
                return false;

            bool invert = parameter is string s &&
                          s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            return invert ? !isVisible : isVisible;
        }
    }
}
