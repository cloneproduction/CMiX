// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CMiX.Studio.ValueConverters
{
    public class AlwaysVisibleConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}