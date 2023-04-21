// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace CMiX.Studio.Converters
{
    /// <summary>
    ///  accepts an object and returns its Type (as a System.Type):
    /// </summary>
    public class DataTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
          CultureInfo culture)
        {
            return value?.GetType() ?? Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
