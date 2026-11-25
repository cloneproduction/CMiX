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
    // The follower applies the entries of other peers in order, once, and in batches. The store is in memory. Peers apply inline, because no dispatcher is set.
    public class SyncPeerFollowerTests
    {
        [Fact]
        public async Task Follower_OfOwnEntries_AdvancesThePositionWithoutApply()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);
            var pushId = peer.LastAppliedId;

            peer.SendMessage(new MessageOnClick(Guid.NewGuid()));
            await WaitUntilAsync(() => peer.LastAppliedId > pushId);

            Assert.Empty(target.Applied);
            Assert.Equal(0, peer.AppliedMessages);
            Assert.Equal(2, peer.SentMessages);
            Assert.True(peer.IsInSync);
        }

        [Fact]
        public async Task ValueChange_FromAnotherPeer_ReachesControlInRealProject()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var store = new InMemorySyncStore();
            var target = new ProjectSyncTarget(project, messenger, provider.GetRequiredService<UndoManager>(), provider.GetRequiredService<ControlRegistry>());
            using var peer = CreatePeer(target, store, messenger);
            messenger.Register(peer);
            peer.StartAndJoin(Options("B"));
            await WaitUntilAsync(() => peer.IsJoined);

            project.CompositionManager.AddItem(typeof(Composition));
            var opacity = ((Composition)project.CompositionManager.ManagerData.Items[0]).Compositing.Opacity;
            await WaitUntilAsync(() => peer.SentMessages >= 2);

            var change = new MessageValueChanged(opacity.ID, new GenericValueModel<float> { ID = opacity.ID, Value = 0.25f });
            await store.AppendAsync(Envelope("other", change));
            await WaitUntilAsync(() => Math.Abs(opacity.Value - 0.25f) < 0.0001f);

            Assert.Equal(1, peer.AppliedMessages);
        }

        // A store can hold an entry that no reader can deserialize. It must not stop the follower.
        [Fact]
        public async Task ACorruptEntry_IsSkipped_AndTheNextEntryIsApplied()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.IsJoined);

            // MessagePack never uses 0xC1, so the read of this entry fails.
            await store.AppendAsync(new byte[] { 0xC1, 0x2A, 0x7F, 0xFF });
            var click = Guid.NewGuid();
            var second = await store.AppendAsync(Envelope("other", new MessageOnClick(click)));

            await WaitUntilAsync(() => peer.LastAppliedId == second, detail: () => $"last={peer.LastAppliedId}");
            await SettleAsync();

            Assert.Equal(click, Assert.Single(target.Applied).ID);
            Assert.Equal(second, peer.LastAppliedId);
        }

        [Fact]
        public async Task Follower_OfARemoveThenAnAdd_AppliesThemInOrder()
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

            var managerId = Guid.NewGuid();
            a.SendMessage(new MessageRemoveItem(managerId, Guid.NewGuid(), -1));
            a.SendMessage(new MessageAddItem(managerId, new GenericValueModel<float> { ID = Guid.NewGuid(), Value = 1f }, 0));
            await WaitUntilAsync(() => targetB.Applied.Count == 2);

            Assert.IsType<MessageRemoveItem>(targetB.Applied[0]);
            Assert.IsType<MessageAddItem>(targetB.Applied[1]);
        }

        // A retry of an append that timed out puts the same envelope into the stream a second time.
        [Fact]
        public async Task Follower_OfTheSameEntryTwice_AppliesItOnce()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            var envelope = Envelope("other", new MessageOnClick(Guid.NewGuid()));
            await store.AppendAsync(envelope);
            var second = await store.AppendAsync(envelope);

            await WaitUntilAsync(() => peer.LastAppliedId == second, detail: () => $"last={peer.LastAppliedId}");
            await SettleAsync();

            Assert.Single(target.Applied);
            Assert.Equal(1, peer.AppliedMessages);
            Assert.Equal(second, peer.LastAppliedId);
        }

        // Two clicks on the same control are two messages with one payload ID. Only the message ID
        // tells a repeat from a second edit.
        [Fact]
        public async Task Follower_OfTwoEntriesWithOnePayloadId_AppliesBoth()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            var id = Guid.NewGuid();
            await store.AppendAsync(Envelope("other", new MessageOnClick(id)));
            var second = await store.AppendAsync(Envelope("other", new MessageOnClick(id)));

            await WaitUntilAsync(() => target.Applied.Count == 2, detail: () => $"applied={target.Applied.Count}");
            Assert.Equal(second, peer.LastAppliedId);
        }

        // The engine drains its queue once per frame. One action per entry would apply one entry
        // per frame, so a replay hands every read batch to one action.
        [Fact]
        public async Task Replay_OfManyEntries_DispatchesOneActionPerReadBatch()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var ids = new List<Guid>();
            for (var i = 0; i < 300; i++)
            {
                var click = new MessageOnClick(Guid.NewGuid());
                ids.Add(click.ID);
                await store.AppendAsync(Envelope("other", click));
            }
            var tail = await store.ReadTailAsync();

            var queue = new MainloopQueue();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.SetDispatcher(queue.Post);
            peer.StartAndJoin(Options("Engine"));

            var applyingDrains = await DrainUntilAsync(queue, () => peer.LastAppliedId == tail, () => target.Applied.Count);

            Assert.Equal(ids, target.Applied.Select(m => m.ID));
            Assert.Equal(2, applyingDrains);
        }

        [Fact]
        public async Task Replay_OfManyEntries_UsesTheReadBatchOfTheTimings()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var ids = new List<Guid>();
            for (var i = 0; i < 300; i++)
            {
                var click = new MessageOnClick(Guid.NewGuid());
                ids.Add(click.ID);
                await store.AppendAsync(Envelope("other", click));
            }
            var tail = await store.ReadTailAsync();

            var queue = new MainloopQueue();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.ReadBatch = 100;
            peer.SetDispatcher(queue.Post);
            peer.StartAndJoin(Options("Engine"));

            var applyingDrains = await DrainUntilAsync(queue, () => peer.LastAppliedId == tail, () => target.Applied.Count);

            Assert.Equal(ids, target.Applied.Select(m => m.ID));
            Assert.Equal(3, applyingDrains);
        }

        [Fact]
        public async Task Follower_OfABatch_AppliesItInOneAction()
        {
            var store = new InMemorySyncStore();
            var queue = new MainloopQueue();
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.SetDispatcher(queue.Post);
            peer.StartAndJoin(Options("Engine"));
            await DrainUntilAsync(queue, () => peer.IsJoined, () => 0);

            var ids = new List<Guid>();
            for (var i = 0; i < 100; i++)
            {
                var click = new MessageOnClick(Guid.NewGuid());
                ids.Add(click.ID);
                await store.AppendAsync(Envelope("other", click));
            }
            var tail = await store.ReadTailAsync();

            var applyingDrains = await DrainUntilAsync(queue, () => peer.LastAppliedId == tail, () => target.Applied.Count);

            Assert.Equal(ids, target.Applied.Select(m => m.ID));
            Assert.InRange(applyingDrains, 1, 3);
        }

        [Fact]
        public async Task Follower_OfADuplicateInsideABatch_AppliesItOnce()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var bytes = Envelope("other", new MessageOnClick(Guid.NewGuid()));
            await store.AppendAsync(bytes);
            var second = await store.AppendAsync(bytes);

            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.LastAppliedId == second);

            Assert.Single(target.Applied);
            Assert.Equal(1, peer.AppliedMessages);
        }

        [Fact]
        public async Task Follower_OfACorruptEntryInsideABatch_AdvancesThePosition()
        {
            var store = new InMemorySyncStore();
            await store.ConnectAsync(default);
            var first = new MessageOnClick(Guid.NewGuid());
            var last = new MessageOnClick(Guid.NewGuid());
            await store.AppendAsync(Envelope("other", first));
            // 0xC1 is the one byte MessagePack never uses, so the deserialize fails.
            await store.AppendAsync(new byte[] { 0xC1 });
            var lastId = await store.AppendAsync(Envelope("other", last));

            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store, isWriter: false);
            peer.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => peer.LastAppliedId == lastId);

            Assert.Equal(new[] { first.ID, last.ID }, target.Applied.Select(m => m.ID));
            Assert.Equal(string.Empty, peer.ErrorMessage);
        }

        // Drains every 10 ms until the condition holds. Returns the number of drains after which
        // the progress value went up.
        private static async Task<int> DrainUntilAsync(MainloopQueue queue, Func<bool> condition, Func<int> progress)
        {
            var watch = Stopwatch.StartNew();
            var drains = 0;
            while (!condition())
            {
                if (watch.ElapsedMilliseconds > 5000)
                    throw new TimeoutException("The condition did not become true in time.");

                var before = progress();
                queue.Drain();
                if (progress() > before) drains++;
                await Task.Delay(10);
            }

            return drains;
        }
    }
}
