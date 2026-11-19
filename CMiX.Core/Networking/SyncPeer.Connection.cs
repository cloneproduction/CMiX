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

                // A store can miss the connect event, so the wait is a short poll.
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

        // A stale run can leave a handler on its store. The handler ignores a store that is not the current one.
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

            // Wake the follower of the current run out of a long backoff.
            if (connected)
                run.Follower?.Wake();

            // A reconnect while not joined: the engine joins again, the Studio checks the start.
            if (connected && _started && _afterFirstConnect && !IsJoined)
            {
                var ct = run.Cts.Token;
                if (run.AutoJoin)
                    _ = Task.Run(() => AutoJoinAsync(run), ct);
                else
                    _ = Task.Run(() => RunStartCheckAsync(run), ct);
            }
        }

        // Clears ErrorMessage only when it still shows the store error. Runs on the dispatcher thread.
        private void ClearStoreError()
        {
            if (ErrorMessage == _storeError)
                ErrorMessage = string.Empty;

            _storeError = string.Empty;
        }

        // Shows a reason only when it is not empty.
        private void ShowStoreError(string reason)
        {
            if (reason.Length == 0)
                return;

            ErrorMessage = reason;
            _storeError = reason;
        }
    }
}
