// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Networking
{
    // The state-hash/push-pull handshake, shared by Server and Client rather than living on
    // either one alone - both load the same CMiX.Core, so keeping this logic in one place means
    // it works on both sides (Studio and Engine) the moment they both run an updated build,
    // with no separate hand-written protocol logic needed on either side.
    public partial class SyncCoordinator : ObservableObject
    {
        private readonly Project _project;
        private readonly IMessageSender _sender;

        public SyncCoordinator(Project project, IMessageSender sender)
        {
            _project = project;
            _sender = sender;
            PushCommand = new RelayCommand(Push);
            PullCommand = new RelayCommand(Pull);
        }

        // Defaults true so the indicator does not read as a permanent alarm before the first real
        // comparison happens.
        [ObservableProperty]
        private bool isInSync = true;

        public ICommand PushCommand { get; }
        public ICommand PullCommand { get; }

        // Call once, right when a connection is established (Server.ClientConnected /
        // Client.ServerConnected) - not as a reply to receiving one, which would risk a
        // send/reply loop. The other side needs the same "just connected" trigger for this to be
        // a real two-way check.
        public void SendOwnHash() =>
            _sender.SendMessage(new MessageStateHash(Guid.NewGuid(), ProjectStateHash.Compute(_project)));

        // Returns true if this message belongs to the sync protocol and was fully handled here -
        // callers should not forward it anywhere else when this returns true.
        public bool TryHandle(IMessage message)
        {
            switch (message)
            {
                case MessageStateHash hash:
                    IsInSync = hash.Hash == ProjectStateHash.Compute(_project);
                    return true;
                case MessageRequestSnapshot:
                    // The other side asked for a "pull" - reply with our current state, the same
                    // as if the user here had clicked Push.
                    Push();
                    return true;
                case MessageProjectSnapshot snapshot:
                    _project.ApplySnapshot(snapshot.Model);
                    // Adopting the sender's state wholesale makes this side match it by
                    // construction; no need to hash-compare against a hash we do not have on hand.
                    IsInSync = true;
                    return true;
                default:
                    return false;
            }
        }

        // While not in sync, content messages must not be applied - only the sync protocol's own
        // messages, or a mismatch could never be resolved. Mirrors ControlMessenger's outgoing
        // block for the incoming direction.
        public bool ShouldBlockIncoming(IMessage message) =>
            !IsInSync && !SyncProtocolMessages.IsSyncProtocol(message);

        private void Push()
        {
            _sender.SendMessage(new MessageProjectSnapshot(Guid.NewGuid(), (ProjectModel)_project.ToModel()));
            // Handing our current state to the peer makes us the source of truth it now matches,
            // whether this was a deliberate Push or an automatic reply to their pull request.
            IsInSync = true;
        }

        private void Pull() => _sender.SendMessage(new MessageRequestSnapshot(Guid.NewGuid()));
    }
}
