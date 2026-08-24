using System.Collections.Generic;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Tests
{
    // A message sender that just records what it was sent, for tests that assert on which
    // messages went out rather than on any real network effect.
    public class RecordingMessageSender : IMessageSender
    {
        public List<IMessage> Sent { get; } = new();

        public void SendMessage(IMessage message) => Sent.Add(message);
    }
}
