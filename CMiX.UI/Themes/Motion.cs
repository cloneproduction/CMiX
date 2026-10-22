// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Animation.Easings;

namespace CMiX.UI.Themes
{
    // The time and the easing of the animations of the library, so that every animation moves the same way.
    public static class Motion
    {
        public static readonly TimeSpan Duration = TimeSpan.FromMilliseconds(100);

        public static Easing Easing => new CubicEaseOut();
    }
}
