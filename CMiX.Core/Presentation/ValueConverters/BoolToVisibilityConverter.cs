// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CMiX.Core.Presentation.ValueConverters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return Visibility.Collapsed;

            bool objValue = (bool)value;

            if (objValue)
            {
                return Visibility.Visible;
            }
            else
            {
                if (parameter is Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                    return Visibility.Hidden;
            }
            //return Visibility.Collapsed;


        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
