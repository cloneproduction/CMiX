using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Exercises the sync protocol directly against SyncCoordinator rather than over a real network
    // connection - two real .NET processes both mark themselves as MessageSender.WPF, and
    // Server.MessageReceived deliberately ignores anything from that sender, so a live two-process
    // test would not exercise the real Studio/Engine path anyway. This also covers Server and
    // Client both, since they share this same class instead of duplicating the logic.
    public class SyncCoordinatorTests
    {
        [Fact]
        public void MatchingHash_SetsIsInSyncTrue()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var sync = new SyncCoordinator(project, new RecordingMessageSender(), new ControlMessenger()) { IsInSync = false };

            var matchingHash = ProjectStateHash.Compute(project);
            var handled = sync.TryHandle(new MessageStateHash(Guid.NewGuid(), matchingHash));

            Assert.True(handled);
            Assert.True(sync.IsInSync);
        }

        [Fact]
        public void MismatchingHash_SetsIsInSyncFalse()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var sync = new SyncCoordinator(project, new RecordingMessageSender(), new ControlMessenger()) { IsInSync = true };

            sync.TryHandle(new MessageStateHash(Guid.NewGuid(), "not-a-real-hash"));

            Assert.False(sync.IsInSync);
        }

        [Fact]
        public void ApplyingASnapshot_ReplacesCompositions_NotAppends()
        {
            var sender = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            sender.CompositionManager.AddItem(typeof(Composition));
            sender.CompositionManager.AddItem(typeof(Composition));
            var snapshotModel = (ProjectModel)sender.ToModel();

            var receiver = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var sync = new SyncCoordinator(receiver, new RecordingMessageSender(), new ControlMessenger());
            // The receiver already has its own, different composition before the snapshot lands.
            receiver.CompositionManager.AddItem(typeof(Composition));

            var handled = sync.TryHandle(new MessageProjectSnapshot(Guid.NewGuid(), snapshotModel));

            Assert.True(handled);
            Assert.Equal(2, receiver.CompositionManager.ManagerData.Items.Count);
            Assert.True(sync.IsInSync);
        }

        [Fact]
        public void ApplyingASnapshot_MatchesSenderHashAfterward()
        {
            var sender = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            sender.CompositionManager.AddItem(typeof(Composition));
            var senderHash = ProjectStateHash.Compute(sender);

            var receiver = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var sync = new SyncCoordinator(receiver, new RecordingMessageSender(), new ControlMessenger());

            sync.TryHandle(new MessageProjectSnapshot(Guid.NewGuid(), (ProjectModel)sender.ToModel()));

            Assert.Equal(senderHash, ProjectStateHash.Compute(receiver));
        }

        [Fact]
        public void RequestSnapshot_RepliesWithOwnSnapshot()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var recordingSender = new RecordingMessageSender();
            var sync = new SyncCoordinator(project, recordingSender, new ControlMessenger());

            var handled = sync.TryHandle(new MessageRequestSnapshot(Guid.NewGuid()));

            Assert.True(handled);
            Assert.Single(recordingSender.Sent);
            Assert.IsType<MessageProjectSnapshot>(recordingSender.Sent[0]);
        }

        [Fact]
        public void RequestSnapshot_SetsIsInSyncTrue_OnThePushingSide()
        {
            // Regression: the side that auto-replies to a pull request must also end up marked in
            // sync, or it keeps dropping every content message afterward via ShouldBlockIncoming
            // even though it just handed the peer its current state.
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var sync = new SyncCoordinator(project, new RecordingMessageSender(), new ControlMessenger()) { IsInSync = false };

            sync.TryHandle(new MessageRequestSnapshot(Guid.NewGuid()));

            Assert.True(sync.IsInSync);
        }

        [Fact]
        public void ContentMessage_IsNotHandled_ByTryHandle()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var sync = new SyncCoordinator(project, new RecordingMessageSender(), new ControlMessenger());

            Assert.False(sync.TryHandle(new MessageValueChanged()));
        }

        [Fact]
        public void ShouldBlockIncoming_OnlyBlocksContentMessages_WhileNotInSync()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var sync = new SyncCoordinator(project, new RecordingMessageSender(), new ControlMessenger()) { IsInSync = false };

            Assert.True(sync.ShouldBlockIncoming(new MessageValueChanged()));
            Assert.False(sync.ShouldBlockIncoming(new MessageStateHash(Guid.NewGuid(), "hash")));
        }

        [Fact]
        public void ShouldBlockIncoming_NeverBlocksWhileInSync()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var sync = new SyncCoordinator(project, new RecordingMessageSender(), new ControlMessenger()) { IsInSync = true };

            Assert.False(sync.ShouldBlockIncoming(new MessageValueChanged()));
        }
    }
}
