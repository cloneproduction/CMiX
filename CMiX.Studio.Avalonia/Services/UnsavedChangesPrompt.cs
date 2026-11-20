// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CMiX.Studio.Avalonia.Views.Dialogs;

namespace CMiX.Studio.Avalonia.Services
{
    // Shows the question in a dialog over the main window. Enter is Save. Escape is Cancel.
    public sealed class UnsavedChangesPrompt : IUnsavedChangesPrompt
    {
        public const string Question = "Save change before closing ?";

        public static MessageDialogOptions Options { get; } = new(
            Question,
            "Warning",
            new[]
            {
                new MessageDialogButton("Save", MessageDialogResult.Save, IsDefault: true),
                new MessageDialogButton("Don't Save", MessageDialogResult.DontSave),
                new MessageDialogButton("Cancel", MessageDialogResult.Cancel, IsCancel: true)
            });

        public async Task<UnsavedChangesChoice> AskAsync()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime { MainWindow: { } owner })
                return UnsavedChangesChoice.Cancel;

            return ToChoice(await DialogMessage.ShowAsync(owner, Options));
        }

        public static UnsavedChangesChoice ToChoice(MessageDialogResult result) => result switch
        {
            MessageDialogResult.Save => UnsavedChangesChoice.Save,
            MessageDialogResult.DontSave => UnsavedChangesChoice.DontSave,
            _ => UnsavedChangesChoice.Cancel
        };
    }
}
