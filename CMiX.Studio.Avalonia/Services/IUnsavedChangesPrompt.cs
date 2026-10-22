// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;

namespace CMiX.Studio.Avalonia.Services
{
    // What the user chose when asked about unsaved changes. Cancel is the first value, so a dialog
    // that is closed without an answer means Cancel.
    public enum UnsavedChangesChoice
    {
        Cancel,
        Save,
        DontSave
    }

    // Asks the user what to do with the unsaved changes of the project before the window closes.
    public interface IUnsavedChangesPrompt
    {
        Task<UnsavedChangesChoice> AskAsync();
    }
}
