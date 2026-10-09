// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Networking
{
    // Carries a stream entry and its message for the MessageApplied event.
    public sealed class MessageAppliedEventArgs : EventArgs
    {
        public MessageAppliedEventArgs(StreamEntry entry, IMessage message)
        {
            Entry = entry;
            Message = message;
        }

        public StreamEntry Entry { get; }
        public IMessage Message { get; }
    }

    // Carries the message for the MessageSent event.
    public sealed class MessageSentEventArgs : EventArgs
    {
        public MessageSentEventArgs(IMessage message)
        {
            Message = message;
        }

        public IMessage Message { get; }
    }
}
