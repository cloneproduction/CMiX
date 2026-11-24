// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CMiX.Studio.Avalonia.Views.Dialogs;

namespace CMiX.Studio.Avalonia.Notices
{
    // Shows the message in a dialog over the main window.
    public sealed class MessageNotice : IMessageNotice
    {
        public async Task ShowErrorAsync(string text, string title)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime { MainWindow: { } owner })
                return;

            await DialogMessage.ShowAsync(owner, Options(text, title));
        }

        public static MessageDialogOptions Options(string text, string title) => MessageDialogOptions.Notice(text, "Error", title);
    }
}
