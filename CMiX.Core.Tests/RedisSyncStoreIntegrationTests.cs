using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

        // SyncStoreContractTests checks the peer list. Only the Redis store sets a time to live on the key.
        [SkippableFact]
        public async Task HeartbeatAsync_WithATimeToLive_SetsItOnThePeerKey()
        {
            Skip.IfNot(_fixture.Available, SkipReason);

            await using var store = await ConnectedStore();

            await store.HeartbeatAsync("peer-1", new Dictionary<string, string> { ["name"] = "Studio" }, TimeSpan.FromSeconds(30));

            var timeToLive = await _fixture.Multiplexer.GetDatabase().KeyTimeToLiveAsync(_prefix + ":peers:peer-1");
            Assert.NotNull(timeToLive);
            Assert.True(timeToLive.Value <= TimeSpan.FromSeconds(30));
            Assert.True(timeToLive.Value > TimeSpan.Zero);
        }

        // Runs without a server: the UI thread must never wait for a connect.
        [Fact]
        public async Task ConnectAsync_ToAnUnreachableHost_ReturnsControlAtOnce()
        {
            var store = new RedisSyncStore(SyncOptions.Defaults with { Ip = "10.255.255.1", KeyPrefix = _prefix });

            var connecting = store.ConnectAsync(CancellationToken.None);

            // The host does not answer, so a connect that returns control at once is still running.
            Assert.False(connecting.IsCompleted);

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
        public async Task ConnectAsync_WithAPeerNameWithASpace_ConnectsAndWorks()
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
