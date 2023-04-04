// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace CMiX.Studio.Converters
{
    public class ColorDrawingToMediaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is System.Drawing.Color color)
                return MColor.FromArgb(color.A, color.R, color.G, color.B);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is System.Windows.Media.Color color)
                return DColor.FromArgb(color.A, color.R, color.G, color.B);

            return null;
        }
    }
}
