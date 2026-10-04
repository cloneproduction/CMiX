using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;
using VL.Serialization.MessagePack;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

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

        // A provider with the real graph and an in-memory store, so no test connects to Redis.
        private static IServiceProvider CreateProvider(InMemorySyncStore store)
        {
            var services = new ServiceCollection();
            new InjectionBuilder().ConfigureAllServices(services);
            services.AddSingleton<Func<SyncOptions, ISyncStore>>(_ => _ => store);
            return services.BuildServiceProvider();
        }

        private static SyncOptions EngineOptions => SyncOptions.Defaults with { PeerName = "Engine" };

        private static async Task WriteProjectSnapshotAsync(InMemorySyncStore store)
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var model = project.ToModel();

            await store.ConnectAsync(default);
            await store.WriteSnapshotAsync(new Snapshot(
                MessagePackSerialization.Serialize((ProjectModel)model), StreamPosition.Zero, "other", DateTime.UtcNow));
        }

        private static async Task JoinAsync(SyncPeer peer, MainloopQueue queue)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (!peer.IsJoined)
            {
                if (watch.ElapsedMilliseconds > 5000)
                    throw new TimeoutException("The peer did not join in time.");

                queue.Drain();
                await Task.Delay(10);
            }
        }

        [Fact]
        public async Task ConfigureEngineTransport_WithAQueue_AppliesNothingUntilDrain()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            var provider = CreateProvider(store);
            var queue = new MainloopQueue();
            using var peer = provider.GetRequiredService<SyncPeer>();
            peer.Timings = Fast;

            new InjectionBuilder().ConfigureEngineTransport(provider, EngineOptions, queue);
            await Task.Delay(300);

            Assert.False(peer.IsJoined);
            Assert.True(queue.Count > 0);

            await JoinAsync(peer, queue);

            var project = provider.GetRequiredService<Project>();
            Assert.Single(project.CompositionManager.ManagerData.Items);
        }

        [Fact]
        public async Task ConfigureEngineTransport_WithAQueue_StopsFastAfterClose()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            var provider = CreateProvider(store);
            var queue = new MainloopQueue();
            var peer = provider.GetRequiredService<SyncPeer>();
            peer.Timings = Fast;
            new InjectionBuilder().ConfigureEngineTransport(provider, EngineOptions, queue);
            await JoinAsync(peer, queue);

            peer.Stop();
            queue.Close();

            var finished = await Task.WhenAny(peer.Stopped, Task.Delay(1000));
            Assert.Same(peer.Stopped, finished);
        }

        [Fact]
        public async Task ConfigureEngineTransport_WithoutAQueue_AppliesInline()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            var provider = CreateProvider(store);
            using var peer = provider.GetRequiredService<SyncPeer>();
            peer.Timings = Fast;

            new InjectionBuilder().ConfigureEngineTransport(provider, EngineOptions);
            await WaitUntilAsync(() => peer.IsJoined);

            Assert.Single(provider.GetRequiredService<Project>().CompositionManager.ManagerData.Items);
        }

        [Fact]
        public void ConfigureEngineTransport_WithAQueue_SetsTheReadBatch()
        {
            var builder = new InjectionBuilder();

            var custom = CreateProvider(new InMemorySyncStore());
            using var customPeer = custom.GetRequiredService<SyncPeer>();
            customPeer.Timings = Fast;
            builder.ConfigureEngineTransport(custom, EngineOptions, new MainloopQueue(), readBatch: 64);

            var standard = CreateProvider(new InMemorySyncStore());
            using var standardPeer = standard.GetRequiredService<SyncPeer>();
            standardPeer.Timings = Fast;
            builder.ConfigureEngineTransport(standard, EngineOptions, new MainloopQueue());

            Assert.Equal(64, customPeer.ReadBatch);
            Assert.Equal(256, standardPeer.ReadBatch);
        }
    }
}
