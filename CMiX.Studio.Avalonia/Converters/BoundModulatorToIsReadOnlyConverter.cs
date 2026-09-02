// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Studio.Avalonia.Converters
{
    // Read-only only for a Set-kind modulator (its output replaces the field entirely, so the typed
    // number means nothing) - unbound or bound to a Modulate-kind modulator (its output blends
    // around whatever the field already has) both stay editable. Shared by every bindable field
    // (ModulatableValue, ModulatableIntegerValue, ...) instead of each hardcoding its own rule.
    public sealed class BoundModulatorToIsReadOnlyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is IModulator { Kind: ModulatorKind.Set };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
