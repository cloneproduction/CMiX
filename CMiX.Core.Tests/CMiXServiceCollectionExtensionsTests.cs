using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class CMiXServiceCollectionExtensionsTests
    {
        private static IServiceProvider Build() => new ServiceCollection().AddCMiXCore().BuildServiceProvider();

        [Fact]
        public void SyncPeer_ResolvesAsSingleton()
        {
            var provider = Build();

            var first = provider.GetRequiredService<SyncPeer>();
            var second = provider.GetRequiredService<SyncPeer>();

            Assert.NotNull(first);
            Assert.Same(first, second);
        }

        [Fact]
        public void SyncPeer_IsNotAWriter_BeforeAnyStartCall()
        {
            var peer = Build().GetRequiredService<SyncPeer>();

            Assert.False(peer.IsWriter);
        }

        [Fact]
        public void ISyncTarget_ResolvesToProjectSyncTarget()
        {
            var target = Build().GetRequiredService<ISyncTarget>();

            Assert.IsType<ProjectSyncTarget>(target);
        }

        [Fact]
        public async Task SyncStoreFactory_ResolvesAndCreatesRedisSyncStore()
        {
            var factory = Build().GetRequiredService<Func<SyncOptions, ISyncStore>>();
            var store = factory(SyncOptions.Defaults);

            Assert.IsType<RedisSyncStore>(store);

            await store.DisposeAsync();
        }

        [Fact]
        public void AddCMiXCore_ReturnsTheCollection()
        {
            var services = new ServiceCollection();

            Assert.Same(services, services.AddCMiXCore());
        }
    }
}
