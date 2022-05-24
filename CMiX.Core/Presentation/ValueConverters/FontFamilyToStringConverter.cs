// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Core.Presentation.ValueConverters
{
    public class FontFamilyToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string fontFamily)
                return new FontFamily(fontFamily);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontFamily fontFamily)
                return fontFamily.ToString();

            return null;
        }
    }
}
