// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Threading.Channels;

namespace CMiX.Core.Networking
{
    // Holds serialized envelopes until one writer task appends them to the store in order.
    public sealed class OutgoingQueue
    {
        private readonly Channel<byte[]> _channel = Channel.CreateUnbounded<byte[]>();
        // One loop at a time. An append is not cancellable, so the loop of a new run must wait for
        // the loop of the old run. Otherwise both peek the same entry.
        private readonly SemaphoreSlim _consumer = new(1, 1);

        public int PendingCount => _channel.Reader.Count;

        public void Enqueue(byte[] envelope) => _channel.Writer.TryWrite(envelope);

        // A failed append waits and retries the same entry, so nothing is lost or reordered while
        // the store is down. onSent returns false when the run is over. The entry then stays in the
        // queue for the loop of the next run.
        public async Task RunAsync(ISyncStore store, Func<StreamPosition, Task<bool>> onSent, CancellationToken ct)
        {
            await _consumer.WaitAsync(ct).ConfigureAwait(false);
            try
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

                        if (!await onSent(id).ConfigureAwait(false)) return;

                        reader.TryRead(out _);
                    }
                }
            }
            finally
            {
                _consumer.Release();
            }
        }
    }
}
