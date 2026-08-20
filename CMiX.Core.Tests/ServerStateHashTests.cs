using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Exercises the hash comparison directly via Server.HandleStateHash rather than over a real
    // network connection - two real .NET processes both mark themselves as MessageSender.WPF, and
    // Server.MessageReceived deliberately ignores anything from that sender, so a live two-Studio-
    // instance test would not exercise the real Studio/Engine path anyway.
    public class ServerStateHashTests
    {
        [Fact]
        public void MatchingHash_SetsIsInSyncTrue()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var server = provider.GetRequiredService<Server>();
            server.IsInSync = false;

            var matchingHash = ProjectStateHash.Compute(project);
            server.HandleStateHash(new MessageStateHash(Guid.NewGuid(), matchingHash));

            Assert.True(server.IsInSync);
        }

        [Fact]
        public void MismatchingHash_SetsIsInSyncFalse()
        {
            var provider = TestServiceProviderFactory.Create();
            var server = provider.GetRequiredService<Server>();
            server.IsInSync = true;

            server.HandleStateHash(new MessageStateHash(Guid.NewGuid(), "not-a-real-hash"));

            Assert.False(server.IsInSync);
        }
    }
}
