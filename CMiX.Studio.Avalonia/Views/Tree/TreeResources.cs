// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    internal static class TreeResources
    {
        public static T Read<T>(Control control, string key, T fallback) =>
            control.TryFindResource(key, out var value) && value is T typed ? typed : fallback;
    }
}
