using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using Xunit;

namespace CMiX.Core.Tests
{
    public class SyncProtocolMessagesTests
    {
        [Fact]
        public void StateHashAndSnapshot_AreSyncProtocol()
        {
            Assert.True(SyncProtocolMessages.IsSyncProtocol(new MessageStateHash(Guid.NewGuid(), "hash")));
            Assert.True(SyncProtocolMessages.IsSyncProtocol(new MessageProjectSnapshot(Guid.NewGuid(), new ProjectModel())));
        }

        [Fact]
        public void ContentMessages_AreNotSyncProtocol()
        {
            Assert.False(SyncProtocolMessages.IsSyncProtocol(new MessageValueChanged()));
            Assert.False(SyncProtocolMessages.IsSyncProtocol(new MessageAddItem(Guid.NewGuid(), null, 0)));
            Assert.False(SyncProtocolMessages.IsSyncProtocol(new MessageOnClick(Guid.NewGuid())));
        }
    }
}
