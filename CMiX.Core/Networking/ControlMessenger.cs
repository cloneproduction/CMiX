// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Networking
{
    public class ControlMessenger
    {
        private readonly List<IMessageSender> _senders = new();

        // Set while the two sides are not in sync, so local experimentation cannot silently leak
        // across the connection before the user deliberately resolves it with push or pull. Only
        // blocks content messages - the sync protocol's own messages (MessageStateHash,
        // MessageProjectSnapshot) always go through, via SyncProtocolMessages.IsSyncProtocol,
        // otherwise the mismatch could never be resolved.
        public bool IsSendingBlocked { get; set; }

        public void Register(IMessageSender sender) => _senders.Add(sender);
        public void Unregister(IMessageSender sender) => _senders.Remove(sender);

        public void SendMessage(IMessage message)
        {
            if (IsSendingBlocked && !SyncProtocolMessages.IsSyncProtocol(message)) return;
            _senders.ForEach(s => s.SendMessage(message));
        }
    }
}
