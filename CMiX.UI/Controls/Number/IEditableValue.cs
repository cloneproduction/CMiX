// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.UI.Controls.Number
{
    // A value editor with an edit box. EditCycle moves the edit mode from one such editor to the next.
    public interface IEditableValue
    {
        bool IsEditing { get; set; }

        // Ends the edit mode and keeps the typed value. Nothing happens when the value is not in edit mode.
        void EndEdit();

        // False for a value that the user cannot change, for example a modulated one.
        bool CanEdit { get; }
    }
}
