// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Networking
{
    public class ControlMessenger
    {
        private readonly List<IMessageSender> _senders = new();

        // True until the peer is joined, and during a snapshot apply. A blocked messenger drops
        // every message.
        public bool IsSendingBlocked { get; set; }

        public void Register(IMessageSender sender) => _senders.Add(sender);
        public void Unregister(IMessageSender sender) => _senders.Remove(sender);

        public void SendMessage(IMessage message)
        {
            if (IsSendingBlocked) return;
            _senders.ForEach(s => s.SendMessage(message));
        }
    }
}
