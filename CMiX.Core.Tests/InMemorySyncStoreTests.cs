using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.Networking;
using Xunit;

namespace CMiX.Core.Tests
{
    public class InMemorySyncStoreTests
    {
        private static async Task<InMemorySyncStore> ConnectedStore()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(CancellationToken.None);
            return store;
        }

        private static byte[] Payload(string text) => Encoding.UTF8.GetBytes(text);

        [Fact]
        public async Task AppendAsync_ReturnsIncreasingIds()
        {
            var store = await ConnectedStore();

            var first = await store.AppendAsync(Payload("a"));
            var second = await store.AppendAsync(Payload("b"));

            Assert.True(first > StreamPosition.Zero);
            Assert.True(second > first);
        }

        [Fact]
        public async Task ReadRangeAsync_ReturnsOnlyLaterEntries_InOrder()
        {
            var store = await ConnectedStore();
            var first = await store.AppendAsync(Payload("a"));
            var second = await store.AppendAsync(Payload("b"));
            var third = await store.AppendAsync(Payload("c"));

            var entries = await store.ReadRangeAsync(first, 10);

            Assert.Equal(2, entries.Count);
            Assert.Equal(second, entries[0].Id);
            Assert.Equal(third, entries[1].Id);
        }

        [Fact]
        public async Task ReadRangeAsync_HonorsCount()
        {
            var store = await ConnectedStore();
            await store.AppendAsync(Payload("a"));
            await store.AppendAsync(Payload("b"));
            await store.AppendAsync(Payload("c"));

            var entries = await store.ReadRangeAsync(StreamPosition.Zero, 2);

            Assert.Equal(2, entries.Count);
        }

        [Fact]
        public async Task ReadTailAsync_IsZero_WhenEmpty()
        {
            var store = await ConnectedStore();

            Assert.Equal(StreamPosition.Zero, await store.ReadTailAsync());
        }

        [Fact]
        public async Task ReadTailAsync_IsTheLastId()
        {
            var store = await ConnectedStore();
            await store.AppendAsync(Payload("a"));
            var last = await store.AppendAsync(Payload("b"));

            Assert.Equal(last, await store.ReadTailAsync());
        }

        [Fact]
        public async Task TrimAsync_RemovesOlderEntries_AndKeepsTheEntryAtMinId()
        {
            var store = await ConnectedStore();
            await store.AppendAsync(Payload("a"));
            var second = await store.AppendAsync(Payload("b"));
            var third = await store.AppendAsync(Payload("c"));

            await store.TrimAsync(second);

            var entries = await store.ReadRangeAsync(StreamPosition.Zero, 10);
            Assert.Equal(2, entries.Count);
            Assert.Equal(second, entries[0].Id);
            Assert.Equal(third, entries[1].Id);
        }

        [Fact]
        public async Task Snapshot_RoundTrips()
        {
            var store = await ConnectedStore();
            var written = new Snapshot(Payload("model"), new StreamPosition(42, 1), "peer-1", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc));

            await store.WriteSnapshotAsync(written);
            var read = await store.ReadSnapshotAsync();

            Assert.Equal(written.StreamId, read.StreamId);
            Assert.Equal(written.WrittenBy, read.WrittenBy);
            Assert.Equal(written.WrittenAt, read.WrittenAt);
            Assert.Equal(written.Model, read.Model);
        }

        [Fact]
        public async Task ReadSnapshotAsync_IsNull_WhenAbsent()
        {
            var store = await ConnectedStore();

            Assert.Null(await store.ReadSnapshotAsync());
        }

        [Fact]
        public async Task ReadBlockingAsync_ReturnsAtOnce_WhenEntriesExist()
        {
            var store = await ConnectedStore();
            await store.AppendAsync(Payload("a"));

            var stopwatch = Stopwatch.StartNew();
            var entries = await store.ReadBlockingAsync(StreamPosition.Zero, TimeSpan.FromSeconds(5), CancellationToken.None);
            stopwatch.Stop();

            Assert.Single(entries);
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Elapsed {stopwatch.ElapsedMilliseconds} ms.");
        }

        [Fact]
        public async Task ReadBlockingAsync_ReturnsTheNewEntry_WhenOneIsAppendedDuringTheWait()
        {
            var store = await ConnectedStore();

            var reading = store.ReadBlockingAsync(StreamPosition.Zero, TimeSpan.FromSeconds(5), CancellationToken.None);
            await Task.Delay(50);
            var appended = await store.AppendAsync(Payload("a"));

            var entries = await reading;

            Assert.Single(entries);
            Assert.Equal(appended, entries[0].Id);
        }

        [Fact]
        public async Task ReadBlockingAsync_ReturnsEmpty_OnTimeout()
        {
            var store = await ConnectedStore();

            var entries = await store.ReadBlockingAsync(StreamPosition.Zero, TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.Empty(entries);
        }

        [Fact]
        public async Task ReadBlockingAsync_OnAnEmptyStore_WithATimeout_DoesExactlyOneRangeRead()
        {
            var store = await ConnectedStore();
            var before = store.ReadRangeCalls;

            await store.ReadBlockingAsync(StreamPosition.Zero, TimeSpan.FromMilliseconds(200), CancellationToken.None);

            Assert.Equal(before + 1, store.ReadRangeCalls);
        }

        [Fact]
        public async Task HeartbeatAsync_ThenListPeersAsync_ReturnsThePeer()
        {
            var store = await ConnectedStore();
            var fields = new Dictionary<string, string>
            {
                ["name"] = "Studio",
                ["role"] = "studio",
                ["host"] = "desk",
                ["lastAppliedId"] = "7-0"
            };

            await store.HeartbeatAsync("peer-1", fields, TimeSpan.FromSeconds(10));
            var peers = await store.ListPeersAsync();

            var peer = Assert.Single(peers);
            Assert.Equal("peer-1", peer.PeerId);
            Assert.Equal("Studio", peer.Name);
            Assert.Equal("studio", peer.Role);
            Assert.Equal("desk", peer.Host);
            Assert.Equal(new StreamPosition(7, 0), peer.LastAppliedId);
        }

        [Fact]
        public async Task DisconnectedStore_Throws()
        {
            var store = await ConnectedStore();
            var raised = new List<bool>();
            store.ConnectionChanged += (_, connected) => raised.Add(connected);

            store.SimulateDisconnect();

            Assert.False(store.IsConnected);
            await Assert.ThrowsAsync<InvalidOperationException>(() => store.AppendAsync(Payload("a")));
            await Assert.ThrowsAsync<InvalidOperationException>(() => store.ReadSnapshotAsync());

            store.SimulateReconnect();

            Assert.True(store.IsConnected);
            Assert.Equal(new[] { false, true }, raised);
        }
    }
}
