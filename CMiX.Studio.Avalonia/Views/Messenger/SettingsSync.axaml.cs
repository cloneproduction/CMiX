// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.ViewModels;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class SettingsSync : UserControl
    {
        public SettingsSync()
        {
            InitializeComponent();
        }

        private SettingsSyncViewModel? _viewModel;

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (_viewModel != null)
                Unhook(_viewModel);
            _viewModel = DataContext as SettingsSyncViewModel;

            if (_viewModel == null) return;

            Hook(_viewModel);
            _viewModel.ShowEndpointCommand.Execute(null);
        }

        private void Hook(SettingsSyncViewModel viewModel)
        {
            viewModel.Peer.PropertyChanged += Peer_PropertyChanged;
            viewModel.ConnectionApplied += ViewModel_ConnectionApplied;
        }

        private void Unhook(SettingsSyncViewModel viewModel)
        {
            viewModel.Peer.PropertyChanged -= Peer_PropertyChanged;
            viewModel.ConnectionApplied -= ViewModel_ConnectionApplied;
        }

        private void Peer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not SyncPeer peer) return;

            // The sync Flyout acts on the store, so it must not outlive the connection.
            if (e.PropertyName == nameof(SyncPeer.IsConnected) && !peer.IsConnected)
                syncButton.Flyout?.Hide();
        }

        private void ViewModel_ConnectionApplied(object? sender, EventArgs e) => editConnectionButton.Flyout!.Hide();

        private void EditConnectionFlyout_Opened(object? sender, EventArgs e) => _viewModel?.LoadConnectionCommand.Execute(null);

        private void CancelEditConnection_Click(object sender, RoutedEventArgs e) => editConnectionButton.Flyout!.Hide();

        // The buttons run the bound command. The Flyout closes after the click, so the command
        // still finds its content attached.
        private void Push_Click(object sender, RoutedEventArgs e) => HideSyncFlyout();

        private void Pull_Click(object sender, RoutedEventArgs e) => HideSyncFlyout();

        private void CancelSync_Click(object sender, RoutedEventArgs e) => syncButton.Flyout!.Hide();

        private void HideSyncFlyout() => Dispatcher.UIThread.Post(() => syncButton.Flyout?.Hide());
    }
}
