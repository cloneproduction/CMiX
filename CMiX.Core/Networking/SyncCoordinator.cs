// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Networking
{
    // The state-hash/push-pull handshake, shared by Server and Client so both sides of the
    // connection use the same logic instead of duplicating it.
    public partial class SyncCoordinator : ObservableObject
    {
        private readonly Project _project;
        private readonly IMessageSender _sender;
        private readonly ControlMessenger _messenger;

        public SyncCoordinator(Project project, IMessageSender sender, ControlMessenger messenger)
        {
            _project = project;
            _sender = sender;
            _messenger = messenger;
            PushCommand = new RelayCommand(Push);
            PullCommand = new RelayCommand(Pull);
        }

        // Defaults true so the indicator isn't a false alarm before the first real check.
        [ObservableProperty]
        private bool isInSync = true;

        // Keeps outgoing blocking in step with incoming blocking (ShouldBlockIncoming).
        partial void OnIsInSyncChanged(bool value) => _messenger.IsSendingBlocked = !value;

        public ICommand PushCommand { get; }
        public ICommand PullCommand { get; }

        // Call once on connect (Server.ClientConnected / Client.ServerConnected), not as a reply
        // to receiving one, to avoid a send/reply loop.
        public void SendOwnHash() =>
            _sender.SendMessage(new MessageStateHash(Guid.NewGuid(), ProjectStateHash.Compute(_project)));

        // True if the message was a sync-protocol message and is fully handled - callers should
        // not forward it further.
        public bool TryHandle(IMessage message)
        {
            switch (message)
            {
                case MessageStateHash hash:
                    IsInSync = hash.Hash == ProjectStateHash.Compute(_project);
                    return true;
                case MessageRequestSnapshot:
                    Push();
                    return true;
                case MessageProjectSnapshot snapshot:
                    _project.ApplySnapshot(snapshot.Model);
                    IsInSync = true;
                    return true;
                default:
                    return false;
            }
        }

        // Only the sync protocol's own messages get through while unsynced, or the mismatch
        // could never be resolved.
        public bool ShouldBlockIncoming(IMessage message) =>
            !IsInSync && !SyncProtocolMessages.IsSyncProtocol(message);

        private void Push()
        {
            _sender.SendMessage(new MessageProjectSnapshot(Guid.NewGuid(), (ProjectModel)_project.ToModel()));
            IsInSync = true;
        }

        private void Pull() => _sender.SendMessage(new MessageRequestSnapshot(Guid.NewGuid()));
    }
}
