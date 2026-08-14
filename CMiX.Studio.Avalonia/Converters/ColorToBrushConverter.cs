// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        SolidColorBrush _red = new SolidColorBrush(),
                        _green = new SolidColorBrush(),
                        _blue = new SolidColorBrush(),
                        _alpha = new SolidColorBrush(),
                        _all = new SolidColorBrush();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return BindingOperations.DoNothing;

            var color = (Color)value;
            switch ((string)parameter)
            {
                case "r":
                    _red.Color = Color.FromRgb(color.R, 0, 0);
                    return _red;
                case "g":
                    _green.Color = Color.FromRgb(0, color.G, 0);
                    return _green;
                case "b":
                    _blue.Color = Color.FromRgb(0, 0, color.B);
                    return _blue;
                case "a":
                    _alpha.Color = Color.FromArgb(color.A, 128, 128, 128);
                    return _alpha;
                case "all":
                    return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            }
            return BindingOperations.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
