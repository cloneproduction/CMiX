// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace CMiX.Studio.Converters
{
    public class TypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : Regex.Replace(Regex.Replace(value.GetType().Name, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),@"(\p{Ll})(\P{Ll})","$1 $2"); // or FullName, or whatever
    }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
