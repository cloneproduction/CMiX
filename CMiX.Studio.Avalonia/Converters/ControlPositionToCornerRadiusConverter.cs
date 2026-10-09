// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using CMiX.Studio.Avalonia.Views.Controls;

namespace CMiX.Studio.Avalonia.Converters
{
    public class ControlPositionToCornerRadiusConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not ControlPosition position)
                return BindingOperations.DoNothing;

            var key = position switch
            {
                ControlPosition.Top => "CornerRadiusTop",
                ControlPosition.Bottom => "CornerRadiusBottom",
                ControlPosition.Left => "CornerRadiusLeft",
                ControlPosition.Right => "CornerRadiusRight",
                ControlPosition.All => "CornerRadiusAll",
                _ => "CornerRadiusNone",
            };

            return Application.Current?.TryFindResource(key, out var resource) == true ? resource : BindingOperations.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
