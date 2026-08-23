// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
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

        // The sync Flyout is only ever opened while ClientIsConnected is true (it sits behind
        // SyncButton, itself hidden otherwise) - closed here too if the Engine drops mid-decision,
        // so it can't outlive the connection it's about to act on.
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is ServerModel server)
                server.PropertyChanged += Server_PropertyChanged;
        }

        private void Server_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ServerModel.ClientIsConnected) && !Server.ClientIsConnected)
                SyncButton.Flyout?.Hide();
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            // ApplyAsync can take a few seconds (its own port-availability check has a 3s
            // timeout), so the button gives visible feedback instead of looking unresponsive.
            ConnectButton.IsEnabled = false;
            ConnectButton.Content = "Connecting...";
            try
            {
                if (await Server.ApplyAsync())
                    EditConnectionButton.Flyout!.Hide();
            }
            finally
            {
                ConnectButton.Content = "Connect";
                ConnectButton.IsEnabled = true;
            }
        }

        private void CancelEditConnection_Click(object sender, RoutedEventArgs e) => EditConnectionButton.Flyout!.Hide();

        private void EditConnectionFlyout_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                EditConnectionButton.Flyout!.Hide();
        }

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

        private void SyncFlyout_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                SyncButton.Flyout!.Hide();
        }
    }
}
