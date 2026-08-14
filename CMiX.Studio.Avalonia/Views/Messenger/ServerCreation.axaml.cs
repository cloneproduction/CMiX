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

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            // TODO Avalonia: ServerCreationWindow is ported in the window phase. Show it here
            // through HanumanInstitute.MvvmDialogs, passing the ViewModel property as the
            // dialog view model, matching the WPF DialogService.Show<ServerCreationWindow> call.
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
