using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.Networking;
using StackExchange.Redis;
using Xunit;
using StreamPosition = CMiX.Core.Networking.StreamPosition;

namespace CMiX.Core.Tests
{
    // Connects once to the local server. When there is none, every test that needs it is skipped.
    public sealed class RedisFixture : IAsyncLifetime
    {
        public bool Available { get; private set; }
        public ConnectionMultiplexer Multiplexer { get; private set; }

        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = 1000,
                ConnectRetry = 1,
                SyncTimeout = 1000
            };
            configuration.EndPoints.Add("127.0.0.1", 6379);

            try
            {
                var multiplexer = await ConnectionMultiplexer.ConnectAsync(configuration);
                await multiplexer.GetDatabase().PingAsync();
                Multiplexer = multiplexer;
                Available = true;
            }
            catch (Exception)
            {
                Available = false;
            }
        }

        public async Task DeleteKeysAsync(string prefix)
        {
            if (!Available)
                return;

            var database = Multiplexer.GetDatabase();
            var server = Multiplexer.GetServer(Multiplexer.GetEndPoints()[0]);

            await foreach (var key in server.KeysAsync(0, prefix + "*"))
                await database.KeyDeleteAsync(key);
        }

        public async Task<int> CountKeysAsync(string prefix)
        {
            if (!Available)
                return 0;

            var server = Multiplexer.GetServer(Multiplexer.GetEndPoints()[0]);
            var count = 0;

            await foreach (var key in server.KeysAsync(0, prefix + "*"))
                count++;

            return count;
        }

        public async Task DisposeAsync()
        {
            if (Multiplexer == null)
                return;

            await Multiplexer.CloseAsync();
            Multiplexer.Dispose();
        }
    }

    [Trait("Category", "Redis")]
    public sealed class RedisSyncStoreIntegrationTests : IClassFixture<RedisFixture>, IAsyncLifetime
    {
        private readonly RedisFixture _fixture;
        private readonly string _prefix = "cmix:test:" + Guid.NewGuid().ToString("N").Substring(0, 8);

        public RedisSyncStoreIntegrationTests(RedisFixture fixture) => _fixture = fixture;

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync() => _fixture.DeleteKeysAsync(_prefix);

        private const string SkipReason = "Redis not reachable on 127.0.0.1:6379";

        private static byte[] Payload(string text) => Encoding.UTF8.GetBytes(text);

        private SyncOptions Options() => SyncOptions.Defaults with { KeyPrefix = _prefix, PeerName = "test" };

        private async Task<RedisSyncStore> ConnectedStore()
        {
            var store = new RedisSyncStore(Options());
            await store.ConnectAsync(CancellationToken.None);
            Assert.True(store.IsConnected);
            return store;
        }

        [SkippableFact]
        public async Task AppendAsync_ThenReadRangeAsync_IsExclusive()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var store = await ConnectedStore();
            var first = await store.AppendAsync(Payload("a"));
            var second = await store.AppendAsync(Payload("b"));
            var third = await store.AppendAsync(Payload("c"));

            var afterFirst = await store.ReadRangeAsync(first, 10);
            var afterThird = await store.ReadRangeAsync(third, 10);
            var all = await store.ReadRangeAsync(StreamPosition.Zero, 2);

            Assert.Equal(2, afterFirst.Count);
            Assert.Equal(second, afterFirst[0].Id);
            Assert.Equal(third, afterFirst[1].Id);
            Assert.Equal(Payload("b"), afterFirst[0].Envelope);
            Assert.Empty(afterThird);
            Assert.Equal(2, all.Count);
        }

        [SkippableFact]
        public async Task ReadTailAsync_IsZeroWhenMissing_AndTheLastIdOtherwise()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var store = await ConnectedStore();

            Assert.Equal(StreamPosition.Zero, await store.ReadTailAsync());

            await store.AppendAsync(Payload("a"));
            var last = await store.AppendAsync(Payload("b"));

            Assert.Equal(last, await store.ReadTailAsync());
        }

        // The tail key spares every heartbeat tick an XINFO STREAM, which returns the first and the
        // last entry with their payloads.
        [SkippableFact]
        public async Task AppendAsync_KeepsTheTailInAKey_AndReadTailAsyncFallsBackToTheStream()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var store = await ConnectedStore();
            var database = _fixture.Multiplexer.GetDatabase();
            var tailKey = _prefix + ":tail";

            Assert.False(await database.KeyExistsAsync(tailKey));
            Assert.Equal(StreamPosition.Zero, await store.ReadTailAsync());

            await store.AppendAsync(Payload("a"));
            await store.AppendAsync(Payload("b"));
            var third = await store.AppendAsync(Payload("c"));

            Assert.Equal(third.ToString(), (string)await database.StringGetAsync(tailKey));
            Assert.Equal(third, await store.ReadTailAsync());

            // A store that was written before the key existed.
            await database.KeyDeleteAsync(tailKey);
            Assert.Equal(third, await store.ReadTailAsync());

            // The fallback read fills the key again, so the next reader skips it.
            Assert.Equal(third.ToString(), (string)await database.StringGetAsync(tailKey));
        }

        [SkippableFact]
        public async Task TrimAsync_RemovesEntriesBelowMinId()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var store = await ConnectedStore();
            await store.AppendAsync(Payload("a"));
            var second = await store.AppendAsync(Payload("b"));
            var third = await store.AppendAsync(Payload("c"));

            await store.TrimAsync(second);

            var entries = await store.ReadRangeAsync(StreamPosition.Zero, 10);
            Assert.Equal(2, entries.Count);
            Assert.Equal(second, entries[0].Id);
            Assert.Equal(third, entries[1].Id);
        }

        [SkippableFact]
        public async Task Snapshot_RoundTrips_AndIsNullWhenAbsent()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var store = await ConnectedStore();

            Assert.Null(await store.ReadSnapshotAsync());

            var written = new Snapshot(Payload("model"), new StreamPosition(42, 1), "peer-1", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc));
            await store.WriteSnapshotAsync(written);

            var read = await store.ReadSnapshotAsync();

            Assert.Equal(written.Model, read.Model);
            Assert.Equal(written.StreamId, read.StreamId);
            Assert.Equal(written.WrittenBy, read.WrittenBy);
            Assert.Equal(written.WrittenAt, read.WrittenAt);
            Assert.Equal(DateTimeKind.Utc, read.WrittenAt.Kind);
        }

        [SkippableFact]
        public async Task HeartbeatAsync_SetsTheTimeToLive_AndListPeersAsyncReturnsThePeer()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var store = await ConnectedStore();
            var fields = new Dictionary<string, string>
            {
                ["name"] = "Studio",
                ["role"] = "studio",
                ["host"] = "desk",
                ["lastAppliedId"] = "7-0"
            };

            await store.HeartbeatAsync("peer-1", fields, TimeSpan.FromSeconds(30));

            var peers = await store.ListPeersAsync();
            var peer = Assert.Single(peers);
            Assert.Equal("peer-1", peer.PeerId);
            Assert.Equal("Studio", peer.Name);
            Assert.Equal("studio", peer.Role);
            Assert.Equal("desk", peer.Host);
            Assert.Equal(new StreamPosition(7, 0), peer.LastAppliedId);

            var timeToLive = await _fixture.Multiplexer.GetDatabase().KeyTimeToLiveAsync(_prefix + ":peers:peer-1");
            Assert.NotNull(timeToLive);
            Assert.True(timeToLive.Value <= TimeSpan.FromSeconds(30));
            Assert.True(timeToLive.Value > TimeSpan.Zero);
        }

        [SkippableFact]
        public async Task ReadBlockingAsync_WakesOnAnAppendFromASecondStore()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var reader = await ConnectedStore();
            await using var writer = await ConnectedStore();

            var reading = reader.ReadBlockingAsync(StreamPosition.Zero, TimeSpan.FromSeconds(5), CancellationToken.None);
            await Task.Delay(200);

            var stopwatch = Stopwatch.StartNew();
            var appended = await writer.AppendAsync(Payload("a"));
            var entries = await reading;
            stopwatch.Stop();

            Assert.Single(entries);
            Assert.Equal(appended, entries[0].Id);
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Woke after {stopwatch.ElapsedMilliseconds} ms.");
        }

        // Runs without a server: the UI thread must never wait for a connect.
        [Fact]
        public async Task ConnectAsync_ToAnUnreachableHost_ReturnsControlAtOnce()
        {
            var store = new RedisSyncStore(SyncOptions.Defaults with { Ip = "10.255.255.1", KeyPrefix = _prefix });

            var stopwatch = Stopwatch.StartNew();
            var connecting = store.ConnectAsync(CancellationToken.None);
            stopwatch.Stop();

            // Well below the 3 s connect timeout of the store.
            Assert.True(stopwatch.ElapsedMilliseconds < 200, $"ConnectAsync blocked for {stopwatch.ElapsedMilliseconds} ms.");

            await connecting;
            Assert.False(store.IsConnected);

            await SyncTestHelpers.WaitUntilAsync(() => store.LastError.Length > 0, 5000);

            var disposing = Stopwatch.StartNew();
            await store.DisposeAsync();
            disposing.Stop();

            Assert.True(disposing.ElapsedMilliseconds < 2500, $"DisposeAsync took {disposing.ElapsedMilliseconds} ms.");
        }

        // The peer name becomes the Redis client name, and Redis refuses a name with a space.
        // StackExchange.Redis removes the space, so the store connects and works.
        [SkippableFact]
        public async Task APeerNameWithASpace_ConnectsAndWorks()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var store = new RedisSyncStore(SyncOptions.Defaults with { KeyPrefix = _prefix, PeerName = "Engine 1" });
            await store.ConnectAsync(CancellationToken.None);

            Assert.True(store.IsConnected);

            var id = await store.AppendAsync(Payload("a"));
            var entries = await store.ReadRangeAsync(StreamPosition.Zero, 10);

            var entry = Assert.Single(entries);
            Assert.Equal(id, entry.Id);
            Assert.Equal(Payload("a"), entry.Envelope);
        }

        // A cancelled connect must leave no open multiplexer behind. The store stays unconnected,
        // and it does not throw.
        [SkippableFact]
        public async Task ConnectAsync_WithACancelledToken_StoresNoMultiplexer()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            var store = new RedisSyncStore(Options());
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await store.ConnectAsync(cts.Token);

            Assert.False(store.IsConnected);
            await Assert.ThrowsAsync<InvalidOperationException>(() => store.ReadTailAsync());

            var disposing = Stopwatch.StartNew();
            await store.DisposeAsync();
            disposing.Stop();

            Assert.True(disposing.ElapsedMilliseconds < 2000, $"DisposeAsync took {disposing.ElapsedMilliseconds} ms.");
        }

        // A wrong port must give a reason. Without one the user sees "Connecting" forever. A wrong
        // password gives no reason on a server without a password, because the server accepts it.
        [Fact]
        public async Task ConnectAsync_ToAWrongPort_ShowsTheReason()
        {
            await using var store = new RedisSyncStore(SyncOptions.Defaults with { Port = 6399, KeyPrefix = _prefix });

            await store.ConnectAsync(CancellationToken.None);

            await SyncTestHelpers.WaitUntilAsync(() => store.LastError.Length > 0, 5000);
            Assert.False(store.IsConnected);
            Assert.Contains("6399", store.LastError);
        }
    }
}
