// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // The writer: the push of the studio, the order of the outgoing queue, and an engine that sends nothing. The store is in memory. Peers apply inline, because no dispatcher is set.
    public class SyncPeerSendTests
    {
        [Fact]
        public async Task Engine_SendMessage_AppendsNothing()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined);

            peer.SendMessage(new MessageOnClick(Guid.NewGuid()));

            Assert.Equal(0, peer.PendingMessages);
            Assert.Empty(store.Entries);
        }

        [Fact]
        public async Task Engine_PushAsync_SetsErrorAndAppendsNothing()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined);

            await peer.PushAsync();

            Assert.Equal("This peer does not write.", peer.ErrorMessage);
            Assert.Empty(store.Entries);
        }

        [Fact]
        public async Task Push_WritesSnapshotAppendsEntryTrims_AndRunningPeerApplies()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget();
            var targetB = new RecordingSyncTarget();
            using var a = CreatePeer(targetA, store);
            using var b = CreatePeer(targetB, store);
            a.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => a.IsJoined);
            b.StartAndJoin(Options("B"));
            await WaitUntilAsync(() => b.IsJoined);

            for (var i = 0; i < 3; i++)
                a.SendMessage(new MessageOnClick(Guid.NewGuid()));
            await WaitUntilAsync(() => targetB.Applied.Count == 3);

            targetA.Model = ModelWithOneComposition();
            await a.PushAsync();
            await WaitUntilAsync(() => targetB.SnapshotsApplied == 2);

            var snapshot = await store.ReadSnapshotAsync();
            Assert.Single(store.Entries);
            Assert.Equal(store.Entries[0].Id, snapshot.StreamId);
            Assert.Equal(ProjectStateHash.Compute(targetA.Model), ProjectStateHash.Compute(targetB.Model));
            await WaitUntilAsync(() => b.LastAppliedId == a.LastAppliedId);
        }

        [Fact]
        public async Task Outgoing_KeepsOrder()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget();
            var targetB = new RecordingSyncTarget();
            using var a = CreatePeer(targetA, store);
            using var b = CreatePeer(targetB, store);
            a.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => a.IsJoined);
            b.StartAndJoin(Options("B"));
            await WaitUntilAsync(() => b.IsJoined);

            var ids = Enumerable.Range(0, 100).Select(_ => Guid.NewGuid()).ToArray();
            foreach (var id in ids)
                a.SendMessage(new MessageOnClick(id));
            await WaitUntilAsync(() => targetB.Applied.Count == 100);

            Assert.Equal(ids, targetB.Applied.Select(m => m.ID));
        }

        [Fact]
        public async Task Push_WhileEditsAreQueued_LandsAfterTheEntryInFlight_AndDropsTheRest()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);

            var gate = new StoreGate();
            store.BeforeAppend = gate.Hold;

            var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            foreach (var id in ids)
                peer.SendMessage(new MessageOnClick(id));
            await WaitUntilAsync(() => gate.IsReached);
            var writes = store.WriteSnapshotCalls;

            var push = peer.PushAsync();
            await SettleAsync();
            Assert.Equal(writes, store.WriteSnapshotCalls);

            store.BeforeAppend = null;
            gate.Release();
            await push;
            await WaitUntilAsync(() => peer.PendingMessages == 0, 15000, () => $"pending={peer.PendingMessages}");

            // The push trimmed to its own entry. The one in flight came before it, the rest never came.
            Assert.Single(inner.Entries);
            Assert.Equal(inner.Entries[0].Id, peer.LastSentId);
            Assert.True(peer.IsInSync);
        }
    }
}
