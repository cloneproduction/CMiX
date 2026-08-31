// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Studio.Avalonia.Converters
{
    // Maps a modulator instance (or null) to an AppIcon IconKey. This is deliberately the only
    // place that knows "a BeatModifier looks like this" - CMiX.Core stays unaware of icons, since
    // it's shared with the VL/vvvv engine side. Add one case per new modulator kind here, nothing
    // else needs to change when one is added.
    public sealed class ModulatorTypeToIconConverter : IValueConverter
    {
        public static ModulatorTypeToIconConverter Instance { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
        {
            BeatModifier => "Beat",
            _ => "Unlinked"
        };

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
