// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking
{
    // The full project state, plus the stream position it was taken at.
    public record Snapshot(byte[] Model, StreamPosition StreamId, string WrittenBy, DateTime WrittenAt);

    // One serialized message envelope, with the ID the store gave it.
    public record StreamEntry(StreamPosition Id, byte[] Envelope);

    public record PeerInfo(string PeerId, string Name, string Role, string Host, StreamPosition LastAppliedId);

    // The store operations the sync protocol needs. Every method is asynchronous, because the UI
    // thread must never wait for the store.
    public interface ISyncStore : IAsyncDisposable
    {
        bool IsConnected { get; }

        event Action<bool> ConnectionChanged;

        // Returns when the first connect attempt is done or has failed. It does not throw when the
        // store is unreachable. IsConnected stays false and the store reconnects on its own.
        Task ConnectAsync(CancellationToken ct);

        // Returns null when there is no snapshot.
        Task<Snapshot> ReadSnapshotAsync();

        Task WriteSnapshotAsync(Snapshot snapshot);

        // Returns the ID the store assigned to the new entry.
        Task<StreamPosition> AppendAsync(byte[] envelope);

        // Entries with an ID greater than afterExclusive, oldest first, at most count.
        Task<IReadOnlyList<StreamEntry>> ReadRangeAsync(StreamPosition afterExclusive, int count);

        // Like ReadRangeAsync with count 256. When the result is empty, it waits up to timeout for a
        // wake-up signal and then reads once more. Returns an empty list on timeout.
        Task<IReadOnlyList<StreamEntry>> ReadBlockingAsync(StreamPosition afterExclusive, TimeSpan timeout, CancellationToken ct);

        // The ID of the newest entry, or StreamPosition.Zero when the stream is empty or missing.
        Task<StreamPosition> ReadTailAsync();

        // Removes the entries with an ID lower than minId.
        Task TrimAsync(StreamPosition minId);

        // Writes the peer fields and sets the time to live on them.
        Task HeartbeatAsync(string peerId, IReadOnlyDictionary<string, string> fields, TimeSpan ttl);

        Task<IReadOnlyList<PeerInfo>> ListPeersAsync();
    }
}
