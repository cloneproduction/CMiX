using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Microsoft.Extensions.DependencyInjection;
using VL.Serialization.MessagePack;
using Xunit;
using Xunit.Abstractions;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // Drives whole peers over the local Redis or Memurai server. Every test uses its own key
    // prefix and deletes it again.
    //
    // A reconnect after a server outage is not tested here. It needs a stop of the server, and the
    // test cannot stop it.
    public sealed class SyncPeerRedisIntegrationTests : IClassFixture<RedisFixture>, IAsyncLifetime
    {
        private const string SkipReason = "Redis not reachable on 127.0.0.1:6379";

        private readonly RedisFixture _fixture;
        private readonly ITestOutputHelper _output;
        private readonly string _prefix = "cmix:test:" + Guid.NewGuid().ToString("N").Substring(0, 8);
        private readonly List<SyncPeer> _peers = new();

        public SyncPeerRedisIntegrationTests(RedisFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        public Task InitializeAsync() => Task.CompletedTask;

        // The keys go only after every peer has stopped. A last heartbeat or append of a running
        // peer would write the prefix again after the delete.
        public async Task DisposeAsync()
        {
            foreach (var peer in _peers)
                peer.Dispose();

            foreach (var peer in _peers)
                await Task.WhenAny(peer.Stopped, Task.Delay(TimeSpan.FromSeconds(10)));

            _peers.Clear();
            await _fixture.DeleteKeysAsync(_prefix);

            Assert.Equal(0, await _fixture.CountKeysAsync(_prefix));
        }

        private static ProjectModel ModelWithOneComposition()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return (ProjectModel)project.ToModel();
        }

        private static ProjectModel ModelOf(Snapshot snapshot) =>
            MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));

        private SyncOptions Options(string name, string role) =>
            SyncOptions.Defaults with { KeyPrefix = _prefix, PeerName = name, Role = role };

        private SyncPeer NewPeer(ISyncTarget target, bool isWriter = true)
        {
            var peer = new SyncPeer(target, new ControlMessenger(), o => new RedisSyncStore(o)) { IsWriter = isWriter };
            _peers.Add(peer);
            return peer;
        }

        private SyncPeer NewStudio(ISyncTarget target, TimeSpan? compactionDelay = null)
        {
            var peer = NewPeer(target);
            peer.ListPeersEnabled = true;
            peer.CompactionEnabled = true;
            if (compactionDelay != null)
                peer.CompactionDelay = compactionDelay.Value;

            return peer;
        }

        private async Task<SyncPeer> StartStudioAsync(ISyncTarget target, TimeSpan? compactionDelay = null)
        {
            var peer = NewStudio(target, compactionDelay);
            peer.Start(Options("Studio", "studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            return peer;
        }

        private async Task<SyncPeer> StartEngineAsync(string name, ISyncTarget target)
        {
            var peer = NewPeer(target, isWriter: false);
            peer.Start(Options(name, "engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            return peer;
        }

        // A second writer, for the tests that check the engine-to-Studio direction. The protocol
        // stays symmetric, so only the wiring keeps engines from sending.
        private async Task<SyncPeer> StartWriterAsync(string name, ISyncTarget target)
        {
            var peer = NewPeer(target);
            peer.Start(Options(name, "studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined, detail: () => $"status={peer.Status} error={peer.ErrorMessage}");
            return peer;
        }

        private async Task<RedisSyncStore> ObserverStoreAsync()
        {
            var store = new RedisSyncStore(Options("observer", "test"));
            await store.ConnectAsync(CancellationToken.None);
            Assert.True(store.IsConnected);
            return store;
        }

        private static async Task<StreamPosition> WaitForSnapshotAtLeastAsync(RedisSyncStore store, StreamPosition position, int timeoutMs)
        {
            var watch = Stopwatch.StartNew();
            while (true)
            {
                var snapshotId = await store.ReadSnapshotIdAsync();
                if (snapshotId >= position)
                    return snapshotId;

                if (watch.ElapsedMilliseconds > timeoutMs)
                    throw new TimeoutException($"The compactor wrote no snapshot at {position}. It is at {snapshotId}.");

                await Task.Delay(100);
            }
        }

        // The peer list changes on a background thread, because the tests set no dispatcher.
        private static PeerInfo[] PeerSnapshot(SyncPeer peer)
        {
            for (var attempt = 0; attempt < 20; attempt++)
            {
                try
                {
                    return peer.Peers.ToArray();
                }
                catch (Exception)
                {
                    Thread.Sleep(5);
                }
            }

            return Array.Empty<PeerInfo>();
        }

        [SkippableFact]
        public async Task StudioOnAnEmptyPrefix_PushesSilently_AndTheEngineJoinsWithTheSameState()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await StartStudioAsync(studioTarget);

            Assert.True(a.IsJoined);
            Assert.Equal(1, a.SentMessages);

            await using (var store = await ObserverStoreAsync())
            {
                var snapshot = await store.ReadSnapshotAsync();
                Assert.NotNull(snapshot);
                Assert.Equal(ProjectStateHash.Compute(studioTarget.Model), ProjectStateHash.Compute(ModelOf(snapshot)));
            }

            var engineTarget = new RecordingSyncTarget();
            await StartEngineAsync("Engine1", engineTarget);

            Assert.Equal(1, engineTarget.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(studioTarget.Model), ProjectStateHash.Compute(engineTarget.Model));
        }

        [SkippableFact]
        public async Task Messages_TravelBothWays_InLessThanOneSecond()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await StartStudioAsync(studioTarget);
            var engineTarget = new RecordingSyncTarget();
            var b = await StartEngineAsync("Engine1", engineTarget);
            var writer2 = await StartWriterAsync("Writer2", new RecordingSyncTarget());

            var fromA = new MessageOnClick(Guid.NewGuid());
            var watch = Stopwatch.StartNew();
            a.SendMessage(fromA);
            await WaitUntilAsync(() => engineTarget.Applied.Any(m => m.ID == fromA.ID));
            var toEngine = watch.ElapsedMilliseconds;

            // The engine does not write. A second writer stands in for the engine-to-Studio
            // direction, so the protocol claim stays tested.
            var fromB = new MessageOnClick(Guid.NewGuid());
            watch.Restart();
            writer2.SendMessage(fromB);
            await WaitUntilAsync(() => studioTarget.Applied.Any(m => m.ID == fromB.ID));
            var toStudio = watch.ElapsedMilliseconds;

            _output.WriteLine($"Studio to engine: {toEngine} ms. Engine to studio: {toStudio} ms.");

            Assert.True(toEngine < 1000, $"The engine applied after {toEngine} ms.");
            Assert.True(toStudio < 1000, $"The studio applied after {toStudio} ms.");
        }

        [SkippableFact]
        public async Task LateEngine_JoinsFromTheSnapshot_AndReplaysOnlyTheEntriesAfterIt()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await StartStudioAsync(studioTarget, TimeSpan.FromMilliseconds(500));
            var firstEngineTarget = new RecordingSyncTarget();
            await StartEngineAsync("Engine1", firstEngineTarget);

            for (var i = 0; i < 200; i++)
                a.SendMessage(new MessageValueChanged(Guid.NewGuid(), new GenericValueModel<float> { ID = Guid.NewGuid(), Value = i }));

            await WaitUntilAsync(() => firstEngineTarget.Applied.Count == 200, 15000, () => $"applied={firstEngineTarget.Applied.Count}");

            await using var store = await ObserverStoreAsync();
            var tail = await store.ReadTailAsync();
            var snapshotId = await WaitForSnapshotAtLeastAsync(store, tail, 15000);

            var lateTarget = new RecordingSyncTarget();
            await StartEngineAsync("Engine2", lateTarget);

            var afterSnapshot = await store.ReadRangeAsync(snapshotId, 1000);

            Assert.Equal(1, lateTarget.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(studioTarget.Model), ProjectStateHash.Compute(lateTarget.Model));
            Assert.True(lateTarget.Applied.Count <= afterSnapshot.Count,
                $"The late engine applied {lateTarget.Applied.Count} of the {afterSnapshot.Count} entries after the snapshot.");

            _output.WriteLine($"The late engine replayed {lateTarget.Applied.Count} entries after the snapshot at {snapshotId}.");
        }

        [SkippableFact]
        public async Task AfterAStudioCrash_TheEnginesGoOn_AndTheStudioRejoins()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await StartStudioAsync(studioTarget);
            var firstEngineTarget = new RecordingSyncTarget();
            await StartEngineAsync("Engine1", firstEngineTarget);
            var secondEngineTarget = new RecordingSyncTarget();
            await StartEngineAsync("Engine2", secondEngineTarget);
            var writer2 = await StartWriterAsync("Writer2", new RecordingSyncTarget());

            a.Stop();

            // The engine does not write. A second writer stands in for a peer that keeps writing
            // while the Studio is down.
            var fromB = new MessageOnClick(Guid.NewGuid());
            writer2.SendMessage(fromB);
            await WaitUntilAsync(() => secondEngineTarget.Applied.Any(m => m.ID == fromB.ID));

            var restartedTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var restarted = NewStudio(restartedTarget);
            restarted.Start(Options("Studio", "studio"), autoJoin: false);
            await WaitUntilAsync(() => restarted.Status == "Not in sync", detail: () => $"status={restarted.Status} error={restarted.ErrorMessage}");

            Assert.Equal(StartCheck.NotInSync, await restarted.CheckStartAsync());

            await restarted.JoinAsync();

            Assert.True(restarted.IsJoined);
            Assert.Equal(ProjectStateHash.Compute(secondEngineTarget.Model), ProjectStateHash.Compute(restartedTarget.Model));

            // The check turns AlreadyInSync when the compactor wrote the snapshot for the entries
            // the studio replayed. That write waits for the compaction delay.
            var check = await restarted.CheckStartAsync();
            var watch = Stopwatch.StartNew();
            while (check != StartCheck.AlreadyInSync && watch.ElapsedMilliseconds < 15000)
            {
                await Task.Delay(100);
                check = await restarted.CheckStartAsync();
            }

            Assert.Equal(StartCheck.AlreadyInSync, check);
        }

        [SkippableFact]
        public async Task ThePeerList_ShowsTheStudioAndBothEngines()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await StartStudioAsync(studioTarget);
            await StartEngineAsync("Engine1", new RecordingSyncTarget());
            await StartEngineAsync("Engine2", new RecordingSyncTarget());

            var names = Array.Empty<string>();
            await WaitUntilAsync(
                () =>
                {
                    names = PeerSnapshot(a).Select(p => p.Name).ToArray();
                    return names.Contains("Studio") && names.Contains("Engine1") && names.Contains("Engine2");
                },
                8000,
                () => "peers=" + string.Join(",", names));

            var peers = PeerSnapshot(a);
            Assert.Equal("studio", peers.Single(p => p.Name == "Studio").Role);
            Assert.Equal("engine", peers.Single(p => p.Name == "Engine1").Role);
            Assert.Equal("engine", peers.Single(p => p.Name == "Engine2").Role);
        }
    }
}
