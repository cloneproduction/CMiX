// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.ValueConverters
{
    public class AltBackgroundConverter : IValueConverter
    {
        bool isAlternate;
        SolidColorBrush even = new SolidColorBrush(Color.FromArgb(255, 45, 45, 45));
        SolidColorBrush odd = new SolidColorBrush(Color.FromArgb(255, 40, 40, 40));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            isAlternate = !isAlternate;
            return isAlternate ? even : odd;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
