// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Animation.Easings;

namespace CMiX.UI.Themes
{
    // The time and the easing of the animations of the library, so that every animation moves the same way.
    public static class Motion
    {
        // Every expander opens and closes with this time and this easing: the panel and its chevron both use them.
        public static readonly TimeSpan Duration = TimeSpan.FromMilliseconds(160);

        // A fade of a mark, such as the dimming of a row during a drag. It is quick and soft.
        public static readonly TimeSpan FadeDuration = TimeSpan.FromMilliseconds(120);

        // The time that a notification (see PanelNotification) shows before it hides itself.
        public static readonly TimeSpan NotificationDuration = TimeSpan.FromSeconds(3);

        public static Easing Easing => new CubicEaseOut();
    }
}
