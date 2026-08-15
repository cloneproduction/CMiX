// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ServerCreation : UserControl
    {
        public ServerCreation()
        {
            InitializeComponent();
        }

        // Mirrors the WPF view, which showed the window through MvvmDialogs. The
        // owner is the DataContext of the containing window so the created window is
        // parented to it. Passing the local ServerManager instead would let a second
        // open ServerCreationWindow match the first by reference and parent to it.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var owner = (TopLevel.GetTopLevel(this) as Window)?.DataContext as INotifyPropertyChanged
                ?? ViewModel;
            App.DialogService?.Show(owner, ViewModel);
        }

        public static readonly StyledProperty<INotifyPropertyChanged> ViewModelProperty =
            AvaloniaProperty.Register<ServerCreation, INotifyPropertyChanged>(nameof(ViewModel));
        public INotifyPropertyChanged ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}
