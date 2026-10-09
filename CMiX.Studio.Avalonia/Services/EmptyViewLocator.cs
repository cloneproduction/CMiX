// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using HanumanInstitute.MvvmDialogs.Avalonia;

namespace CMiX.Studio.Avalonia.Services
{
    // StrongViewLocator is abstract, so MainWindowDialogManager needs a concrete instance even
    // though nothing currently needs a view registered - popups are Flyouts declared directly in
    // XAML instead of dialog-service windows, and the message-box/file-dialog helpers used
    // elsewhere go through DialogFactory, not this locator.
    public class EmptyViewLocator : StrongViewLocator
    {
    }
}
