using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
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
    [Trait("Category", "Redis")]
    public sealed class SyncPeerRedisIntegrationTests : IClassFixture<RedisFixture>, IAsyncLifetime
    {
        private const string SkipReason = "Redis not reachable on 127.0.0.1:6379";

        private readonly RedisFixture _fixture;
        private readonly ITestOutputHelper _output;
        private readonly RedisPeerHarness _peers = new(
            "cmix:test:" + Guid.NewGuid().ToString("N").Substring(0, 8), SyncOptions.Defaults, joinTimeoutMs: 5000);

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
            await _peers.StopPeersAsync();
            await _fixture.DeleteKeysAsync(_peers.Prefix);

            Assert.Equal(0, await _fixture.CountKeysAsync(_peers.Prefix));
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

        [SkippableFact]
        public async Task StudioOnAnEmptyPrefix_PushesSilently_AndTheEngineJoinsWithTheSameState()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await _peers.StartStudioAsync(studioTarget);

            Assert.True(a.IsJoined);
            Assert.Equal(1, a.SentMessages);

            await using (var store = await _peers.ObserverStoreAsync())
            {
                var snapshot = await store.ReadSnapshotAsync();
                Assert.NotNull(snapshot);
                Assert.Equal(ProjectStateHash.Compute(studioTarget.Model), ProjectStateHash.Compute(ModelOf(snapshot)));
            }

            var engineTarget = new RecordingSyncTarget();
            await _peers.StartEngineAsync("Engine1", engineTarget);

            Assert.Equal(1, engineTarget.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(studioTarget.Model), ProjectStateHash.Compute(engineTarget.Model));
        }

        [SkippableFact]
        public async Task Messages_TravelBothWays_InLessThanOneSecond()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await _peers.StartStudioAsync(studioTarget);
            var engineTarget = new RecordingSyncTarget();
            var b = await _peers.StartEngineAsync("Engine1", engineTarget);
            var writer2 = await _peers.StartWriterAsync("Writer2", new RecordingSyncTarget());

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

            Assert.True(toEngine < 2000, $"The engine applied after {toEngine} ms.");
            Assert.True(toStudio < 2000, $"The studio applied after {toStudio} ms.");
        }

        [SkippableFact]
        public async Task LateEngine_JoinsFromTheSnapshot_AndReplaysOnlyTheEntriesAfterIt()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await _peers.StartStudioAsync(studioTarget, TimeSpan.FromMilliseconds(500));
            var firstEngineTarget = new RecordingSyncTarget();
            await _peers.StartEngineAsync("Engine1", firstEngineTarget);

            for (var i = 0; i < 200; i++)
                a.SendMessage(new MessageValueChanged(Guid.NewGuid(), new GenericValueModel<float> { ID = Guid.NewGuid(), Value = i }));

            await WaitUntilAsync(() => firstEngineTarget.Applied.Count == 200, 15000, () => $"applied={firstEngineTarget.Applied.Count}");

            await using var store = await _peers.ObserverStoreAsync();
            var tail = await store.ReadTailAsync();
            var snapshotId = await WaitForSnapshotAtLeastAsync(store, tail, 15000);

            var lateTarget = new RecordingSyncTarget();
            await _peers.StartEngineAsync("Engine2", lateTarget);

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
            var a = await _peers.StartStudioAsync(studioTarget);
            var firstEngineTarget = new RecordingSyncTarget();
            await _peers.StartEngineAsync("Engine1", firstEngineTarget);
            var secondEngineTarget = new RecordingSyncTarget();
            await _peers.StartEngineAsync("Engine2", secondEngineTarget);
            var writer2 = await _peers.StartWriterAsync("Writer2", new RecordingSyncTarget());

            a.Stop();

            // The engine does not write. A second writer stands in for a peer that keeps writing
            // while the Studio is down.
            var fromB = new MessageOnClick(Guid.NewGuid());
            writer2.SendMessage(fromB);
            await WaitUntilAsync(() => secondEngineTarget.Applied.Any(m => m.ID == fromB.ID));

            var restartedTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var restarted = _peers.NewStudio(restartedTarget);
            restarted.Start(_peers.Options("Studio", "studio"));
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
        public async Task Peers_OfAStudioAndTwoEngines_ListAllThree()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var a = await _peers.StartStudioAsync(studioTarget);
            await _peers.StartEngineAsync("Engine1", new RecordingSyncTarget());
            await _peers.StartEngineAsync("Engine2", new RecordingSyncTarget());

            var names = Array.Empty<string>();
            await WaitUntilAsync(
                () =>
                {
                    names = PeersOf(a).Select(p => p.Name).ToArray();
                    return names.Contains("Studio") && names.Contains("Engine1") && names.Contains("Engine2");
                },
                8000,
                () => "peers=" + string.Join(",", names));

            var peers = PeersOf(a);
            Assert.Equal("studio", peers.Single(p => p.Name == "Studio").Role);
            Assert.Equal("engine", peers.Single(p => p.Name == "Engine1").Role);
            Assert.Equal("engine", peers.Single(p => p.Name == "Engine2").Role);
        }
    }
}
