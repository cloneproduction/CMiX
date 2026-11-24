// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.Settings
{
    // The model of the shortcut file. A value is a gesture string, for example "Ctrl+Shift+Z". An empty string means no gesture.
    public class ShortcutFile
    {
        public Dictionary<string, string> Shortcuts { get; set; } = new();
    }
}
