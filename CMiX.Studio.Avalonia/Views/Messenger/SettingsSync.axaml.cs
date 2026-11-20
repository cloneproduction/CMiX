// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Services;
using CMiX.UI.Controls.Boolean;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class SettingsSync : UserControl
    {
        public SettingsSync()
        {
            InitializeComponent();
            editConnectionButton.Flyout!.Opened += EditConnectionFlyout_Opened;
            useCredentialManagerBox.PropertyChanged += (s, e) =>
            {
                if (e.Property == ToggleButtonCaptioned.IsCheckedProperty)
                    UpdateCredentialBoxes();
            };
            credentialNameBox.TextChanged += (s, e) => UpdateCredentialBoxes();
        }

        private readonly WindowsCredentialStore _store = new();
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
                syncButton.Flyout?.Hide();
        }

        // The endpoint line shows the options of the running peer.
        private void ShowEndpoint(SyncPeer peer)
        {
            var options = peer.Options;
            endpointText.Text = options == null
                ? string.Empty
                : $"{options.Ip}:{options.Port} db {options.Database}, user {options.User} {StudioSettings.Load().Resolve(_store).UserSource}";
        }

        private void UpdateCredentialBoxes()
        {
            var useCredential = useCredentialManagerBox.IsChecked == true && !string.IsNullOrEmpty(credentialNameBox.Text);
            userBox.IsEnabled = !useCredential;
            passwordBox.IsEnabled = !useCredential;
        }

        // The boxes show the file content. WithFallbacks fills the empty entries, so TextBoxIP
        // always gets four parts. User and Password show the file values, never the credential.
        private void EditConnectionFlyout_Opened(object? sender, EventArgs e)
        {
            var settings = StudioSettings.Load();
            var options = settings.ToSyncOptions();
            ipBox.IPAddress = options.Ip;
            portBox.Text = options.Port.ToString(CultureInfo.InvariantCulture);
            databaseBox.Text = options.Database.ToString(CultureInfo.InvariantCulture);
            userBox.Text = options.User;
            passwordBox.Text = options.Password;
            keyPrefixBox.Text = options.KeyPrefix;
            peerNameBox.Text = options.PeerName;
            useCredentialManagerBox.IsChecked = settings.Redis.UseCredentialManager;
            credentialNameBox.Text = settings.Redis.CredentialName;
            UpdateCredentialBoxes();
            errorText.Text = string.Empty;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!TryReadInt(portBox.Text, 1, out var port))
            {
                errorText.Text = "The port must be a whole number above zero.";
                return;
            }

            if (!TryReadInt(databaseBox.Text, 0, out var database))
            {
                errorText.Text = "The database must be a whole number of zero or more.";
                return;
            }

            var edited = new SyncOptions(ipBox.IPAddress, port, database, userBox.Text ?? string.Empty,
                passwordBox.Text ?? string.Empty, keyPrefixBox.Text ?? string.Empty,
                peerNameBox.Text ?? string.Empty, "studio");
            var settings = StudioSettings.FromSyncOptions(edited);
            settings.Redis.UseCredentialManager = useCredentialManagerBox.IsChecked == true;
            settings.Redis.CredentialName = credentialNameBox.Text ?? string.Empty;
            // The file also holds the interface settings. They stay as they are.
            settings.Interface = StudioSettings.Load().Interface ?? new InterfaceSettings();

            try
            {
                settings.Save();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                errorText.Text = ex.Message;
                return;
            }

            errorText.Text = string.Empty;

            // Both calls return at once. The peer connects on a background task.
            var peer = Peer;
            peer.Stop();
            peer.Start(settings.Resolve(_store).Options);
            ShowEndpoint(peer);
            editConnectionButton.Flyout!.Hide();
        }

        private static bool TryReadInt(string? text, int minimum, out int value) =>
            int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value >= minimum;

        private void CancelEditConnection_Click(object sender, RoutedEventArgs e) => editConnectionButton.Flyout!.Hide();

        // The buttons run the bound command. The Flyout closes after the click, so the command
        // still finds its content attached.
        private void Push_Click(object sender, RoutedEventArgs e) => HideSyncFlyout();

        private void Pull_Click(object sender, RoutedEventArgs e) => HideSyncFlyout();

        private void CancelSync_Click(object sender, RoutedEventArgs e) => syncButton.Flyout!.Hide();

        private void HideSyncFlyout() => Dispatcher.UIThread.Post(() => syncButton.Flyout?.Hide());
    }
}
