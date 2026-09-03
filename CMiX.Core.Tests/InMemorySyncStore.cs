using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.Networking;

namespace CMiX.Core.Tests
{
    // A store that keeps everything in memory, for tests that need the protocol but not a server.
    // It is thread-safe, because later tests drive several peers on one store from several tasks.
    public class InMemorySyncStore : ISyncStore
    {
        private const int BlockingReadCount = 256;

        private readonly object _gate = new();
        private readonly List<StreamEntry> _entries = new();
        private readonly Dictionary<string, (Dictionary<string, string> Fields, DateTime Expires)> _peers = new();
        private readonly SemaphoreSlim _wakeSignal = new(0, 1);

        private Snapshot _snapshot;
        private long _ms;
        private bool _isConnected;

        // startMilliseconds gives the entry IDs realistic values, for tests of the retention rule.
        public InMemorySyncStore(long startMilliseconds = 0) => _ms = startMilliseconds;

        public bool IsConnected => _isConnected;

        public event Action<bool> ConnectionChanged;

        public IReadOnlyList<StreamEntry> Entries
        {
            get
            {
                lock (_gate)
                    return _entries.ToList();
            }
        }

        public Task ConnectAsync(CancellationToken ct)
        {
            _isConnected = true;
            ConnectionChanged?.Invoke(true);
            return Task.CompletedTask;
        }

        public void SimulateDisconnect()
        {
            _isConnected = false;
            ConnectionChanged?.Invoke(false);
        }

        public void SimulateReconnect()
        {
            _isConnected = true;
            ConnectionChanged?.Invoke(true);
        }

        private void RequireConnection()
        {
            if (!_isConnected)
                throw new InvalidOperationException("The in-memory store is not connected.");
        }

        public Task<Snapshot> ReadSnapshotAsync()
        {
            RequireConnection();
            lock (_gate)
                return Task.FromResult(_snapshot);
        }

        public Task<StreamPosition> ReadSnapshotIdAsync()
        {
            RequireConnection();
            lock (_gate)
                return Task.FromResult(_snapshot?.StreamId ?? StreamPosition.Zero);
        }

        public Task WriteSnapshotAsync(Snapshot snapshot)
        {
            RequireConnection();
            lock (_gate)
                _snapshot = snapshot;

            return Task.CompletedTask;
        }

        // Drops the snapshot, like a server that restarts without its data.
        public void ClearSnapshot()
        {
            lock (_gate)
                _snapshot = null;
        }

        public Task<StreamPosition> AppendAsync(byte[] envelope)
        {
            RequireConnection();

            StreamPosition id;
            lock (_gate)
            {
                id = new StreamPosition(++_ms, 0);
                _entries.Add(new StreamEntry(id, envelope));
            }

            Wake();
            return Task.FromResult(id);
        }

        private void Wake()
        {
            if (_wakeSignal.CurrentCount > 0)
                return;

            try
            {
                _wakeSignal.Release();
            }
            catch (SemaphoreFullException)
            {
            }
        }

        public Task<IReadOnlyList<StreamEntry>> ReadRangeAsync(StreamPosition afterExclusive, int count)
        {
            RequireConnection();

            lock (_gate)
            {
                IReadOnlyList<StreamEntry> result = _entries
                    .Where(entry => entry.Id > afterExclusive)
                    .Take(count)
                    .ToList();

                return Task.FromResult(result);
            }
        }

        public async Task<IReadOnlyList<StreamEntry>> ReadBlockingAsync(StreamPosition afterExclusive, TimeSpan timeout, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var entries = await ReadRangeAsync(afterExclusive, BlockingReadCount);
            if (entries.Count > 0)
                return entries;

            await _wakeSignal.WaitAsync(timeout, ct);

            return await ReadRangeAsync(afterExclusive, BlockingReadCount);
        }

        public Task<StreamPosition> ReadTailAsync()
        {
            RequireConnection();

            lock (_gate)
                return Task.FromResult(_entries.Count == 0 ? StreamPosition.Zero : _entries[_entries.Count - 1].Id);
        }

        public Task TrimAsync(StreamPosition minId)
        {
            RequireConnection();

            lock (_gate)
                _entries.RemoveAll(entry => entry.Id < minId);

            return Task.CompletedTask;
        }

        public Task HeartbeatAsync(string peerId, IReadOnlyDictionary<string, string> fields, TimeSpan ttl)
        {
            RequireConnection();

            lock (_gate)
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                    peer = (new Dictionary<string, string>(), DateTime.UtcNow);

                foreach (var pair in fields)
                    peer.Fields[pair.Key] = pair.Value;

                _peers[peerId] = (peer.Fields, DateTime.UtcNow + ttl);
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<PeerInfo>> ListPeersAsync()
        {
            RequireConnection();

            lock (_gate)
            {
                var now = DateTime.UtcNow;
                var expired = _peers.Where(pair => pair.Value.Expires <= now).Select(pair => pair.Key).ToList();
                foreach (var peerId in expired)
                    _peers.Remove(peerId);

                IReadOnlyList<PeerInfo> result = _peers
                    .Select(pair => ToPeerInfo(pair.Key, pair.Value.Fields))
                    .ToList();

                return Task.FromResult(result);
            }
        }

        private static PeerInfo ToPeerInfo(string peerId, Dictionary<string, string> fields)
        {
            var lastAppliedId = StreamPosition.Zero;
            if (fields.TryGetValue("lastAppliedId", out var raw) && !StreamPosition.TryParse(raw, out lastAppliedId))
                lastAppliedId = StreamPosition.Zero;

            return new PeerInfo(peerId, Field(fields, "name"), Field(fields, "role"), Field(fields, "host"), lastAppliedId);
        }

        private static string Field(Dictionary<string, string> fields, string name)
            => fields.TryGetValue(name, out var value) && value != null ? value : string.Empty;

        public ValueTask DisposeAsync()
        {
            _isConnected = false;
            return default;
        }
    }
}
