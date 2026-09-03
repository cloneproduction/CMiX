using System;
using System.Linq;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Microsoft.Extensions.DependencyInjection;
using VL.Serialization.MessagePack;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // The Studio writes a new snapshot and trims the stream, so a late peer replays only the newest
    // entries. The tests use a short delay, because the real one is five seconds.
    public class SnapshotCompactorTests
    {
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(400);
        private static readonly TimeSpan NoTimer = TimeSpan.FromSeconds(30);

        private static ProjectModel ModelWithOneComposition()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return (ProjectModel)project.ToModel();
        }

        private static ProjectModel ModelOf(Snapshot snapshot) =>
            MessagePackSerialization.Deserialize<ProjectModel>(new ReadOnlyMemory<byte>(snapshot.Model));

        private static MessageValueChanged ValueChange(float value)
        {
            var id = Guid.NewGuid();
            return new MessageValueChanged(id, new GenericValueModel<float> { ID = id, Value = value });
        }

        private static MessageAddItem AddItem() =>
            new MessageAddItem(Guid.NewGuid(), new GenericValueModel<float> { ID = Guid.NewGuid(), Value = 1f }, 0);

        private static SyncPeer Studio(ISyncTarget target, ISyncStore store, TimeSpan delay)
        {
            var peer = CreatePeer(target, store);
            peer.CompactionEnabled = true;
            peer.CompactionDelay = delay;
            return peer;
        }

        [Fact]
        public async Task ValueChange_CompactsAfterTheDelayAndNotBefore()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => store.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushed = Current().StreamId;

            peer.SendMessage(ValueChange(0.25f));
            await Task.Delay(Delay / 4);
            Assert.Equal(pushed, Current().StreamId);

            await WaitUntilAsync(() => Current().StreamId > pushed, 5000, () => $"last={peer.LastAppliedId}");
            var snapshot = Current();
            Assert.Equal(peer.LastAppliedId, snapshot.StreamId);
            Assert.Equal(ProjectStateHash.Compute(target.Model), ProjectStateHash.Compute(ModelOf(snapshot)));
        }

        [Fact]
        public async Task ManagerMessage_FromAnotherPeer_CompactsAtOnce()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, NoTimer);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => store.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushed = Current().StreamId;

            await store.AppendAsync(Envelope("other", AddItem()));

            await WaitUntilAsync(() => Current().StreamId > pushed, 500, () => $"last={peer.LastAppliedId}");
            Assert.Equal(peer.LastAppliedId, Current().StreamId);
        }

        [Fact]
        public async Task TwoValueChanges_InOneWindow_CompactOnce()
        {
            var store = new WrappingSyncStore(new InMemorySyncStore());
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);
            var pushWrites = store.WriteSnapshotCalls;

            peer.SendMessage(ValueChange(0.25f));
            await Task.Delay(Delay / 4);
            peer.SendMessage(ValueChange(0.5f));

            await WaitUntilAsync(() => store.WriteSnapshotCalls == pushWrites + 1);
            await Task.Delay(Delay * 3);
            Assert.Equal(pushWrites + 1, store.WriteSnapshotCalls);
        }

        [Fact]
        public async Task Trim_WithYoungEntries_KeepsEverything()
        {
            var store = new WrappingSyncStore(new InMemorySyncStore());
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);
            store.TrimCalls.Clear();

            peer.SendMessage(ValueChange(0.25f));
            await WaitUntilAsync(() => store.TrimCalls.Count > 0);

            Assert.Equal(StreamPosition.Zero, store.TrimCalls.ToArray().Last());
        }

        [Fact]
        public async Task Trim_WithOldEntries_KeepsTheRetentionWindow()
        {
            var store = new WrappingSyncStore(new InMemorySyncStore(1_000_000));
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);
            store.TrimCalls.Clear();

            peer.SendMessage(ValueChange(0.25f));
            await WaitUntilAsync(() => store.TrimCalls.Count > 0);

            var retention = (long)SyncTimings.Retention.TotalMilliseconds;
            var expected = new StreamPosition(peer.TailId.Milliseconds - retention, 0);
            Assert.Equal(expected, store.TrimCalls.ToArray().Last());
        }

        [Fact]
        public void TrimPosition_TakesTheLowerOfTheOwnPositionAndTheRetentionBound()
        {
            var retention = (long)SyncTimings.Retention.TotalMilliseconds;

            Assert.Equal(StreamPosition.Zero, SnapshotCompactor.TrimPosition(new StreamPosition(3, 0), new StreamPosition(3, 0)));
            Assert.Equal(new StreamPosition(1_000_000 - retention, 0),
                SnapshotCompactor.TrimPosition(new StreamPosition(1_000_000, 0), new StreamPosition(1_000_000, 0)));

            // A peer that lags more than the retention keeps the entries it did not apply yet.
            Assert.Equal(new StreamPosition(500_000, 0),
                SnapshotCompactor.TrimPosition(new StreamPosition(500_000, 0), new StreamPosition(1_000_000, 0)));
        }

        [Fact]
        public async Task LateJoiner_AfterCompaction_GetsTheStateWithoutTheOldEntries()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var a = Studio(targetA, store, Delay);
            a.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => a.IsJoined);

            Snapshot Current() => store.ReadSnapshotAsync().GetAwaiter().GetResult();

            for (var i = 0; i < 20; i++)
                a.SendMessage(ValueChange(i / 20f));

            await WaitUntilAsync(() => store.Entries.Count == 21 && a.LastAppliedId == store.Entries[20].Id);
            var tail = a.LastAppliedId;
            await WaitUntilAsync(() => Current().StreamId == tail, 5000, () => $"snapshot={Current().StreamId} tail={tail}");

            var targetC = new RecordingSyncTarget();
            using var c = CreatePeer(targetC, store);
            c.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => c.IsJoined);

            Assert.Equal(1, targetC.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(targetA.Model), ProjectStateHash.Compute(targetC.Model));
            Assert.Empty(targetC.Applied);
            Assert.Equal(a.LastAppliedId, c.LastAppliedId);
        }

        [Fact]
        public async Task NotJoined_WritesNothing()
        {
            var inner = new InMemorySyncStore();
            await inner.ConnectAsync(default);
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var stored = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(new Snapshot(MessagePackSerialization.Serialize(stored), tail, "other", DateTime.UtcNow));

            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsConnected && peer.Status == "Not in sync", detail: () => $"status={peer.Status}");

            peer.SendMessage(ValueChange(0.25f));
            await Task.Delay(Delay * 4);

            Assert.Equal(0, store.WriteSnapshotCalls);
            var snapshot = await inner.ReadSnapshotAsync();
            Assert.Equal(tail, snapshot.StreamId);
            Assert.Equal(ProjectStateHash.Compute(stored), ProjectStateHash.Compute(ModelOf(snapshot)));
        }

        [Fact]
        public async Task Add_WhileTheEntryIsPending_CapturesOnlyAfterTheAppend()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => inner.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushWrites = store.WriteSnapshotCalls;
            var pushed = Current().StreamId;

            var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            store.BeforeAppend = () => gate.Task;

            peer.SendMessage(AddItem());
            await Task.Delay(Delay * 3);
            Assert.Equal(pushWrites, store.WriteSnapshotCalls);

            gate.SetResult(true);
            await WaitUntilAsync(() => inner.Entries.Count == 2);
            var added = inner.Entries[1].Id;

            await WaitUntilAsync(() => Current().StreamId > pushed, 5000, () => $"last={peer.LastAppliedId} sent={peer.LastSentId}");
            var snapshot = Current();
            Assert.True(snapshot.StreamId >= added, $"snapshot={snapshot.StreamId} added={added}");
            Assert.True(snapshot.StreamId >= peer.LastSentId, $"snapshot={snapshot.StreamId} sent={peer.LastSentId}");
        }

        [Fact]
        public async Task LateJoiner_AfterADeferredCapture_DoesNotReplayTheAdd()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => inner.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushed = Current().StreamId;

            var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            store.BeforeAppend = () => gate.Task;

            peer.SendMessage(AddItem());
            await Task.Delay(Delay * 3);
            gate.SetResult(true);

            await WaitUntilAsync(() => Current().StreamId > pushed, 5000, () => $"last={peer.LastAppliedId} sent={peer.LastSentId}");

            var lateTarget = new RecordingSyncTarget();
            using var late = CreatePeer(lateTarget, store, isWriter: false);
            late.Start(Options("Engine"), autoJoin: true);
            await WaitUntilAsync(() => late.IsJoined);
            await WaitUntilAsync(() => late.LastAppliedId == peer.LastAppliedId, 5000,
                () => $"late={late.LastAppliedId} studio={peer.LastAppliedId}");

            Assert.Equal(1, lateTarget.SnapshotsApplied);
            Assert.DoesNotContain(lateTarget.Applied, message => message is MessageAddItem);
        }

        [Fact]
        public async Task ValueChange_WhileTheEntryIsPending_CapturesOnlyAfterTheAppend()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => inner.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushWrites = store.WriteSnapshotCalls;
            var pushed = Current().StreamId;

            var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            store.BeforeAppend = () => gate.Task;

            peer.SendMessage(ValueChange(0.25f));
            await Task.Delay(Delay * 3);
            Assert.Equal(pushWrites, store.WriteSnapshotCalls);

            gate.SetResult(true);
            await WaitUntilAsync(() => inner.Entries.Count == 2);
            var changed = inner.Entries[1].Id;

            await WaitUntilAsync(() => Current().StreamId > pushed, 5000, () => $"last={peer.LastAppliedId} sent={peer.LastSentId}");
            var snapshot = Current();
            Assert.True(snapshot.StreamId >= changed, $"snapshot={snapshot.StreamId} changed={changed}");
            Assert.True(snapshot.StreamId >= peer.LastSentId, $"snapshot={snapshot.StreamId} sent={peer.LastSentId}");
        }

        [Fact]
        public async Task SecondAdd_DuringADeferredCapture_EndsAtTheLastEntry()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => inner.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushWrites = store.WriteSnapshotCalls;

            var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            store.BeforeAppend = () => gate.Task;

            peer.SendMessage(AddItem());
            await Task.Delay(Delay);
            peer.SendMessage(AddItem());
            Assert.Equal(pushWrites, store.WriteSnapshotCalls);

            gate.SetResult(true);
            await WaitUntilAsync(() => inner.Entries.Count == 3);
            var last = inner.Entries[2].Id;

            await WaitUntilAsync(() => Current().StreamId >= last, 5000, () => $"snapshot={Current().StreamId} last={last}");
            await Task.Delay(Delay * 3);

            // The second add can arrive before or after the first capture.
            Assert.InRange(store.WriteSnapshotCalls - pushWrites, 1, 2);
            Assert.True(Current().StreamId >= last, $"snapshot={Current().StreamId} last={last}");
        }

        [Fact]
        public async Task MessageDuringACompaction_CompactsOnceMoreAfterIt()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, NoTimer);
            peer.Start(Options("Studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined);
            var pushWrites = store.WriteSnapshotCalls;

            var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            store.BeforeWriteSnapshot = () => gate.Task;

            await inner.AppendAsync(Envelope("other", AddItem()));
            await WaitUntilAsync(() => store.WriteSnapshotCalls == pushWrites + 1);

            await inner.AppendAsync(Envelope("other", AddItem()));
            await inner.AppendAsync(Envelope("other", AddItem()));
            await WaitUntilAsync(() => target.Applied.Count == 3);

            gate.SetResult(true);
            await WaitUntilAsync(() => store.WriteSnapshotCalls == pushWrites + 2);
            await Task.Delay(500);

            Assert.Equal(pushWrites + 2, store.WriteSnapshotCalls);
            Assert.Equal(peer.LastAppliedId, (await inner.ReadSnapshotAsync()).StreamId);
        }
    }
}
