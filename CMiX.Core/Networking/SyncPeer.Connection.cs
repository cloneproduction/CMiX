// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VL.Serialization.MessagePack;

namespace CMiX.Core.Networking
{
    public partial class SyncPeer
    {
        private async Task WaitForConnectionAsync(ISyncStore store, SyncTimings timings, CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                await store.ConnectAsync(ct).ConfigureAwait(false);
                if (store.IsConnected) return;

                // A store that misses the event of the server keeps the peer waiting. The connect
                // of a store that already has its client is cheap, so the poll is short.
                await WaitForConnectionEventAsync(store, timings.HeartbeatInterval, ct).ConfigureAwait(false);
                if (store.IsConnected) return;
            }
        }

        private static async Task WaitForConnectionEventAsync(ISyncStore store, TimeSpan timeout, CancellationToken ct)
        {
            var connected = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            void OnChanged(ISyncStore sender, bool value)
            {
                if (value && ReferenceEquals(sender, store)) connected.TrySetResult(true);
            }

            store.ConnectionChanged += OnChanged;
            try
            {
                await Task.WhenAny(connected.Task, Task.Delay(timeout, ct)).ConfigureAwait(false);
            }
            finally
            {
                store.ConnectionChanged -= OnChanged;
            }
        }

        // A run that subscribes after its Stop leaves a handler on the store it left. The sender
        // tells those events from the events of the current store.
        private void OnConnectionChanged(ISyncStore sender, bool connected)
        {
            var run = Volatile.Read(ref _run);
            if (run == null || !ReferenceEquals(sender, run.Store)) return;

            var reason = sender.LastError ?? string.Empty;

            Dispatch(() =>
            {
                if (connected) _wasConnected = true;
                IsConnected = connected;
                if (connected)
                    ClearStoreError();
                else
                    ShowStoreError(reason);
                OnPropertyChanged(nameof(Status));

                // A compaction that threw while the store was down is not retried by itself.
                if (connected && _compactor != null && _compactor.HasFailed)
                    _compactor.Request();
            });

            // The follower can wait out a long backoff. Only the follower of the current run gets
            // the signal, because Stop and a new join clear the field.
            if (connected)
                run.Follower?.Wake();

            // A reconnect while the peer is not in sync. The store state can have changed.
            if (connected && _started && _afterFirstConnect && !IsJoined)
            {
                var ct = run.Cts.Token;
                if (run.AutoJoin)
                    _ = Task.Run(() => AutoJoinAsync(run), ct);
                else
                    _ = Task.Run(() => RunStartCheckAsync(run), ct);
            }
        }

        // Runs on the dispatcher thread.
        private void ClearStoreError()
        {
            if (ErrorMessage == _storeError)
                ErrorMessage = string.Empty;

            _storeError = string.Empty;
        }

        private void ShowStoreError(string reason)
        {
            if (reason.Length == 0)
                return;

            ErrorMessage = reason;
            _storeError = reason;
        }
    }
}
