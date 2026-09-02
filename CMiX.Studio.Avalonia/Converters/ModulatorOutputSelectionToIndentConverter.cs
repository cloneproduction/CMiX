// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Converters
{
    // A single-output modulator's one row keeps zero extra margin, pixel-identical to every row
    // before ModulatorOutputSelection existed. A multi-output modulator's rows sit under their own
    // label row and are indented to read as that label's children.
    public sealed class ModulatorOutputSelectionToIndentConverter : IValueConverter
    {
        private static readonly Thickness Indented = new(16, 0, 0, 0);
        private static readonly Thickness Flush = new(0);

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not ModulatorOutputSelection selection)
                return Flush;

            return (selection.Modulator?.OutputNames?.Count ?? 0) > 1 ? Indented : Flush;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
