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
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool b)
            return Visibility.Collapsed;

        bool invert = parameter is string s &&
                      s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

        bool visible = invert ? !b : b;
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Visibility v)
            return false;

        bool invert = parameter is string s &&
                      s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

        bool isVisible = v == Visibility.Visible;
        return invert ? !isVisible : isVisible;
    }
}}
