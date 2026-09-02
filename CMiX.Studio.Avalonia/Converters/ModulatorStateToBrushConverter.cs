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
    // Drives the assign-button's dot through the 3 states that matter (unbound / bound / bound-and-
    // hovered) - replaces the old type-icon + 2-state BoolToAccentBrushConverter, which collapsed
    // "bound, not hovered" into the same grey as "unbound" since Fill was driven off a single 2-way
    // choice. values[0]: BoundModulator itself (or null when unbound) - the bound/unbound check.
    // values[1]: BoundModulator.IsHovered mirrored as its own binding purely so this MultiBinding
    // re-evaluates when it flips - a "BoundModulator" path binding alone only reacts to the field's
    // own BoundModulator reference changing, never to a nested property of the instance it already
    // points at. values[2]/[3]/[4]: the unbound/bound/hovered brushes, each already resolved by a
    // plain {StaticResource} binding before this runs - picking between real, already-resolved
    // brushes avoids relying on a style default ever showing through again once a binding is
    // attached to the target property (see ModulatableVectorXYZ.axaml's comment for why it doesn't).
    public sealed class ModulatorStateToBrushConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 5)
                return AvaloniaProperty.UnsetValue;

            if (values[0] is not IModulator modulator)
                return values[2];

            return modulator.IsHovered ? values[4] : values[3];
        }
    }
}
