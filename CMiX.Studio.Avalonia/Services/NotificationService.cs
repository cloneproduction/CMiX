// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.UI.Converters;
using CMiX.UI.Popups;

namespace CMiX.Studio.Avalonia.Services
{
    // Shows the messages in the PanelNotification of the main window. The main window attaches its host.
    // The undo key comes from the ShortcutMap when the message shows, so a changed key shows correctly.
    public sealed class NotificationService : INotificationService
    {
        private readonly ShortcutMap shortcuts;
        private PanelNotification? host;

        public NotificationService(ShortcutMap shortcuts) => this.shortcuts = shortcuts;

        public void Attach(PanelNotification host) => this.host = host;

        public void Show(string text) => host?.Show(text);

        public void ShowWithUndo(string text) =>
            host?.Show(text, shortcuts["Edit.Undo"] is { } gesture ? KeyGestureTextConverter.TextOf(gesture) : null);
    }
}
