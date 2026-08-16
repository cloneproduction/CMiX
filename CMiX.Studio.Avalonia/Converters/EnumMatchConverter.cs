// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CMiX.Studio.Avalonia.Converters
{
    // Replacement for the WPF DataTrigger pattern that compared an enum property against a
    // constant to swap a ContentTemplate or Visibility. Bind IsVisible/IsEnabled to the enum
    // property with ConverterParameter set to the enum value to match, e.g.
    // IsVisible="{Binding Pipeline.Value, Converter={x:Static Converters:EnumMatchConverter.Instance}, ConverterParameter={x:Static Materials:PipelineType.Metallic}}".
    public sealed class EnumMatchConverter : IValueConverter
    {
        public static EnumMatchConverter Instance { get; } = new EnumMatchConverter();

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Equals(value, parameter);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BindingOperations.DoNothing;
        }
    }
}
