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

        // The WPF view showed the window through MvvmDialogs, which resolved the owner
        // from its registered view whose DataContext matched. The Avalonia service
        // instead matches the owner view model against open window DataContexts, which
        // fails for the local ServerManager context and requires INotifyPropertyChanged
        // on the main view model, so the window is shown directly with the same owned
        // nonmodal behavior.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new ServerCreationWindow { DataContext = ViewModel };
            if (TopLevel.GetTopLevel(this) is Window owner)
                window.Show(owner);
            else
                window.Show();
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
