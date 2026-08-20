using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Push/pull applied directly via Server.HandleProjectSnapshot, for the same reason
    // ServerStateHashTests bypasses the real transport - see that file's comment.
    public class ServerSnapshotTests
    {
        [Fact]
        public void ApplyingASnapshot_ReplacesCompositions_NotAppends()
        {
            var senderProvider = TestServiceProviderFactory.Create();
            var sender = senderProvider.GetRequiredService<Project>();
            sender.CompositionManager.AddItem(typeof(Composition));
            sender.CompositionManager.AddItem(typeof(Composition));
            var snapshotModel = (ProjectModel)sender.ToModel();

            var receiverProvider = TestServiceProviderFactory.Create();
            var receiver = receiverProvider.GetRequiredService<Project>();
            var receiverServer = receiverProvider.GetRequiredService<Server>();
            // The receiver already has its own, different composition before the snapshot lands.
            receiver.CompositionManager.AddItem(typeof(Composition));

            receiverServer.HandleProjectSnapshot(new MessageProjectSnapshot(Guid.NewGuid(), snapshotModel));

            Assert.Equal(2, receiver.CompositionManager.ManagerData.Items.Count);
            Assert.True(receiverServer.IsInSync);
        }

        [Fact]
        public void ApplyingASnapshot_MatchesSenderHashAfterward()
        {
            var senderProvider = TestServiceProviderFactory.Create();
            var sender = senderProvider.GetRequiredService<Project>();
            sender.CompositionManager.AddItem(typeof(Composition));
            var senderHash = ProjectStateHash.Compute(sender);

            var receiverProvider = TestServiceProviderFactory.Create();
            var receiver = receiverProvider.GetRequiredService<Project>();
            var receiverServer = receiverProvider.GetRequiredService<Server>();

            receiverServer.HandleProjectSnapshot(new MessageProjectSnapshot(Guid.NewGuid(), (ProjectModel)sender.ToModel()));

            Assert.Equal(senderHash, ProjectStateHash.Compute(receiver));
        }
    }
}
