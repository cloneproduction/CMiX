using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using VL.Serialization.MessagePack;

namespace CMiX.Core.Tests
{
    // A sync target that records what the peer applied, for tests that check the protocol without
    // real controls. Two real projects in one process would share the global messenger.
    public class RecordingSyncTarget : ISyncTarget
    {
        private readonly object _gate = new();
        private readonly List<IMessage> _applied = new();

        public ProjectModel Model { get; set; } = new();
        public int SnapshotsApplied { get; private set; }

        public IReadOnlyList<IMessage> Applied
        {
            get
            {
                lock (_gate)
                    return _applied.ToArray();
            }
        }

        public ProjectModel Capture() => Model;

        public void ApplySnapshot(ProjectModel model)
        {
            lock (_gate)
            {
                Model = model;
                SnapshotsApplied++;
            }
        }

        public void Apply(IMessage message)
        {
            lock (_gate)
            {
                if (message is MessageProjectSnapshot snapshot)
                {
                    Model = snapshot.Model;
                    SnapshotsApplied++;
                }
                _applied.Add(message);
            }
        }
    }

    // A store whose ConnectAsync never completes, for tests of the non-blocking start.
    public class HangingSyncStore : ISyncStore
    {
        public bool IsConnected => false;
        public event Action<bool> ConnectionChanged { add { } remove { } }

        public Task ConnectAsync(CancellationToken ct) => Task.Delay(Timeout.Infinite, ct);
        public Task<Snapshot> ReadSnapshotAsync() => throw new InvalidOperationException();
        public Task<StreamPosition> ReadSnapshotIdAsync() => throw new InvalidOperationException();
        public Task WriteSnapshotAsync(Snapshot snapshot) => throw new InvalidOperationException();
        public Task<StreamPosition> AppendAsync(byte[] envelope) => throw new InvalidOperationException();
        public Task<IReadOnlyList<StreamEntry>> ReadRangeAsync(StreamPosition afterExclusive, int count) => throw new InvalidOperationException();
        public Task<IReadOnlyList<StreamEntry>> ReadBlockingAsync(StreamPosition afterExclusive, TimeSpan timeout, CancellationToken ct) => throw new InvalidOperationException();
        public Task<StreamPosition> ReadTailAsync() => throw new InvalidOperationException();
        public Task TrimAsync(StreamPosition minId) => throw new InvalidOperationException();
        public Task HeartbeatAsync(string peerId, IReadOnlyDictionary<string, string> fields, TimeSpan ttl) => throw new InvalidOperationException();
        public Task<IReadOnlyList<PeerInfo>> ListPeersAsync() => throw new InvalidOperationException();
        public ValueTask DisposeAsync() => default;
    }

    // Wraps a store. Records the thread of every data call, and can fail every data call on demand.
    public class WrappingSyncStore : ISyncStore
    {
        private readonly ISyncStore _inner;

        public WrappingSyncStore(ISyncStore inner)
        {
            _inner = inner;
            _inner.ConnectionChanged += value => ConnectionChanged?.Invoke(value);
        }

        private int _writeSnapshotCalls;

        public ConcurrentBag<int> CallThreads { get; } = new();
        public bool Fail { get; set; }
        public int FailedCalls { get; private set; }

        public int WriteSnapshotCalls => Volatile.Read(ref _writeSnapshotCalls);
        public ConcurrentQueue<StreamPosition> TrimCalls { get; } = new();

        // Lets a test hold a snapshot write open, to check what happens during a compaction.
        public Func<Task> BeforeWriteSnapshot { get; set; }

        public bool IsConnected => !Fail && _inner.IsConnected;
        public event Action<bool> ConnectionChanged;

        public void SetFail(bool fail)
        {
            Fail = fail;
            ConnectionChanged?.Invoke(!fail);
        }

        private void Enter()
        {
            CallThreads.Add(Environment.CurrentManagedThreadId);
            if (!Fail) return;

            FailedCalls++;
            throw new InvalidOperationException("The store is failing.");
        }

        public Task ConnectAsync(CancellationToken ct) => _inner.ConnectAsync(ct);
        public Task<Snapshot> ReadSnapshotAsync() { Enter(); return _inner.ReadSnapshotAsync(); }
        public Task<StreamPosition> ReadSnapshotIdAsync() { Enter(); return _inner.ReadSnapshotIdAsync(); }
        public async Task WriteSnapshotAsync(Snapshot snapshot)
        {
            Enter();
            Interlocked.Increment(ref _writeSnapshotCalls);
            var gate = BeforeWriteSnapshot;
            if (gate != null)
                await gate();

            await _inner.WriteSnapshotAsync(snapshot);
        }
        public Task<StreamPosition> AppendAsync(byte[] envelope) { Enter(); return _inner.AppendAsync(envelope); }
        public Task<IReadOnlyList<StreamEntry>> ReadRangeAsync(StreamPosition afterExclusive, int count) { Enter(); return _inner.ReadRangeAsync(afterExclusive, count); }
        public Task<IReadOnlyList<StreamEntry>> ReadBlockingAsync(StreamPosition afterExclusive, TimeSpan timeout, CancellationToken ct) { Enter(); return _inner.ReadBlockingAsync(afterExclusive, timeout, ct); }
        public Task<StreamPosition> ReadTailAsync() { Enter(); return _inner.ReadTailAsync(); }
        public Task TrimAsync(StreamPosition minId) { Enter(); TrimCalls.Enqueue(minId); return _inner.TrimAsync(minId); }
        public Task HeartbeatAsync(string peerId, IReadOnlyDictionary<string, string> fields, TimeSpan ttl) { Enter(); return _inner.HeartbeatAsync(peerId, fields, ttl); }
        public Task<IReadOnlyList<PeerInfo>> ListPeersAsync() { Enter(); return _inner.ListPeersAsync(); }
        public ValueTask DisposeAsync() => _inner.DisposeAsync();
    }

    // Runs dispatched actions on one dedicated thread, like a UI thread.
    public sealed class SingleThreadDispatcher : IDisposable
    {
        private readonly BlockingCollection<Action> _queue = new();
        private readonly Thread _thread;

        public SingleThreadDispatcher()
        {
            _thread = new Thread(Run) { IsBackground = true };
            _thread.Start();
        }

        public int ThreadId => _thread.ManagedThreadId;

        public void Post(Action action) => _queue.Add(action);

        private void Run()
        {
            foreach (var action in _queue.GetConsumingEnumerable())
                action();
        }

        public void Dispose() => _queue.CompleteAdding();
    }

    public static class SyncTestHelpers
    {
        public static async Task WaitUntilAsync(Func<bool> condition, int timeoutMs = 5000, Func<string> detail = null)
        {
            var watch = Stopwatch.StartNew();
            while (!condition())
            {
                if (watch.ElapsedMilliseconds > timeoutMs)
                    throw new TimeoutException("The condition did not become true in time. " + detail?.Invoke());

                await Task.Delay(10);
            }
        }

        public static byte[] Envelope(string sender, IMessage message) =>
            MessagePackSerialization.Serialize(new MessageEnvelope
            {
                SenderID = sender,
                MessageID = Guid.NewGuid(),
                Payload = message
            });

        public static SyncPeer CreatePeer(ISyncTarget target, ISyncStore store, ControlMessenger messenger = null)
            => new SyncPeer(target, messenger ?? new ControlMessenger(), _ => store);

        public static SyncOptions Options(string name) => SyncOptions.Defaults with { PeerName = name, Role = "test" };
    }
}
