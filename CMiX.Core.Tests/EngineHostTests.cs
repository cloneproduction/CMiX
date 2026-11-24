using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Engine;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // The start of an engine through EngineHost and SyncPeerStartup, with and without a queue, and the start of a studio.
    public class EngineHostTests
    {
        private static SyncOptions EngineOptions => SyncOptions.Defaults with { PeerName = "Engine" };

        // A host with the real graph and an in-memory store, so no test connects to Redis.
        private static EngineHost CreateHost(InMemorySyncStore store) => new(services =>
        {
            services.AddSingleton<Func<SyncOptions, ISyncStore>>(_ => _ => store);
            services.AddSingleton(provider =>
            {
                var peer = ActivatorUtilities.CreateInstance<SyncPeer>(provider);
                peer.Timings = Fast;
                return peer;
            });
        });

        // A provider with the real graph and an in-memory store, so no test connects to Redis.
        private static IServiceProvider CreateProvider(InMemorySyncStore store)
        {
            var services = new ServiceCollection().AddCMiXCore();
            services.AddSingleton<Func<SyncOptions, ISyncStore>>(_ => _ => store);
            return services.BuildServiceProvider();
        }

        [Fact]
        public void ServiceProvider_BeforeStart_IsNull()
        {
            using var host = CreateHost(new InMemorySyncStore());

            Assert.Null(host.ServiceProvider);
            Assert.Null(host.MainloopQueue);
        }

        [Fact]
        public void Start_OfAHost_GivesTheProviderAndTheQueue()
        {
            using var host = CreateHost(new InMemorySyncStore());

            host.Start(EngineOptions);

            Assert.NotNull(host.ServiceProvider.GetRequiredService<SyncPeer>());
            Assert.NotNull(host.MainloopQueue);
        }

        [Fact]
        public void Start_OfAHost_ResolvesPrefabManagerWithoutAReorderService()
        {
            using var host = CreateHost(new InMemorySyncStore());

            host.Start(EngineOptions);

            Assert.Null(host.ServiceProvider.GetRequiredService<PrefabManager>().ManagerReorderService);
        }

        [Fact]
        public void Start_OfAHost_StartsThePeerWithTheGivenOptions()
        {
            using var host = CreateHost(new InMemorySyncStore());

            host.Start(EngineOptions);

            Assert.Equal(EngineOptions, host.ServiceProvider.GetRequiredService<SyncPeer>().Options);
        }

        [Fact]
        public void Start_Twice_Throws()
        {
            using var host = CreateHost(new InMemorySyncStore());
            host.Start(EngineOptions);

            Assert.Throws<InvalidOperationException>(() => host.Start(EngineOptions));
        }

        [Fact]
        public void Start_WithAReadBatch_SetsTheReadBatchOfThePeer()
        {
            using var custom = CreateHost(new InMemorySyncStore());
            using var standard = CreateHost(new InMemorySyncStore());

            custom.Start(EngineOptions, readBatch: 64);
            standard.Start(EngineOptions);

            Assert.Equal(64, custom.ServiceProvider.GetRequiredService<SyncPeer>().ReadBatch);
            Assert.Equal(256, standard.ServiceProvider.GetRequiredService<SyncPeer>().ReadBatch);
        }

        [Fact]
        public async Task Start_WithAQueue_AppliesNothingUntilTheQueueDrains()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            using var host = CreateHost(store);

            host.Start(EngineOptions);
            await WaitUntilAsync(() => host.MainloopQueue.Count > 0);
            await SettleAsync();
            var peer = host.ServiceProvider.GetRequiredService<SyncPeer>();

            Assert.False(peer.IsJoined);
            Assert.True(host.MainloopQueue.Count > 0);

            await JoinAsync(peer, host.MainloopQueue);

            Assert.Single(host.ServiceProvider.GetRequiredService<Project>().CompositionManager.ManagerData.Items);
        }

        [Fact]
        public async Task Dispose_StopsThePeerFast_AndClosesTheQueue()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            var host = CreateHost(store);
            host.Start(EngineOptions);
            var queue = host.MainloopQueue;
            var peer = host.ServiceProvider.GetRequiredService<SyncPeer>();
            await JoinAsync(peer, queue);

            host.Dispose();

            var finished = await Task.WhenAny(peer.Stopped, Task.Delay(1000));
            Assert.Same(peer.Stopped, finished);

            var ran = false;
            queue.Post(() => ran = true);
            Assert.True(ran);
        }

        [Fact]
        public void Dispose_BeforeStart_DoesNothing()
        {
            var host = CreateHost(new InMemorySyncStore());

            host.Dispose();
            host.Dispose();
        }

        [Fact]
        public void Dispose_Twice_DoesNothing()
        {
            var host = CreateHost(new InMemorySyncStore());
            host.Start(EngineOptions);

            host.Dispose();
            host.Dispose();
        }

        [Fact]
        public async Task StartAsEngine_WithoutAQueue_AppliesInline()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            var provider = CreateProvider(store);
            using var peer = provider.GetRequiredService<SyncPeer>();
            peer.Timings = Fast;

            peer.StartAsEngine(EngineOptions);
            await WaitUntilAsync(() => peer.IsJoined);

            Assert.Single(provider.GetRequiredService<Project>().CompositionManager.ManagerData.Items);
        }

        [Fact]
        public void StartAsStudio_OfAPeer_MakesItAWriterThatListsPeersAndCompacts()
        {
            var provider = CreateProvider(new InMemorySyncStore());
            using var peer = provider.GetRequiredService<SyncPeer>();
            peer.Timings = Fast;

            peer.StartAsStudio(provider.GetRequiredService<ControlMessenger>(), action => action(), EngineOptions);

            Assert.True(peer.IsWriter);
            Assert.True(peer.ListPeersEnabled);
            Assert.True(peer.CompactionEnabled);
            Assert.Equal("studio", peer.Options.Role);
        }
    }
}
