// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Settings
{
    // The look of the Studio, in the same file: { "Interface": { "ShowKeyHints": true } }. InterfaceOptions reads and writes it.
    public class InterfaceSettings
    {
        // The hint strip of the keys under the tree (KeyHintStrip).
        public bool ShowKeyHints { get; set; } = true;
    }
}
