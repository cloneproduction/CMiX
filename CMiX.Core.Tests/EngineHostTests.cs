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
        public void BeforeStart_TheProviderAndTheQueueAreNull()
        {
            using var host = CreateHost(new InMemorySyncStore());

            Assert.Null(host.ServiceProvider);
            Assert.Null(host.MainloopQueue);
        }

        [Fact]
        public void Start_GivesTheProviderAndTheQueue()
        {
            using var host = CreateHost(new InMemorySyncStore());

            host.Start(EngineOptions);

            Assert.NotNull(host.ServiceProvider.GetRequiredService<SyncPeer>());
            Assert.NotNull(host.MainloopQueue);
        }

        [Fact]
        public void Start_StartsThePeerWithTheGivenOptions()
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
        public void Start_SetsTheReadBatch()
        {
            using var custom = CreateHost(new InMemorySyncStore());
            using var standard = CreateHost(new InMemorySyncStore());

            custom.Start(EngineOptions, readBatch: 64);
            standard.Start(EngineOptions);

            Assert.Equal(64, custom.ServiceProvider.GetRequiredService<SyncPeer>().ReadBatch);
            Assert.Equal(256, standard.ServiceProvider.GetRequiredService<SyncPeer>().ReadBatch);
        }

        [Fact]
        public async Task Start_AppliesNothingUntilTheQueueDrains()
        {
            var store = new InMemorySyncStore();
            await WriteProjectSnapshotAsync(store);
            using var host = CreateHost(store);

            host.Start(EngineOptions);
            await Task.Delay(300);
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
    }
}
