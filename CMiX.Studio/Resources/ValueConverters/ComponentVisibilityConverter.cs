// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace CMiX.Studio.ValueConverters
{
    public class ComponentVisibilityConverter : IMultiValueConverter
    {
        bool parentIsVisible;
        bool isVisible;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool)
                parentIsVisible = (bool)values[0];
                
            if (values[1] is bool)
                isVisible = (bool)values[1];
               
            if (!parentIsVisible)
                return parentIsVisible;
            else
                return isVisible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            bool visibility = (bool)value;
            object[] ret = new object[2];

            if (!parentIsVisible)
            {
                ret[0] = false;
                ret[1] = false;
            }
            else
            {
                ret[0] = visibility;
                ret[1] = visibility;
            }

            return ret;
        }
    }
}