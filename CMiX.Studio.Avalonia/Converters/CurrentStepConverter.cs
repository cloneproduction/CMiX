// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Converters
{
    public class CurrentStepConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            var activeBrush = parameter as IBrush ?? Brushes.OrangeRed;

            if (values.Count > 1 && values[0] is int itemIndex && values[1] is int currentStep)
                return itemIndex == currentStep ? activeBrush : Brushes.Transparent;
            return Brushes.Transparent;
        }
    }
}
