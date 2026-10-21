using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    public class SyncPeerStartupTests
    {
        private static SyncOptions EngineOptions => SyncOptions.Defaults with { PeerName = "Engine" };

        // A provider with the real graph and an in-memory store, so no test connects to Redis.
        private static IServiceProvider CreateProvider(InMemorySyncStore store)
        {
            var services = new ServiceCollection().AddCMiXCore();
            services.AddSingleton<Func<SyncOptions, ISyncStore>>(_ => _ => store);
            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task StartAsEngine_WithAQueue_AppliesNothingUntilDrain()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            var provider = CreateProvider(store);
            var queue = new MainloopQueue();
            using var peer = provider.GetRequiredService<SyncPeer>();
            peer.Timings = Fast;

            peer.StartAsEngine(EngineOptions, queue);
            await Task.Delay(300);

            Assert.False(peer.IsJoined);
            Assert.True(queue.Count > 0);

            await JoinAsync(peer, queue);

            Assert.Single(provider.GetRequiredService<Project>().CompositionManager.ManagerData.Items);
        }

        [Fact]
        public async Task StartAsEngine_WithAQueue_StopsFastAfterClose()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            var provider = CreateProvider(store);
            var queue = new MainloopQueue();
            var peer = provider.GetRequiredService<SyncPeer>();
            peer.Timings = Fast;
            peer.StartAsEngine(EngineOptions, queue);
            await JoinAsync(peer, queue);

            peer.Stop();
            queue.Close();

            var finished = await Task.WhenAny(peer.Stopped, Task.Delay(1000));
            Assert.Same(peer.Stopped, finished);
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
        public void StartAsEngine_WithAQueue_SetsTheReadBatch()
        {
            var custom = CreateProvider(new InMemorySyncStore());
            using var customPeer = custom.GetRequiredService<SyncPeer>();
            customPeer.Timings = Fast;
            customPeer.StartAsEngine(EngineOptions, new MainloopQueue(), readBatch: 64);

            var standard = CreateProvider(new InMemorySyncStore());
            using var standardPeer = standard.GetRequiredService<SyncPeer>();
            standardPeer.Timings = Fast;
            standardPeer.StartAsEngine(EngineOptions, new MainloopQueue());

            Assert.Equal(64, customPeer.ReadBatch);
            Assert.Equal(256, standardPeer.ReadBatch);
        }

        [Fact]
        public void StartAsStudio_MakesThePeerAWriterThatListsPeersAndCompacts()
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
