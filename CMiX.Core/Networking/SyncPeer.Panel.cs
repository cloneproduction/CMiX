// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Networking
{
    public partial class SyncPeer
    {
        public Guid PeerId { get; } = Guid.NewGuid();
        public SyncOptions Options { get; private set; }
        public string Name => Options?.PeerName ?? string.Empty;
        public string Role => Options?.Role ?? string.Empty;

        public ICommand PushCommand { get; }
        public ICommand PullCommand { get; }
        public ObservableCollection<PeerInfo> Peers { get; }

        public bool ListPeersEnabled { get; set; }

        // Only a writer appends entries and pushes. Engines read and apply.
        public bool IsWriter { get; set; }

        // The time values of the sync. Start takes the set that its run uses.
        public SyncTimings Timings { get; set; } = SyncTimings.Default;

        // How long a value change waits before the compactor writes a new snapshot. Tests shorten it.
        public TimeSpan CompactionDelay
        {
            get => Timings.CompactionDelay;
            set => Timings = Timings with { CompactionDelay = value };
        }

        // How many entries one read returns, and one dispatched action applies. Set before Start.
        public int ReadBatch
        {
            get => Timings.ReadBatch;
            set => Timings = Timings with { ReadBatch = value };
        }

        // A follower that made no read for this long checks the gap first. Tests shorten it.
        public TimeSpan FollowerStalePause
        {
            get => Timings.StalePause;
            set => Timings = Timings with { StalePause = value };
        }

        public int PendingMessages => _outgoing.PendingCount;

        // Completes when the loops have returned and the store is disposed. Completed before the first Start.
        public Task Stopped => _stopped?.Task ?? Task.CompletedTask;

        public event EventHandler<MessageAppliedEventArgs> MessageApplied;
        public event EventHandler<MessageSentEventArgs> MessageSent;

        // Raised on the dispatcher thread after the target applied a snapshot.
        public event EventHandler SnapshotApplied;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInSync), nameof(Status))]
        private bool _isConnected;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInSync), nameof(Status))]
        private bool _isJoined;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInSync))]
        private StreamPosition _lastAppliedId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInSync))]
        private StreamPosition _tailId;

        [ObservableProperty]
        private StreamPosition _lastSentId;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private long _sentMessages;

        [ObservableProperty]
        private long _appliedMessages;

        // The queue appends only while joined, so old edits never land on a store the user has not chosen.
        partial void OnIsJoinedChanged(bool value)
        {
            _messenger.IsSendingBlocked = !value;
            _outgoing.SetOpen(value);
        }

        public bool IsInSync => IsConnected && IsJoined && LastAppliedId == TailId;

        public string Status =>
            _run == null ? "Not started" :
            !IsConnected ? (_wasConnected ? "Reconnecting" : "Connecting") :
            _activity != null ? _activity :
            !IsJoined ? "Not in sync" : "Connected";
    }
}
