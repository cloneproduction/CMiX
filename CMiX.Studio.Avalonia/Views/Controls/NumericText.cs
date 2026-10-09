// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Formats and parses numeric text so the current culture decimal separator
    // is kept for display, but no thousands group separator is ever emitted,
    // and the text can always be parsed back regardless of the active culture.
    internal static class NumericText
    {
        public static string Format(double value)
        {
            return value.ToString("0.000", CultureInfo.CurrentCulture);
        }

        public static string Format(int value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }

        public static bool TryParse(string text, out double value)
        {
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
                return true;

            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}
