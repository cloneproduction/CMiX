// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CMiX.Core.Prefabs;

namespace CMiX.Studio.Avalonia.Converters
{
    public class TypeNameToDisplayLabelConverter : IValueConverter
    {
        public static readonly TypeNameToDisplayLabelConverter Instance = new();

        private static readonly string[] TypeNameSuffixes = { "Modifier", "Modulator" };

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var name = (value as Type)?.Name ?? "";
            foreach (var suffix in TypeNameSuffixes)
            {
                if (name.Length > suffix.Length && name.EndsWith(suffix, StringComparison.Ordinal))
                {
                    name = name[..^suffix.Length];
                    break;
                }
            }
            return ControlFactory.StringHelper.PascalCaseToDisplay(name);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
