// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Globalization;
using StackExchange.Redis;
using RedisStreamEntry = StackExchange.Redis.StreamEntry;

namespace CMiX.Core.Networking
{
    // The Redis implementation of the store. Keys live under one configurable prefix, so several
    // projects can share one server.
    public sealed class RedisSyncStore : ISyncStore
    {
        private const string SnapshotModelField = "model";
        private const string SnapshotStreamIdField = "streamId";
        private const string SnapshotWrittenByField = "writtenBy";
        private const string SnapshotWrittenAtField = "writtenAt";
        private const string EnvelopeField = "env";

        private readonly SyncOptions _options;
        private readonly ConfigurationOptions _configuration;
        private readonly string _snapshotKey;
        private readonly string _streamKey;
        private readonly string _tailKey;
        private readonly string _peerKeyPrefix;
        private readonly RedisChannel _wakeChannel;
        private readonly SemaphoreSlim _wakeSignal = new(0, 1);

        private ConnectionMultiplexer _multiplexer;
        private ISubscriber _subscriber;
        private volatile string _lastError = string.Empty;
        private int _subscribing;

        public RedisSyncStore(SyncOptions options)
        {
            _options = options.WithFallbacks();

            _configuration = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = 3000,
                ConnectRetry = 3,
                SyncTimeout = 2000,
                DefaultDatabase = _options.Database,
                User = _options.User,
                ReconnectRetryPolicy = new ExponentialRetry(1000)
            };
            _configuration.EndPoints.Add(_options.Ip, _options.Port);

            if (!string.IsNullOrEmpty(_options.Password))
                _configuration.Password = _options.Password;

            if (!string.IsNullOrEmpty(_options.PeerName))
                _configuration.ClientName = _options.PeerName;

            _snapshotKey = _options.KeyPrefix + ":snapshot";
            _streamKey = _options.KeyPrefix + ":updates";
            _tailKey = _options.KeyPrefix + ":tail";
            _peerKeyPrefix = _options.KeyPrefix + ":peers:";
            _wakeChannel = RedisChannel.Literal(_options.KeyPrefix + ":wake");
        }

        public bool IsConnected => _multiplexer != null && _multiplexer.IsConnected;

        public string LastError => _lastError;

        public event Action<ISyncStore, bool> ConnectionChanged;

        public async Task ConnectAsync(CancellationToken ct)
        {
            if (_multiplexer != null)
            {
                await EnsureSubscribedAsync().ConfigureAwait(false);
                return;
            }

            // The connect runs on a background task, so no caller thread waits for the server.
            // AbortOnConnectFail is false, so this gives a multiplexer even when the server is down.
            // The multiplexer then reconnects on its own.
            // The token stays out of Task.Run. A cancelled token must not drop the multiplexer that
            // the connect made, because nobody would close it.
            var multiplexer = await Task.Run(async () =>
            {
                try
                {
                    return await ConnectionMultiplexer.ConnectAsync(_configuration).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _lastError = ex.Message;
                    return null;
                }
            }).ConfigureAwait(false);

            if (multiplexer == null)
                return;

            // A cancel while the connect ran. The multiplexer is not stored, so it is closed here.
            if (ct.IsCancellationRequested)
            {
                await CloseAndDisposeAsync(multiplexer).ConfigureAwait(false);
                return;
            }

            multiplexer.ConnectionFailed += OnConnectionFailed;
            multiplexer.ConnectionRestored += OnConnectionRestored;
            _multiplexer = multiplexer;

            // The handler was not attached while the first connect ran. Without this the user sees
            // no reason until the multiplexer tries again.
            if (!multiplexer.IsConnected && _lastError.Length == 0)
                _lastError = "No connection to " + _options.Ip + ":" + _options.Port + ".";

            await EnsureSubscribedAsync().ConfigureAwait(false);

            ConnectionChanged?.Invoke(this, IsConnected);
        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _lastError = Describe(e);
            ConnectionChanged?.Invoke(this, IsConnected);
        }

        private void OnConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _lastError = string.Empty;

            // The first subscribe fails when the server is down at that moment. Then the readers
            // poll until this one gets through.
            _ = Task.Run(EnsureSubscribedAsync);

