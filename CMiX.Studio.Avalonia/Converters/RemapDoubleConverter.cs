// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using CMiX.Studio.Avalonia.Mathematics;

namespace CMiX.Studio.Avalonia.Converters
{
    public class RemapDoubleConverter : MarkupExtension, IValueConverter
    {
        public double FromMin { get; set; }
        public double FromMax { get; set; }
        public double ToMin { get; set; }
        public double ToMax { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double d)
                return AvaloniaProperty.UnsetValue;

            return MathUtils.Map(d, FromMin, FromMax, ToMin, ToMax);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double d)
                return AvaloniaProperty.UnsetValue;

            return MathUtils.Map(d, ToMin, ToMax, FromMin, FromMax);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
