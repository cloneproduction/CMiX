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
    // Marks whichever row in the assign popup is the field's current binding, not just "is
    // something bound" (BoolToAccentBrush/ModulatorStateToBrush already cover that on the button
    // itself). values[0]: the row's own ModulatorOutputSelection. values[1]/[2]: the field's
    // BoundModulator/BoundOutputName, reached via ElementName back to the button since a popup row's
    // own DataContext is the row item, not the field - see ModulatorAssignButton.axaml. values[3]/[4]:
    // the default/assigned brushes, already resolved by plain {StaticResource} bindings.
    public sealed class AssignedOutputToBrushConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 5)
                return AvaloniaProperty.UnsetValue;

            if (values[0] is not ModulatorOutputSelection selection)
                return values[3];

            bool isAssigned = Equals(selection.Modulator, values[1])
                && selection.Output?.Name == (values[2] as string);

            return isAssigned ? values[4] : values[3];
        }
    }
}
