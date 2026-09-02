// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Studio.Avalonia.Converters
{
    // The lighter counterpart to AssignedOutputToBrushConverter, for a multi-output modulator's own
    // group-label row: it reads as "contains the assigned one" whenever it IS the field's
    // BoundModulator, without needing to check BoundOutputName too - the field can only ever be
    // bound to one output at a time, so "this modulator instance is BoundModulator" already implies
    // one of the rows underneath it is the exact match. values[0]: the row's own IModulator.
    // values[1]: BoundModulator, reached via ElementName back to the button (see
    // ModulatorAssignButton.axaml). values[2]/[3]: the default/assigned brushes, already resolved by
    // plain {StaticResource} bindings.
    public sealed class AssignedModulatorToBrushConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 4)
                return AvaloniaProperty.UnsetValue;

            bool isAssignedGroup = values[0] is IModulator modulator && Equals(modulator, values[1]);
            return isAssignedGroup ? values[3] : values[2];
        }
    }
}
