using CMiX.Core.Networking.Messages;
using VL.Serialization.MessagePack;
using Xunit;

namespace CMiX.Core.Tests
{
    // The two selection messages go over the network inside an envelope. A peer reads them back by their type.
    public class SelectionMessageWireTests
    {
        private static MessageEnvelope Roundtrip(IMessage message) =>
            MessagePackSerialization.Deserialize<MessageEnvelope>(new ReadOnlyMemory<byte>(SyncTestHelpers.Envelope("peer", message)));

        [Fact]
        public void TheSelectMessage_KeepsItsTypeAndValuesOnTheWire()
        {
            var id = Guid.NewGuid();
            var controlID = Guid.NewGuid();

            var message = Assert.IsType<MessageSelectedItemChanged>(Roundtrip(new MessageSelectedItemChanged(id, controlID, 2)).Payload);

            Assert.Equal((id, controlID, 2), (message.ID, message.ControlID, message.Index));
        }

        [Fact]
        public void TheClearSelectionMessage_KeepsItsTypeAndValuesOnTheWire()
        {
            var id = Guid.NewGuid();

            var message = Assert.IsType<MessageRemoveSelectedItem>(Roundtrip(new MessageRemoveSelectedItem(id)).Payload);

            Assert.Equal(id, message.ID);
        }
    }
}
