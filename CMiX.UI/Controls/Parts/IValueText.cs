// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.UI.Controls.Parts
{
    // A value editor that gives its value as text and takes a value from text.
    // ValueMenu uses it for the Copy and Paste rows.
    internal interface IValueText
    {
        // True when the editor has a value that can be text, so that Copy has something to copy.
        bool CanCopyValueText();

        // The value as text. The format does not depend on the culture.
        string CopyValueText();

        // True when the editor can take the text now. It is false for text that is not a value of this editor.
        bool CanPasteValueText(string? text);

        // Sets the value from the text. It does nothing when CanPasteValueText is false.
        void PasteValueText(string? text);
    }
}
