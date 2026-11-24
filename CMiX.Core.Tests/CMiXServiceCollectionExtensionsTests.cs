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
        public void GetRequiredService_OfSyncPeer_GivesOneInstance()
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
        public void GetRequiredService_OfISyncTarget_GivesTheProjectSyncTarget()
        {
            var target = Build().GetRequiredService<ISyncTarget>();

            Assert.IsType<ProjectSyncTarget>(target);
        }

        [Fact]
        public async Task GetRequiredService_OfTheStoreFactory_CreatesARedisSyncStore()
        {
            var factory = Build().GetRequiredService<Func<SyncOptions, ISyncStore>>();
            var store = factory(SyncOptions.Defaults);

            Assert.IsType<RedisSyncStore>(store);

            await store.DisposeAsync();
        }

        [Fact]
        public void AddCMiXCore_OfACollection_ReturnsTheSameCollection()
        {
            var services = new ServiceCollection();

            Assert.Same(services, services.AddCMiXCore());
        }
    }
}
