// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Core.Presentation.ValueConverters
{
    public class ColorToNoAlphaConverter : IValueConverter
    {
        SolidColorBrush outputBrush = new SolidColorBrush();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var inputBrush = (Color)value;

                outputBrush.Color = Color.FromArgb(255, inputBrush.R, inputBrush.G, inputBrush.B);

                return outputBrush;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
