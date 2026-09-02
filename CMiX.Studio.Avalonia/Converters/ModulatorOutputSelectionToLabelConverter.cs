// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Converters
{
    // A single-output modulator's one row still shows the modulator's own name, exactly as every
    // row did before ModulatorOutputSelection existed. A multi-output modulator's rows sit under
    // their own non-clickable label row (which already shows the modulator's name), so each of
    // those instead shows just its own output name ("X", "Y", ...).
    public sealed class ModulatorOutputSelectionToLabelConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not ModulatorOutputSelection selection)
                return null;

            return (selection.Modulator?.Outputs?.Count ?? 0) > 1
                ? selection.Output?.Name
                : selection.Modulator?.PrefabService?.Name?.Value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
