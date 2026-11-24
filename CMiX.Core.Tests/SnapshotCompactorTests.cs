using System.Collections.Concurrent;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // The Studio writes a new snapshot and trims the stream, so a late peer replays only the newest
    // entries. The tests use a short delay, because the real one is five seconds.
    public class SnapshotCompactorTests
    {
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan NoTimer = TimeSpan.FromSeconds(30);

        // One compaction delay and one settle tick. A compaction that the last change started has run after it.
        private static async Task CompactionSettleAsync()
        {
            await Task.Delay(Delay);
            await SettleAsync();
        }

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
        public async Task SendMessage_OfAValueChange_CompactsAfterTheDelayAndNotBefore()
        {
            var store = new InMemorySyncStore();
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => store.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushed = Current().StreamId;

            peer.SendMessage(ValueChange(0.25f));
            await Task.Delay(Delay / 2);
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
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => store.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushed = Current().StreamId;

            await store.AppendAsync(Envelope("other", AddItem()));

            await WaitUntilAsync(() => Current().StreamId > pushed, 5000, () => $"last={peer.LastAppliedId}");
            Assert.Equal(peer.LastAppliedId, Current().StreamId);
        }

        [Fact]
        public async Task TwoValueChanges_InOneWindow_CompactOnce()
        {
            var store = new WrappingSyncStore(new InMemorySyncStore());
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);
            var pushWrites = store.WriteSnapshotCalls;

            peer.SendMessage(ValueChange(0.25f));
            await Task.Delay(Delay / 4);
            peer.SendMessage(ValueChange(0.5f));

            await WaitUntilAsync(() => store.WriteSnapshotCalls == pushWrites + 1);
            await CompactionSettleAsync();
            Assert.Equal(pushWrites + 1, store.WriteSnapshotCalls);
        }

        [Fact]
        public async Task Trim_WithYoungEntries_KeepsEverything()
        {
            var store = new WrappingSyncStore(new InMemorySyncStore());
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.StartAndJoin(Options("Studio"));
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
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);
            store.TrimCalls.Clear();

            peer.SendMessage(ValueChange(0.25f));
            await WaitUntilAsync(() => store.TrimCalls.Count > 0);

            var retention = (long)Fast.Retention.TotalMilliseconds;
            var expected = new StreamPosition(peer.TailId.Milliseconds - retention, 0);
            Assert.Equal(expected, store.TrimCalls.ToArray().Last());
        }

        [Fact]
        public void TrimPosition_OfACompaction_IsTheLowerOfTheOwnPositionAndTheRetentionBound()
        {
            var retention = SyncTimings.Default.Retention;
            var bound = (long)retention.TotalMilliseconds;

            Assert.Equal(StreamPosition.Zero,
                SnapshotCompactor.TrimPosition(new StreamPosition(3, 0), new StreamPosition(3, 0), retention));
            Assert.Equal(new StreamPosition(1_000_000 - bound, 0),
                SnapshotCompactor.TrimPosition(new StreamPosition(1_000_000, 0), new StreamPosition(1_000_000, 0), retention));

            // A peer that lags more than the retention keeps the entries it did not apply yet.
            Assert.Equal(new StreamPosition(500_000, 0),
                SnapshotCompactor.TrimPosition(new StreamPosition(500_000, 0), new StreamPosition(1_000_000, 0), retention));
        }

        [Fact]
        public async Task LateJoiner_AfterCompaction_GetsTheStateWithoutTheOldEntries()
        {
            var store = new InMemorySyncStore();
            var targetA = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var a = Studio(targetA, store, Delay);
            a.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => a.IsJoined);

            Snapshot Current() => store.ReadSnapshotAsync().GetAwaiter().GetResult();

            for (var i = 0; i < 20; i++)
                a.SendMessage(ValueChange(i / 20f));

            await WaitUntilAsync(() => store.Entries.Count == 21 && a.LastAppliedId == store.Entries[20].Id);
            var tail = a.LastAppliedId;
            await WaitUntilAsync(() => Current().StreamId == tail, 5000, () => $"snapshot={Current().StreamId} tail={tail}");

            var targetC = new RecordingSyncTarget();
            using var c = CreatePeer(targetC, store);
            c.StartAndJoin(Options("Engine"));
            await WaitUntilAsync(() => c.IsJoined);

            Assert.Equal(1, targetC.SnapshotsApplied);
            Assert.Equal(ProjectStateHash.Compute(targetA.Model), ProjectStateHash.Compute(targetC.Model));
            Assert.Empty(targetC.Applied);
            Assert.Equal(a.LastAppliedId, c.LastAppliedId);
        }

        [Fact]
        public async Task SendMessage_WhileNotJoined_WritesNoSnapshot()
        {
            var inner = new InMemorySyncStore();
            await inner.ConnectAsync(default);
            var tail = await inner.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            var stored = ModelWithOneComposition();
            await inner.WriteSnapshotAsync(SnapshotOf(stored, tail));

            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.Start(Options("Studio"));
            await WaitUntilAsync(() => peer.IsConnected && peer.Status == "Not in sync", detail: () => $"status={peer.Status}");

            peer.SendMessage(ValueChange(0.25f));
            await WaitUntilAsync(() => peer.PendingMessages == 1);
            await CompactionSettleAsync();

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
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => inner.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushWrites = store.WriteSnapshotCalls;
            var pushed = Current().StreamId;

            var gate = new StoreGate();
            store.BeforeAppend = gate.Hold;

            peer.SendMessage(AddItem());
            await WaitUntilAsync(() => gate.IsReached);
            await CompactionSettleAsync();
            Assert.Equal(pushWrites, store.WriteSnapshotCalls);

            gate.Release();
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
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => inner.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushed = Current().StreamId;

            var gate = new StoreGate();
            store.BeforeAppend = gate.Hold;

            peer.SendMessage(AddItem());
            await WaitUntilAsync(() => gate.IsReached);
            await CompactionSettleAsync();
            gate.Release();

            await WaitUntilAsync(() => Current().StreamId > pushed, 5000, () => $"last={peer.LastAppliedId} sent={peer.LastSentId}");

            var lateTarget = new RecordingSyncTarget();
            using var late = CreatePeer(lateTarget, store, isWriter: false);
            late.StartAndJoin(Options("Engine"));
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
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => inner.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushWrites = store.WriteSnapshotCalls;
            var pushed = Current().StreamId;

            var gate = new StoreGate();
            store.BeforeAppend = gate.Hold;

            peer.SendMessage(ValueChange(0.25f));
            await WaitUntilAsync(() => gate.IsReached);
            await CompactionSettleAsync();
            Assert.Equal(pushWrites, store.WriteSnapshotCalls);

            gate.Release();
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
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);

            Snapshot Current() => inner.ReadSnapshotAsync().GetAwaiter().GetResult();
            var pushWrites = store.WriteSnapshotCalls;

            var gate = new StoreGate();
            store.BeforeAppend = gate.Hold;

            peer.SendMessage(AddItem());
            await Task.Delay(Delay);
            peer.SendMessage(AddItem());
            Assert.Equal(pushWrites, store.WriteSnapshotCalls);

            gate.Release();
            await WaitUntilAsync(() => inner.Entries.Count == 3);
            var last = inner.Entries[2].Id;

            await WaitUntilAsync(() => Current().StreamId >= last, 5000, () => $"snapshot={Current().StreamId} last={last}");
            await CompactionSettleAsync();

            // The second add can arrive before or after the first capture.
            Assert.InRange(store.WriteSnapshotCalls - pushWrites, 1, 2);
            Assert.True(Current().StreamId >= last, $"snapshot={Current().StreamId} last={last}");
        }

        [Fact]
        public async Task SendMessage_DuringACompaction_CompactsOnceMoreAfterIt()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, NoTimer);
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);
            var pushWrites = store.WriteSnapshotCalls;

            var gate = new StoreGate();
            store.BeforeWriteSnapshot = gate.Hold;

            await inner.AppendAsync(Envelope("other", AddItem()));
            await WaitUntilAsync(() => store.WriteSnapshotCalls == pushWrites + 1);

            await inner.AppendAsync(Envelope("other", AddItem()));
            await inner.AppendAsync(Envelope("other", AddItem()));
            await WaitUntilAsync(() => target.Applied.Count == 3);

            gate.Release();
            await WaitUntilAsync(() => store.WriteSnapshotCalls >= pushWrites + 2, 5000,
                () => $"writes={store.WriteSnapshotCalls}");

            // Gives a further compaction the time to appear.
            await CompactionSettleAsync();

            Assert.Equal(pushWrites + 2, store.WriteSnapshotCalls);
            Assert.Equal(peer.LastAppliedId, (await inner.ReadSnapshotAsync()).StreamId);
        }

        [Fact]
        public async Task Compaction_AfterAFailure_RunsAgainAfterTheReconnect()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, NoTimer);
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);
            var pushWrites = store.WriteSnapshotCalls;
            var failed = store.FailedCalls;

            // The store goes down while the compaction runs, so the trim after the write throws.
            var gate = new StoreGate();
            store.BeforeWriteSnapshot = gate.Hold;

            peer.SendMessage(AddItem());
            await WaitUntilAsync(() => store.WriteSnapshotCalls >= pushWrites + 1);

            store.BeforeWriteSnapshot = null;
            store.SetFail(true);
            gate.Release();
            await WaitUntilAsync(() => store.FailedCalls > failed, 5000, () => $"failed={store.FailedCalls}");

            // Gives the compactor the time to record the failure.
            await Task.Delay(100);
            store.SetFail(false);

            await WaitUntilAsync(() => store.WriteSnapshotCalls > pushWrites + 1, 2000,
                () => $"writes={store.WriteSnapshotCalls}");
            Assert.Equal(peer.LastAppliedId, (await inner.ReadSnapshotAsync()).StreamId);
        }

        // Stopped says that the store is quiet. A compaction that still writes would break that.
        [Fact]
        public async Task Stop_DuringACompaction_WaitsForIt()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, NoTimer);
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);
            var pushWrites = store.WriteSnapshotCalls;

            var gate = new StoreGate();
            store.BeforeWriteSnapshot = gate.Hold;

            peer.SendMessage(AddItem());
            await WaitUntilAsync(() => store.WriteSnapshotCalls == pushWrites + 1);

            var stopped = peer.Stopped;
            peer.Stop();
            await SettleAsync();
            Assert.False(stopped.IsCompleted);

            gate.Release();
            Assert.Same(stopped, await Task.WhenAny(stopped, Task.Delay(1000)));
        }

        // The compactor reads the count in the notification. A notification before the entry left
        // the queue shows one pending message, and the deferred capture waits for the next message.
        [Fact]
        public async Task PendingMessages_AfterAnAppend_IsNotifiedWhenTheEntryLeftTheQueue()
        {
            using var dispatcher = new SingleThreadDispatcher();
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, Delay);
            peer.SetDispatcher(dispatcher.Post);
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);

            var counts = new ConcurrentQueue<int>();
            peer.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SyncPeer.PendingMessages))
                    counts.Enqueue(peer.PendingMessages);
            };

            // The send notifies with one pending message. The loop notifies again with none.
            peer.SendMessage(ValueChange(0.25f));
            await WaitUntilAsync(() => counts.Count >= 2 && counts.ToArray().Last() == 0 && peer.LastAppliedId >= peer.LastSentId,
                5000, () => $"counts={string.Join(",", counts)} pending={peer.PendingMessages}");

            Assert.Equal(0, counts.ToArray().Last());
            Assert.Contains(1, counts);

            // The add asks for a capture while the entry is pending. The notification after the
            // entry releases it.
            var writes = store.WriteSnapshotCalls;
            peer.SendMessage(AddItem());

            await WaitUntilAsync(() => store.WriteSnapshotCalls > writes, (int)Delay.TotalMilliseconds * 4,
                () => $"writes={store.WriteSnapshotCalls} pending={peer.PendingMessages}");
        }

        [Fact]
        public async Task Compaction_OfALostSnapshot_WritesItAgainOnTheNextTick()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            using var peer = Studio(target, store, NoTimer);
            peer.StartAndJoin(Options("Studio"));
            await WaitUntilAsync(() => peer.IsJoined);
            var pushWrites = store.WriteSnapshotCalls;

            inner.ClearSnapshot();

            // The next heartbeat tick finds that the snapshot is gone.
            await WaitUntilAsync(() => store.WriteSnapshotCalls > pushWrites, 5000,
                () => $"writes={store.WriteSnapshotCalls}");

            var snapshot = await inner.ReadSnapshotAsync();
            Assert.NotNull(snapshot);
            Assert.Equal(peer.LastAppliedId, snapshot.StreamId);
            Assert.Equal(ProjectStateHash.Compute(target.Model), ProjectStateHash.Compute(ModelOf(snapshot)));
        }
    }
}
