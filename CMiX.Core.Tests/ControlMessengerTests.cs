using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ControlMessengerTests
    {
        [Fact]
        public void SendMessage_WhileSendingIsBlocked_DropsEverything()
        {
            var messenger = new ControlMessenger();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);
            messenger.IsSendingBlocked = true;

            messenger.SendMessage(new MessageValueChanged());
            messenger.SendMessage(new MessageProjectSnapshot(Guid.NewGuid(), new ProjectModel()));

            Assert.Empty(sender.Sent);
        }

        [Fact]
        public void SendMessage_WhileSendingIsNotBlocked_SendsEverything()
        {
            var messenger = new ControlMessenger();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);

            messenger.SendMessage(new MessageValueChanged());
            messenger.SendMessage(new MessageProjectSnapshot(Guid.NewGuid(), new ProjectModel()));

            Assert.Equal(2, sender.Sent.Count);
        }
    }
}
