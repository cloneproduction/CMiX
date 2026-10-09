// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Studio.Avalonia.Views.Controls;

namespace CMiX.Studio.Avalonia.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double b)
                return AvaloniaProperty.UnsetValue;

            return NumericText.Format(b);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string strValue)
                return AvaloniaProperty.UnsetValue;

            if (NumericText.TryParse(strValue, out double resultDouble))
                return resultDouble;
            else
                return AvaloniaProperty.UnsetValue;
        }
    }
}
