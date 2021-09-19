// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Input;

namespace CMiX.Core.Presentation.Views
{
    public partial class MessengerSettingsWindow : Window
    {
        public MessengerSettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = Mouse.GetPosition(null).Y;
            this.Left = Mouse.GetPosition(null).X;
        }
    }
}
