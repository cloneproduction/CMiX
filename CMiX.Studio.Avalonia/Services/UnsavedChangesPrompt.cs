// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CMiX.Studio.Avalonia.Views.Dialogs;

namespace CMiX.Studio.Avalonia.Services
{
    // Shows the dialog over the main window.
    public sealed class UnsavedChangesPrompt : IUnsavedChangesPrompt
    {
        public async Task<UnsavedChangesChoice> AskAsync()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime { MainWindow: { } owner })
                return UnsavedChangesChoice.Cancel;

            return await new UnsavedChangesDialog().ShowDialog<UnsavedChangesChoice>(owner);
        }
    }
}
