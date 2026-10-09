// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.ComponentModel;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace CMiX.Studio.Avalonia.Services
{
    // Falls back to the main window when the owner view model isn't a window's own
    // DataContext, instead of throwing.
    public class MainWindowDialogManager : DialogManager
    {
        public MainWindowDialogManager(IViewLocator viewLocator, IDialogFactory dialogFactory)
            : base(viewLocator: viewLocator, dialogFactory: dialogFactory)
        {
        }

        public override IView? FindViewByViewModel(INotifyPropertyChanged viewModel) =>
            base.FindViewByViewModel(viewModel) ?? GetMainWindow();
    }
}