            ConnectionChanged?.Invoke(this, IsConnected);
        }

        private static string Describe(ConnectionFailedEventArgs e)
        {
            var message = e.Exception?.Message;
            return string.IsNullOrEmpty(message) ? e.FailureType.ToString() : e.FailureType + ": " + message;
        }

        // The wake channel is an optimization. Without it the readers fall back to polling.
        private async Task EnsureSubscribedAsync()
        {
            var multiplexer = _multiplexer;
            if (multiplexer == null || _subscriber != null)
                return;

            if (Interlocked.CompareExchange(ref _subscribing, 1, 0) != 0)
                return;

            try
            {
                if (_subscriber == null)
                {
                    var subscriber = multiplexer.GetSubscriber();
                    await subscriber.SubscribeAsync(_wakeChannel, OnWake).ConfigureAwait(false);
                    _subscriber = subscriber;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                Volatile.Write(ref _subscribing, 0);
            }
        }

        private void OnWake(RedisChannel channel, RedisValue message) => Wake();

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

        private IDatabase Database()
        {
            if (_multiplexer == null)
                throw new InvalidOperationException("The Redis store is not connected. Call ConnectAsync first.");

            // No database argument, so DefaultDatabase from the configuration applies.
            return _multiplexer.GetDatabase();
        }

        public async Task<Snapshot> ReadSnapshotAsync()
        {
            var entries = await Database().HashGetAllAsync(_snapshotKey).ConfigureAwait(false);
            if (entries == null || entries.Length == 0)
                return null;

            var model = default(byte[]);
            var streamId = StreamPosition.Zero;
            var writtenBy = string.Empty;
            var writtenAt = default(DateTime);

            foreach (var entry in entries)
            {
                switch ((string)entry.Name)
                {
                    case SnapshotModelField:
                        model = (byte[])entry.Value;
                        break;
                    case SnapshotStreamIdField:
                        StreamPosition.TryParse(entry.Value, out streamId);
                        break;
                    case SnapshotWrittenByField:
                        writtenBy = entry.Value;
                        break;
                    case SnapshotWrittenAtField:
                        DateTime.TryParse(entry.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out writtenAt);
                        break;
                }
            }

            if (model == null)
                return null;

            return new Snapshot(model, streamId, writtenBy, writtenAt);
        }

        public async Task<StreamPosition> ReadSnapshotIdAsync()
        {
            var value = await Database().HashGetAsync(_snapshotKey, SnapshotStreamIdField).ConfigureAwait(false);
            return StreamPosition.TryParse(value, out var id) ? id : StreamPosition.Zero;
        }

        public Task WriteSnapshotAsync(Snapshot snapshot)
        {
            var entries = new[]
            {
                new HashEntry(SnapshotModelField, snapshot.Model),
                new HashEntry(SnapshotStreamIdField, snapshot.StreamId.ToString()),
                new HashEntry(SnapshotWrittenByField, snapshot.WrittenBy ?? string.Empty),
                new HashEntry(SnapshotWrittenAtField, snapshot.WrittenAt.ToString("o", CultureInfo.InvariantCulture))
            };

            return Database().HashSetAsync(_snapshotKey, entries);
        }

        public async Task<StreamPosition> AppendAsync(byte[] envelope)
        {
            var database = Database();
            var id = await database.StreamAddAsync(_streamKey, EnvelopeField, envelope).ConfigureAwait(false);
            // The tail key spares every reader an XINFO of the stream. One connection keeps the
            // order, so no reader is woken before the key holds the new ID.
            await database.StringSetAsync(_tailKey, id).ConfigureAwait(false);
            await database.PublishAsync(_wakeChannel, RedisValue.EmptyString).ConfigureAwait(false);
            return StreamPosition.Parse(id);
        }

        public async Task<IReadOnlyList<StreamEntry>> ReadRangeAsync(StreamPosition afterExclusive, int count)
        {
            // The "(" prefix asks Redis for an exclusive start. Redis 6.2 and later support it.
            var minId = "(" + afterExclusive;
            var entries = await Database().StreamRangeAsync(_streamKey, minId, "+", count).ConfigureAwait(false);
            return Convert(entries);
        }

        public async Task<IReadOnlyList<StreamEntry>> ReadBlockingAsync(StreamPosition afterExclusive, int count, TimeSpan timeout, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var entries = await ReadRangeAsync(afterExclusive, count).ConfigureAwait(false);
            if (entries.Count > 0)
                return entries;

            // XREAD BLOCK is not used. StackExchange.Redis multiplexes one connection, so a blocking
            // command would stall every other caller. A pub/sub wake-up does the same job.
            var signaled = await _wakeSignal.WaitAsync(timeout, ct).ConfigureAwait(false);
            if (!signaled)
                return Array.Empty<StreamEntry>();

            return await ReadRangeAsync(afterExclusive, count).ConfigureAwait(false);
        }

        // XINFO STREAM returns the first and the last entry with their payloads, so the tail key is
        // read instead. A store written before the key existed falls back to the newest entry.
        public async Task<StreamPosition> ReadTailAsync()
        {
            var database = Database();
            var value = await database.StringGetAsync(_tailKey).ConfigureAwait(false);
            if (StreamPosition.TryParse(value, out var tail))
                return tail;

            var entries = await database.StreamRangeAsync(_streamKey, "-", "+", 1, Order.Descending).ConfigureAwait(false);
            if (entries == null || entries.Length == 0)
                return StreamPosition.Zero;

            if (!StreamPosition.TryParse(entries[0].Id, out var id))
                return StreamPosition.Zero;

            // Fills the key so the next reader skips this fallback. Any peer may do this, because
            // the value is the same for all.
            await database.StringSetAsync(_tailKey, entries[0].Id).ConfigureAwait(false);
            return id;
        }

        // XTRIM goes through ExecuteAsync because the typed API only offers MAXLEN.
        public Task TrimAsync(StreamPosition minId)
            => Database().ExecuteAsync("XTRIM", _streamKey, "MINID", minId.ToString());

        public async Task HeartbeatAsync(string peerId, IReadOnlyDictionary<string, string> fields, TimeSpan ttl)
        {
            var database = Database();
            var key = _peerKeyPrefix + peerId;

            var entries = fields.Select(pair => new HashEntry(pair.Key, pair.Value ?? string.Empty)).ToArray();
            if (entries.Length > 0)
                await database.HashSetAsync(key, entries).ConfigureAwait(false);

            await database.KeyExpireAsync(key, ttl).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<PeerInfo>> ListPeersAsync()
        {
            var database = Database();
            var server = Server();
            var peers = new List<PeerInfo>();

            await foreach (var key in server.KeysAsync(_options.Database, _peerKeyPrefix + "*"))
            {
                var name = key.ToString();
                var peerId = name.Substring(_peerKeyPrefix.Length);
                var entries = await database.HashGetAllAsync(key).ConfigureAwait(false);
                if (entries == null || entries.Length == 0)
                    continue;

                peers.Add(ToPeerInfo(peerId, entries));
            }

            return peers;
        }

        private static PeerInfo ToPeerInfo(string peerId, HashEntry[] entries)
        {
            var name = string.Empty;
            var role = string.Empty;
            var host = string.Empty;
            var lastAppliedId = StreamPosition.Zero;

            foreach (var entry in entries)
            {
                switch ((string)entry.Name)
                {
                    case "name":
                        name = entry.Value;
                        break;
                    case "role":
                        role = entry.Value;
                        break;
                    case "host":
                        host = entry.Value;
                        break;
                    case "lastAppliedId":
                        if (!StreamPosition.TryParse(entry.Value, out lastAppliedId))
                            lastAppliedId = StreamPosition.Zero;
                        break;
                }
            }

            return new PeerInfo(peerId, name, role, host, lastAppliedId);
        }

        private IServer Server()
        {
            if (_multiplexer == null)
                throw new InvalidOperationException("The Redis store is not connected. Call ConnectAsync first.");

            var endPoints = _multiplexer.GetEndPoints();
            if (endPoints.Length == 0)
                throw new InvalidOperationException("The Redis store has no endpoint.");

            foreach (var endPoint in endPoints)
            {
                var candidate = _multiplexer.GetServer(endPoint);
                if (candidate.IsConnected)
                    return candidate;
            }

            return _multiplexer.GetServer(endPoints[0]);
        }

        private static IReadOnlyList<StreamEntry> Convert(RedisStreamEntry[] entries)
        {
            if (entries == null || entries.Length == 0)
                return Array.Empty<StreamEntry>();

            var result = new List<StreamEntry>(entries.Length);
            foreach (var entry in entries)
            {
                if (!StreamPosition.TryParse(entry.Id, out var id))
                    continue;

                var envelope = default(byte[]);
                foreach (var value in entry.Values)
                {
                    if ((string)value.Name == EnvelopeField)
                    {
                        envelope = (byte[])value.Value;
                        break;
                    }
                }

                if (envelope != null)
                    result.Add(new StreamEntry(id, envelope));
            }

            return result;
        }

        public async ValueTask DisposeAsync()
        {
            var multiplexer = _multiplexer;
            _multiplexer = null;
            if (multiplexer == null)
                return;

            multiplexer.ConnectionFailed -= OnConnectionFailed;
            multiplexer.ConnectionRestored -= OnConnectionRestored;

            await CloseAndDisposeAsync(multiplexer).ConfigureAwait(false);
        }

        private async Task CloseAndDisposeAsync(ConnectionMultiplexer multiplexer)
        {
            // Bounded, so a dead server cannot hold up shutdown.
            try
            {
                await Task.WhenAny(CloseAsync(multiplexer), Task.Delay(TimeSpan.FromSeconds(2))).ConfigureAwait(false);
            }
            catch (Exception)
            {
            }

            try
            {
                multiplexer.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private async Task CloseAsync(ConnectionMultiplexer multiplexer)
        {
            var subscriber = _subscriber;
            _subscriber = null;

            try
            {
                if (subscriber != null)
                    await subscriber.UnsubscribeAsync(_wakeChannel, OnWake).ConfigureAwait(false);
            }
            catch (Exception)
            {
            }

            try
            {
                await multiplexer.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
            }
        }
    }
}
