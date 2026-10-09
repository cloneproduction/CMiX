// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Converters
{
    public class ColorToHexConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var hexCode = System.Convert.ToString(value);
            if (hexCode == null)
                return null;

            try
            {
                return Color.Parse(hexCode);
            }
            catch
            {
                return null;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Color.ToString() resolves to a known color's name (e.g. "White") whenever the value
            // matches one exactly, rather than always producing hex - System.Convert.ToString(value)
            // relied on that, so a control merely re-publishing an unchanged Color on its own first
            // bind (e.g. StandardColorPicker) silently rewrote a stored "#FFFFFFFF" to "White". Both
            // strings parse back to the same color, but as raw strings they're not equal, so the
            // rewrite looked like - and was recorded as - a real edit. Formatting from the channel
            // bytes directly keeps the round trip byte-for-byte stable.
            if (value is Color color)
                return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

            return System.Convert.ToString(value);
        }
    }
}
