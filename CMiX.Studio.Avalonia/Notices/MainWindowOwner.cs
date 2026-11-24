// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace CMiX.Studio.Avalonia.Notices
{
    // The main window of the desktop, or null before it exists. A dialog uses it as its owner.
    public static class MainWindowOwner
    {
        public static Window? Find() =>
            Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window } ? window : null;
    }
}
