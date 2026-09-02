// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Studio.Avalonia.Converters
{
    // Read-only only for a Set-kind output (its value replaces the field entirely, so the typed
    // number means nothing) - unbound or bound to a Modulate-kind output both stay editable. Kind
    // now lives per-output (not flat on the modulator, see ModulatorOutput), so this needs both
    // values[0]: the BoundModulator (or null when unbound) and values[1]: the bound output's own
    // name, to find the specific ModulatorOutput entry whose Kind actually applies. Shared by every
    // bindable field (ModulatableValue, ModulatableIntegerValue, ...) instead of each hardcoding its
    // own rule.
    public sealed class BoundModulatorToIsReadOnlyConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2)
                return AvaloniaProperty.UnsetValue;

            if (values[0] is not IModulator modulator || values[1] is not string outputName)
                return false;

            var output = modulator.Outputs.FirstOrDefault(o => o.Name == outputName);
            return output is { Kind: ModulatorKind.Set };
        }
    }
}
