// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Converters
{
    // "<Modulator> <Output>", e.g. "Beat Value" / "Tracking Count" - always both, no special-casing
    // for a modulator with only one output, so every row in the popup is built the same way. Every
    // row is a plain clickable Button now (see ModulatorAssignButton.axaml) - no separate,
    // non-clickable group-label row above them, which used to carry the modulator's own name alone.
    public sealed class ModulatorOutputSelectionToLabelConverter : IValueConverter
    {
        private const string ModulatorSuffix = " Modulator";

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not ModulatorOutputSelection selection)
                return null;

            var modulatorName = selection.Modulator?.PrefabService?.Name?.Value;
            if (modulatorName != null && modulatorName.EndsWith(ModulatorSuffix, StringComparison.Ordinal))
                modulatorName = modulatorName[..^ModulatorSuffix.Length];

            return $"{modulatorName} {selection.Output?.Name}";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
