using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Engine;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;
using VL.Serialization.MessagePack;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
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
        public void Update_ReturnsTheProviderAndTheQueue_AndKeepsThemOnTheNextCall()
        {
            using var host = CreateHost(new InMemorySyncStore());

            host.Update(out var provider, out var queue, EngineOptions);
            host.Update(out var nextProvider, out var nextQueue, EngineOptions);

            Assert.NotNull(provider.GetRequiredService<SyncPeer>());
            Assert.NotNull(queue);
            Assert.Same(provider, nextProvider);
            Assert.Same(queue, nextQueue);
        }

        [Fact]
        public void Update_StartsThePeerOnce_WithTheOptionsOfTheFirstCall()
        {
            using var host = CreateHost(new InMemorySyncStore());

            host.Update(out var provider, out _, EngineOptions);
            host.Update(out _, out _, EngineOptions with { PeerName = "Other" });

            Assert.Equal(EngineOptions, provider.GetRequiredService<SyncPeer>().Options);
        }

        [Fact]
        public void Update_SetsTheReadBatch()
        {
            using var custom = CreateHost(new InMemorySyncStore());
            using var standard = CreateHost(new InMemorySyncStore());

            custom.Update(out var customProvider, out _, EngineOptions, readBatch: 64);
            standard.Update(out var standardProvider, out _, EngineOptions);

            Assert.Equal(64, customProvider.GetRequiredService<SyncPeer>().ReadBatch);
            Assert.Equal(256, standardProvider.GetRequiredService<SyncPeer>().ReadBatch);
        }

        [Fact]
        public async Task Update_AppliesNothingUntilTheQueueDrains()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            using var host = CreateHost(store);

            host.Update(out var provider, out var queue, EngineOptions);
            await Task.Delay(300);
            var peer = provider.GetRequiredService<SyncPeer>();

            Assert.False(peer.IsJoined);
            Assert.True(queue.Count > 0);

            await JoinAsync(peer, queue);

            Assert.Single(provider.GetRequiredService<Project>().CompositionManager.ManagerData.Items);
        }

        [Fact]
        public async Task Dispose_StopsThePeerFast_AndClosesTheQueue()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            var host = CreateHost(store);
            host.Update(out var provider, out var queue, EngineOptions);
            var peer = provider.GetRequiredService<SyncPeer>();
            await JoinAsync(peer, queue);

            host.Dispose();

            var finished = await Task.WhenAny(peer.Stopped, Task.Delay(1000));
            Assert.Same(peer.Stopped, finished);

            var ran = false;
            queue.Post(() => ran = true);
            Assert.True(ran);
        }

        [Fact]
        public void Dispose_BeforeAnyUpdate_DoesNothing()
        {
            var host = CreateHost(new InMemorySyncStore());

            host.Dispose();
            host.Dispose();
        }

        [Fact]
        public void Dispose_Twice_DoesNothing()
        {
            var host = CreateHost(new InMemorySyncStore());
            host.Update(out _, out _, EngineOptions);

            host.Dispose();
            host.Dispose();
        }
    }
}
