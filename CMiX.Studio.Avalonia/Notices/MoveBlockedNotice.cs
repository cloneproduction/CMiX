// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CMiX.Studio.Avalonia.Views.Dialogs;

namespace CMiX.Studio.Avalonia.Notices
{
    // Shows the message in a dialog over the main window. OK, Enter or Escape closes it.
    public sealed class MoveBlockedNotice : IMoveBlockedNotice
    {
        public async Task ShowAsync(string message)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime { MainWindow: { } owner })
                return;

            await DialogMessage.ShowAsync(owner, Options(message));
        }

        public static MessageDialogOptions Options(string message) => MessageDialogOptions.Notice(message, "Warning");
    }
}
