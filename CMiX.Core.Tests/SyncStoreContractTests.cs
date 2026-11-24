// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Networking;
using Xunit;

namespace CMiX.Core.Tests
{
    // The test double must behave as the real store. Every test runs against InMemorySyncStore and,
    // when a server answers on 127.0.0.1:6379, against RedisSyncStore.
    public sealed class SyncStoreContractTests : IClassFixture<RedisFixture>, IAsyncLifetime
    {
        private const string InMemory = "InMemory";
        private const string Redis = "Redis";
        private const string SkipReason = "Redis not reachable on 127.0.0.1:6379";

        private readonly RedisFixture _fixture;
        private readonly string _prefix = "cmix:test:" + Guid.NewGuid().ToString("N").Substring(0, 8);
        private readonly List<ISyncStore> _stores = new();
        private InMemorySyncStore _memory;

        public SyncStoreContractTests(RedisFixture fixture) => _fixture = fixture;

        public static IEnumerable<object[]> Stores() => new[] { new object[] { InMemory }, new object[] { Redis } };

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            foreach (var store in _stores)
                await store.DisposeAsync();

            await _fixture.DeleteKeysAsync(_prefix);
        }

        private static byte[] Payload(string text) => Encoding.UTF8.GetBytes(text);

        // A store of the kind that is not connected yet. Two Redis stores share one key prefix,
        // and two in-memory stores are the same instance, so a second store sees the first one.
        private ISyncStore NewStore(string kind)
        {
            Skip.If(kind == Redis && !_fixture.Available, SkipReason);

            ISyncStore store = kind == Redis
                ? new RedisSyncStore(SyncOptions.Defaults with { KeyPrefix = _prefix, PeerName = "test" })
                : _memory ??= new InMemorySyncStore();
            if (!_stores.Contains(store))
                _stores.Add(store);

            return store;
        }

