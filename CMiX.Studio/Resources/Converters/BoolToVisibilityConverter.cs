// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CMiX.Studio.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool b)
                return FalseValue;

            bool invert = parameter is string s &&
                          s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            bool result = invert ? !b : b;
            return result ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility v)
                return false;

            bool invert = parameter is string s &&
                          s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            bool isVisible = v == TrueValue;
            return invert ? !isVisible : isVisible;
        }
    }
}
