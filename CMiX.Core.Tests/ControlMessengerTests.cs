using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ControlMessengerTests
    {
        [Fact]
        public void WhileBlocked_ContentMessagesAreDropped_ButSyncProtocolStillSends()
        {
            var messenger = new ControlMessenger();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);
            messenger.IsSendingBlocked = true;

            messenger.SendMessage(new MessageValueChanged());
            messenger.SendMessage(new MessageStateHash(Guid.NewGuid(), "hash"));
            messenger.SendMessage(new MessageProjectSnapshot(Guid.NewGuid(), new ProjectModel()));

            Assert.Equal(2, sender.Sent.Count);
            Assert.All(sender.Sent, m => Assert.True(SyncProtocolMessages.IsSyncProtocol(m)));
        }

        [Fact]
        public void WhileNotBlocked_EverythingSends()
        {
            var messenger = new ControlMessenger();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);

            messenger.SendMessage(new MessageValueChanged());
            messenger.SendMessage(new MessageStateHash(Guid.NewGuid(), "hash"));

            Assert.Equal(2, sender.Sent.Count);
        }
    }
}
