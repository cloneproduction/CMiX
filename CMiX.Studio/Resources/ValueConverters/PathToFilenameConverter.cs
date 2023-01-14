// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace CMiX.Studio.ValueConverters
{
    public class PathToFilenameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = null;
            if (value != null)
            {
                //if (string.IsNullOrWhiteSpace(path) == false)
                result = Path.GetFileNameWithoutExtension(value.ToString());
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
