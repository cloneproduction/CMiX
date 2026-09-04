// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Services;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class SyncSettings : UserControl
    {
        public SyncSettings()
        {
            InitializeComponent();
            EditConnectionButton.Flyout!.Opened += EditConnectionFlyout_Opened;
        }

        private SyncPeer Peer => (SyncPeer)DataContext!;
        private SyncPeer? _subscribedPeer;

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (_subscribedPeer != null)
                _subscribedPeer.PropertyChanged -= Peer_PropertyChanged;
            _subscribedPeer = null;

            if (DataContext is not SyncPeer peer) return;

            peer.PropertyChanged += Peer_PropertyChanged;
            _subscribedPeer = peer;
            ShowEndpoint(peer);
        }

        private void Peer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not SyncPeer peer) return;

            // The sync Flyout acts on the store, so it must not outlive the connection.
            if (e.PropertyName == nameof(SyncPeer.IsConnected) && !peer.IsConnected)
                SyncButton.Flyout?.Hide();

            if (e.PropertyName == nameof(SyncPeer.Status))
                ConnectButton.Content = peer.Status == "Connecting" ? "Connecting..." : "Connect";
        }

        // The endpoint line shows the options of the running peer.
        private void ShowEndpoint(SyncPeer peer)
        {
            var options = peer.Options;
            EndpointText.Text = options == null
                ? string.Empty
                : $"{options.Ip}:{options.Port} db {options.Database}";
        }

        // The fields show the file content. WithFallbacks fills the empty entries, so IPBox
        // always gets four parts.
        private void EditConnectionFlyout_Opened(object? sender, EventArgs e)
        {
            var options = StudioSettings.Load().ToSyncOptions();
            IpBox.IPAddress = options.Ip;
            PortBox.Text = options.Port.ToString(CultureInfo.InvariantCulture);
            DatabaseBox.Text = options.Database.ToString(CultureInfo.InvariantCulture);
            UserBox.Text = options.User;
            PasswordBox.Text = options.Password;
            KeyPrefixBox.Text = options.KeyPrefix;
            PeerNameBox.Text = options.PeerName;
            ErrorText.Text = string.Empty;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!TryReadInt(PortBox.Text, 1, out var port))
            {
                ErrorText.Text = "The port must be a whole number above zero.";
                return;
            }

            if (!TryReadInt(DatabaseBox.Text, 0, out var database))
            {
                ErrorText.Text = "The database must be a whole number of zero or more.";
                return;
            }

            var edited = new SyncOptions(IpBox.IPAddress, port, database, UserBox.Text ?? string.Empty,
                PasswordBox.Text ?? string.Empty, KeyPrefixBox.Text ?? string.Empty,
                PeerNameBox.Text ?? string.Empty, "studio");
            var settings = StudioSettings.FromSyncOptions(edited);

            try
            {
                settings.Save();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                ErrorText.Text = ex.Message;
                return;
            }

            ErrorText.Text = string.Empty;

            // Both calls return at once. The peer connects on a background task.
            var peer = Peer;
            peer.Stop();
            peer.Start(settings.ToSyncOptions(), autoJoin: false);
            ShowEndpoint(peer);
            EditConnectionButton.Flyout!.Hide();
        }

        private static bool TryReadInt(string? text, int minimum, out int value) =>
            int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value >= minimum;

        private void CancelEditConnection_Click(object sender, RoutedEventArgs e) => EditConnectionButton.Flyout!.Hide();

        private void EditConnectionFlyout_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                EditConnectionButton.Flyout!.Hide();
        }

        // The buttons run the bound command. The Flyout closes after the click, so the command
        // still finds its content attached.
        private void Push_Click(object sender, RoutedEventArgs e) => HideSyncFlyout();

        private void Pull_Click(object sender, RoutedEventArgs e) => HideSyncFlyout();

        private void CancelSync_Click(object sender, RoutedEventArgs e) => SyncButton.Flyout!.Hide();

        private void HideSyncFlyout() => Dispatcher.UIThread.Post(() => SyncButton.Flyout?.Hide());

        private void SyncFlyout_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                SyncButton.Flyout!.Hide();
        }
    }
}