        private async Task<ISyncStore> ConnectedStore(string kind)
        {
            var store = NewStore(kind);
            await store.ConnectAsync(CancellationToken.None);
            Assert.True(store.IsConnected);
            return store;
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task AppendAsync_TwoEntries_GivesIncreasingIds(string kind)
        {
            var store = await ConnectedStore(kind);

            var first = await store.AppendAsync(Payload("a"));
            var second = await store.AppendAsync(Payload("b"));

            Assert.True(first > StreamPosition.Zero);
            Assert.True(second > first);
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadRangeAsync_AfterAnId_GivesOnlyTheLaterEntriesInOrder(string kind)
        {
            var store = await ConnectedStore(kind);
            var first = await store.AppendAsync(Payload("a"));
            var second = await store.AppendAsync(Payload("b"));
            var third = await store.AppendAsync(Payload("c"));

            var afterFirst = await store.ReadRangeAsync(first, 10);
            var afterThird = await store.ReadRangeAsync(third, 10);

            Assert.Equal(2, afterFirst.Count);
            Assert.Equal(second, afterFirst[0].Id);
            Assert.Equal(third, afterFirst[1].Id);
            Assert.Equal(Payload("b"), afterFirst[0].Envelope);
            Assert.Empty(afterThird);
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadRangeAsync_WithACount_GivesNoMoreEntries(string kind)
        {
            var store = await ConnectedStore(kind);
            await store.AppendAsync(Payload("a"));
            await store.AppendAsync(Payload("b"));
            await store.AppendAsync(Payload("c"));

            var entries = await store.ReadRangeAsync(StreamPosition.Zero, 2);

            Assert.Equal(2, entries.Count);
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadTailAsync_OfAnEmptyStore_IsZero(string kind)
        {
            var store = await ConnectedStore(kind);

            Assert.Equal(StreamPosition.Zero, await store.ReadTailAsync());
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadTailAsync_AfterTwoAppends_IsTheLastId(string kind)
        {
            var store = await ConnectedStore(kind);
            await store.AppendAsync(Payload("a"));
            var last = await store.AppendAsync(Payload("b"));

            Assert.Equal(last, await store.ReadTailAsync());
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task TrimAsync_ToAnId_KeepsThatEntryAndTheLaterOnes(string kind)
        {
            var store = await ConnectedStore(kind);
            await store.AppendAsync(Payload("a"));
            var second = await store.AppendAsync(Payload("b"));
            var third = await store.AppendAsync(Payload("c"));

            await store.TrimAsync(second);

            var entries = await store.ReadRangeAsync(StreamPosition.Zero, 10);
            Assert.Equal(2, entries.Count);
            Assert.Equal(second, entries[0].Id);
            Assert.Equal(third, entries[1].Id);
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadSnapshotAsync_OfAnEmptyStore_IsNull(string kind)
        {
            var store = await ConnectedStore(kind);

            Assert.Null(await store.ReadSnapshotAsync());
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadSnapshotAsync_AfterAWrite_GivesTheWrittenSnapshot(string kind)
        {
            var store = await ConnectedStore(kind);
            var written = new Snapshot(Payload("model"), new StreamPosition(42, 1), "peer-1", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc));

            await store.WriteSnapshotAsync(written);
            var read = await store.ReadSnapshotAsync();

            Assert.Equal(written.Model, read.Model);
            Assert.Equal(written.StreamId, read.StreamId);
            Assert.Equal(written.WrittenBy, read.WrittenBy);
            Assert.Equal(written.WrittenAt, read.WrittenAt);
            Assert.Equal(DateTimeKind.Utc, read.WrittenAt.Kind);
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadBlockingAsync_WithAnEntry_ReturnsAtOnce(string kind)
        {
            var store = await ConnectedStore(kind);
            await store.AppendAsync(Payload("a"));

            var stopwatch = Stopwatch.StartNew();
            var entries = await store.ReadBlockingAsync(StreamPosition.Zero, 256, TimeSpan.FromSeconds(5), CancellationToken.None);
            stopwatch.Stop();

            Assert.Single(entries);
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Elapsed {stopwatch.ElapsedMilliseconds} ms.");
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadBlockingAsync_AnAppendFromASecondStore_WakesTheRead(string kind)
        {
            var reader = await ConnectedStore(kind);
            var writer = await ConnectedStore(kind);

            var reading = reader.ReadBlockingAsync(StreamPosition.Zero, 256, TimeSpan.FromSeconds(5), CancellationToken.None);
            await Task.Delay(200);

            var stopwatch = Stopwatch.StartNew();
            var appended = await writer.AppendAsync(Payload("a"));
            var entries = await reading;
            stopwatch.Stop();

            Assert.Single(entries);
            Assert.Equal(appended, entries[0].Id);
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Woke after {stopwatch.ElapsedMilliseconds} ms.");
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadBlockingAsync_WithNoEntry_GivesNothingAfterTheTimeout(string kind)
        {
            var store = await ConnectedStore(kind);

            var entries = await store.ReadBlockingAsync(StreamPosition.Zero, 256, TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.Empty(entries);
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ReadBlockingAsync_WithACount_GivesNoMoreEntries(string kind)
        {
            var store = await ConnectedStore(kind);
            for (var i = 0; i < 5; i++)
                await store.AppendAsync(Payload("a"));

            var entries = await store.ReadBlockingAsync(StreamPosition.Zero, 2, TimeSpan.FromSeconds(5), CancellationToken.None);

            Assert.Equal(2, entries.Count);
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task ListPeersAsync_AfterAHeartbeat_GivesThePeer(string kind)
        {
            var store = await ConnectedStore(kind);
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
        }

        [SkippableTheory]
        [MemberData(nameof(Stores))]
        public async Task AppendAsync_OnAStoreThatIsNotConnected_Throws(string kind)
        {
            var store = NewStore(kind);

            Assert.False(store.IsConnected);
            await Assert.ThrowsAsync<InvalidOperationException>(() => store.AppendAsync(Payload("a")));
            await Assert.ThrowsAsync<InvalidOperationException>(() => store.ReadSnapshotAsync());
        }
    }
}
