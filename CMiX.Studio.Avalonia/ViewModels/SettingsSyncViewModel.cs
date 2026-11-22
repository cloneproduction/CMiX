// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using System.IO;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels
{
    // The connection settings of the sync peer: the edit boxes, the checks, the save to the file and the restart of the peer.
    public sealed partial class SettingsSyncViewModel : ObservableObject
    {
        public SettingsSyncViewModel(SyncPeer peer, ICredentialStore store, StudioSettingsFile file)
        {
            Peer = peer;
            _store = store;
            _file = file;
        }

        private readonly ICredentialStore _store;
        private readonly StudioSettingsFile _file;

        public SyncPeer Peer { get; }

        public event EventHandler? ConnectionApplied;

        [ObservableProperty]
        private string? ip;

        [ObservableProperty]
        private string? port;

        [ObservableProperty]
        private string? database;

        [ObservableProperty]
        private string? user;

        [ObservableProperty]
        private string? password;

        [ObservableProperty]
        private string? keyPrefix;

        [ObservableProperty]
        private string? peerName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUserEditable))]
        private bool useCredentialManager;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUserEditable))]
        private string? credentialName;

        [ObservableProperty]
        private string errorText = string.Empty;

        [ObservableProperty]
        private string endpointText = string.Empty;

        // The credential gives the user and the password, so the two boxes are off.
        public bool IsUserEditable => !(UseCredentialManager && !string.IsNullOrEmpty(CredentialName));

        // The endpoint line shows the options of the running peer.
        [RelayCommand]
        private void ShowEndpoint()
        {
            var options = Peer.Options;
            EndpointText = options == null
                ? string.Empty
                : $"{options.Ip}:{options.Port} db {options.Database}, user {options.User} {_file.Load().Resolve(_store).UserSource}";
        }

        // The boxes show the file values, never the credential. WithFallbacks gives TextBoxIP its four parts.
        [RelayCommand]
        private void LoadConnection()
        {
            var settings = _file.Load();
            var options = settings.ToSyncOptions();
            Ip = options.Ip;
            Port = options.Port.ToString(CultureInfo.InvariantCulture);
            Database = options.Database.ToString(CultureInfo.InvariantCulture);
            User = options.User;
            Password = options.Password;
            KeyPrefix = options.KeyPrefix;
            PeerName = options.PeerName;
            UseCredentialManager = settings.Redis.UseCredentialManager;
            CredentialName = settings.Redis.CredentialName;
            ErrorText = string.Empty;
        }

        [RelayCommand]
        private void Connect()
        {
            if (!TryReadInt(Port, 1, out var portNumber))
            {
                ErrorText = "The port must be a whole number above zero.";
                return;
            }

            if (!TryReadInt(Database, 0, out var databaseNumber))
            {
                ErrorText = "The database must be a whole number of zero or more.";
                return;
            }

            var settings = EditedSettings(portNumber, databaseNumber);

            try
            {
                _file.Save(settings);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                ErrorText = ex.Message;
                return;
            }

            ErrorText = string.Empty;

            // Both calls return at once. The peer connects on a background task.
            Peer.Stop();
            Peer.Start(settings.Resolve(_store).Options);
            ShowEndpoint();
            ConnectionApplied?.Invoke(this, EventArgs.Empty);
        }

        private StudioSettings EditedSettings(int portNumber, int databaseNumber)
        {
            var edited = new SyncOptions(Ip, portNumber, databaseNumber, User ?? string.Empty,
                Password ?? string.Empty, KeyPrefix ?? string.Empty,
                PeerName ?? string.Empty, "studio");
            var settings = StudioSettings.FromSyncOptions(edited);
            settings.Redis.UseCredentialManager = UseCredentialManager;
            settings.Redis.CredentialName = CredentialName ?? string.Empty;
            // The file also holds the interface settings. They stay as they are.
            settings.Interface = _file.Load().Interface ?? new InterfaceSettings();
            return settings;
        }

        private static bool TryReadInt(string? text, int minimum, out int value) =>
            int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value >= minimum;
    }
}
