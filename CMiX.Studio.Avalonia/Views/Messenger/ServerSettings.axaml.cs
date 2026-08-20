// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ServerSettings : UserControl
    {
        public ServerSettings()
        {
            InitializeComponent();
        }

        // Mirrors the WPF view, which showed the window through MvvmDialogs. The owner is the
        // DataContext of the containing window so the created window is parented to it.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as INotifyPropertyChanged;
            var owner = (TopLevel.GetTopLevel(this) as Window)?.DataContext as INotifyPropertyChanged
                ?? viewModel;
            App.DialogService?.Show(owner, viewModel);
        }
    }
}
