// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Notices
{
    // What the user chose when asked about unsaved changes. Cancel is the first value, so a dialog
    // that is closed without an answer means Cancel.
    public enum UnsavedChangesChoice
    {
        Cancel,
        Save,
        DontSave
    }
}
