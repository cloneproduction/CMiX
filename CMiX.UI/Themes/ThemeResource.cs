// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;

namespace CMiX.UI.Themes
{
    // Reads a token of the theme from a control, in the theme variant of the control.
    public static class ThemeResource
    {
        public static T? Find<T>(Control control, string key) where T : class =>
            control.TryFindResource(key, control.ActualThemeVariant, out var value) && value is T found ? found : null;

        public static T Get<T>(Control control, string key, T fallback) where T : struct =>
            control.TryFindResource(key, control.ActualThemeVariant, out var value) && value is T found ? found : fallback;
    }
}
