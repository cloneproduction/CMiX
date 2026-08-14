// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.Services;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ServerCreation : UserControl
    {
        public ServerCreation()
        {
            InitializeComponent();
            // The WPF view also created its own DialogService instance.
            DialogService = new DialogService(new DialogManager(viewLocator: new DialogViewLocator()));
        }

        private IDialogService DialogService { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogService.Show((INotifyPropertyChanged)DataContext, ViewModel);
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
