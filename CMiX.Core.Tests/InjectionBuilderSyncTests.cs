using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class InjectionBuilderSyncTests
    {
        [Fact]
        public void SyncPeer_ResolvesAsSingleton()
        {
            var provider = TestServiceProviderFactory.Create();

            var first = provider.GetRequiredService<SyncPeer>();
            var second = provider.GetRequiredService<SyncPeer>();

            Assert.NotNull(first);
            Assert.Same(first, second);
        }

        [Fact]
        public void SyncPeer_IsNotAWriter_BeforeAnyConfigureCall()
        {
            var provider = TestServiceProviderFactory.Create();

            var peer = provider.GetRequiredService<SyncPeer>();

            Assert.False(peer.IsWriter);
        }

        [Fact]
        public void ISyncTarget_ResolvesToProjectSyncTarget()
        {
            var provider = TestServiceProviderFactory.Create();

            var target = provider.GetRequiredService<ISyncTarget>();

            Assert.IsType<ProjectSyncTarget>(target);
        }

        [Fact]
        public async Task SyncStoreFactory_ResolvesAndCreatesRedisSyncStore()
        {
            var provider = TestServiceProviderFactory.Create();

            var factory = provider.GetRequiredService<Func<SyncOptions, ISyncStore>>();
            var store = factory(SyncOptions.Defaults);

            Assert.IsType<RedisSyncStore>(store);

            await store.DisposeAsync();
        }
    }
}
