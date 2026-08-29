// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Converters
{
    // values[0]: an IModulator (or null when unbound) - not a bool. Reading IsHovered here rather
    // than as an extra ".IsHovered" hop in the XAML binding path means an unbound channel's null
    // BoundModulator is this converter's own final input, not an intermediate null Avalonia logs
    // as a binding error. values[1]/values[2]: brushes to use when hovered/not, each already
    // resolved by a plain {StaticResource} binding before this runs. Picking between two real,
    // already-resolved brushes avoids relying on a style default ever showing through again once
    // a binding is attached to the target property - it doesn't, see ModulatableVectorXYZ.axaml's
    // comment for why.
    public sealed class BoolToAccentBrushConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 3)
                return AvaloniaProperty.UnsetValue;

            return values[0] is IModulator { IsHovered: true } ? values[1] : values[2];
        }
    }
}
