// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
