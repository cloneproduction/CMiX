// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;
using Avalonia.Interactivity;
using ServerModel = CMiX.Core.Networking.Servers.Server;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ServerSettings : UserControl
    {
        public ServerSettings()
        {
            InitializeComponent();
        }

        private ServerModel Server => (ServerModel)DataContext!;

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (await Server.ApplyAsync())
                EditConnectionButton.Flyout!.Hide();
        }

        private void CancelEditConnection_Click(object sender, RoutedEventArgs e) => EditConnectionButton.Flyout!.Hide();

        private void Push_Click(object sender, RoutedEventArgs e)
        {
            Server.PushCommand.Execute(null);
            SyncButton.Flyout!.Hide();
        }

        private void Pull_Click(object sender, RoutedEventArgs e)
        {
            Server.PullCommand.Execute(null);
            SyncButton.Flyout!.Hide();
        }

        private void CancelSync_Click(object sender, RoutedEventArgs e) => SyncButton.Flyout!.Hide();
    }
}
