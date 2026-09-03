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
