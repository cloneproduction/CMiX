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
