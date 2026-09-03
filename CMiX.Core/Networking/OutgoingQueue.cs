// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Threading.Channels;

namespace CMiX.Core.Networking
{
    // Holds serialized envelopes until one writer task appends them to the store in order.
    public sealed class OutgoingQueue
    {
        private readonly Channel<byte[]> _channel = Channel.CreateUnbounded<byte[]>();

        public int PendingCount => _channel.Reader.Count;

        public void Enqueue(byte[] envelope) => _channel.Writer.TryWrite(envelope);

        // A failed append waits and retries the same entry, so nothing is lost or reordered while
        // the store is down.
        public async Task RunAsync(ISyncStore store, Func<StreamPosition, Task> onSent, CancellationToken ct)
        {
            var reader = _channel.Reader;
            while (await reader.WaitToReadAsync(ct).ConfigureAwait(false))
            {
                while (reader.TryPeek(out var envelope))
                {
                    ct.ThrowIfCancellationRequested();

                    StreamPosition id;
                    try
                    {
                        id = await store.AppendAsync(envelope).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        await Task.Delay(SyncTimings.RetryDelay, ct).ConfigureAwait(false);
                        continue;
                    }

                    await onSent(id).ConfigureAwait(false);
                    reader.TryRead(out _);
                }
            }
        }
    }
}
