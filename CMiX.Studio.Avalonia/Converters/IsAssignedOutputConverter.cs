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
    // Picks the row-highlight brush for the assign popup's row - AccentInactiveBrush (the same key
    // ListBoxItemStyle uses for a selected-but-not-hovered item) when the row's own
    // ModulatorOutputSelection is the field's current binding, otherwise AvaloniaProperty.UnsetValue
    // so the wrapping Border's Background stays at its own default (unset/no paint) rather than
    // needing a literal "Transparent" brush threaded through - a plain string source would need
    // Avalonia's own string-to-brush coercion to apply on the way back out of this converter, which
    // isn't worth the risk given a wrong assumption about Avalonia binding mechanics already broke
    // this once this session (Button.Classes rejecting a MultiBinding at runtime despite compiling
    // fine). Drives a wrapping Border's Background rather than Button.Classes for the same reason -
    // Classes is a collection (AvaloniaList<string>), so a MultiBinding assigned to it via
    // property-element syntax gets added as a list item instead of bound. Only the exact matching
    // row is marked - the group-label row above it (for a multi-output modulator) is left alone on
    // purpose. values[0]: the row's own ModulatorOutputSelection. values[1]/[2]: the field's
    // BoundModulator/BoundOutputName, reached via ElementName back to the button since a popup row's
    // own DataContext is the row item, not the field - see ModulatorAssignButton.axaml. values[3]:
    // the assigned-state brush, already resolved by a plain {StaticResource} binding.
    public sealed class IsAssignedOutputConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 4)
                return AvaloniaProperty.UnsetValue;

            if (values[0] is not ModulatorOutputSelection selection)
                return AvaloniaProperty.UnsetValue;

            bool isAssigned = Equals(selection.Modulator, values[1]) && selection.Output?.Name == (values[2] as string);
            return isAssigned ? values[3] : AvaloniaProperty.UnsetValue;
        }
    }
}
