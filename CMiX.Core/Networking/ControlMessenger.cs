// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Networking
{
    public class ControlMessenger
    {
        private readonly List<IMessageSender> _senders = new();

        // Set while the two sides are unsynced, to block content messages until push or pull
        // resolves it. Sync-protocol messages always go through regardless.
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
