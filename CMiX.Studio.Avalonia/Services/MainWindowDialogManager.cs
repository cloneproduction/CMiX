// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.ComponentModel;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace CMiX.Studio.Avalonia.Services
{
    // The base owner lookup matches the owner view model by reference against the
    // DataContext of every open window and throws when nothing matches. Nested view
    // models such as ServerManager are never a window DataContext, so this falls
    // back to the main window and every dialog stays parented instead of crashing
    // with Cannot find View for viewModel.
    public class MainWindowDialogManager : DialogManager
    {
        public MainWindowDialogManager(IViewLocator viewLocator)
            : base(viewLocator: viewLocator)
        {
        }

        public override IView? FindViewByViewModel(INotifyPropertyChanged viewModel) =>
            base.FindViewByViewModel(viewModel) ?? GetMainWindow();
    }
}
