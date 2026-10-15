// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Globalization;
using Avalonia.Media;

namespace CMiX.UI.Controls.Parts
{
    // The text of a number for Copy and Paste. The text of a copy does not depend on the culture.
    internal static class ValueText
    {
        public static string Format(double value) => value.ToString("R", CultureInfo.InvariantCulture);

        // The color as #AARRGGBB, made from the bytes. Color.ToString() gives the name of a known color, such as White.
        public static string Format(Color color) => $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

        // The invariant format first, then the format of the user.
        public static bool TryParse(string? text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var style = NumberStyles.Float;
            return (double.TryParse(text, style, CultureInfo.InvariantCulture, out value)
                    || double.TryParse(text, style, CultureInfo.CurrentCulture, out value))
                   && double.IsFinite(value);
        }
    }
}
