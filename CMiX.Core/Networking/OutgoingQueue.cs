// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Channels;

namespace CMiX.Core.Networking
{
    // Holds serialized envelopes until one writer task appends them to the store in order.
    public sealed class OutgoingQueue
    {
        private readonly record struct Item(long Sequence, byte[] Envelope);

        private readonly Channel<Item> _channel = Channel.CreateUnbounded<Item>();
        // One loop at a time. An append is not cancellable, so the loop of a new run must wait for
        // the loop of the old run. Otherwise both peek the same entry.
        private readonly SemaphoreSlim _consumer = new(1, 1);
        private readonly object _gate = new();

        private long _nextSequence;
        private long _discardBelow;
        private TaskCompletionSource<bool> _open = NewSource();
        private TaskCompletionSource<bool> _idle = CompletedSource();

        public int PendingCount => _channel.Reader.Count;

        public void Enqueue(byte[] envelope)
        {
            lock (_gate)
                _channel.Writer.TryWrite(new Item(_nextSequence++, envelope));
        }

        // The loop appends only while the queue is open. The peer opens it when it is joined.
        public void SetOpen(bool open)
        {
            lock (_gate)
            {
                if (open)
                    _open.TrySetResult(true);
                else if (_open.Task.IsCompleted)
                    _open = NewSource();
            }
        }

        // Marks every entry queued so far. The loop drops them without an append.
        public void Discard()
        {
            lock (_gate)
                _discardBelow = _nextSequence;
        }

        // Completes when no append is in flight.
        public Task IdleAsync(CancellationToken ct)
        {
            Task idle;
            lock (_gate)
                idle = _idle.Task;

            return idle.WaitAsync(ct);
        }

        // A failed append waits and retries the same entry, so nothing is lost or reordered while
        // the store is down. onSent returns false when the run is over. The entry then stays in the
        // queue for the loop of the next run. onDrained comes after the entry left the queue, so a
        // listener of it reads the true pending count.
        public async Task RunAsync(ISyncStore store, SyncTimings timings, Func<StreamPosition, Task<bool>> onSent,
            Func<Task> onDrained, CancellationToken ct)
        {
            await _consumer.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                var reader = _channel.Reader;
                while (await reader.WaitToReadAsync(ct).ConfigureAwait(false))
                {
                    while (reader.TryPeek(out var item))
                    {
                        ct.ThrowIfCancellationRequested();

                        if (IsDiscarded(item.Sequence))
                        {
                            reader.TryRead(out _);
                            await onDrained().ConfigureAwait(false);
                            continue;
                        }

                        await OpenTask().WaitAsync(ct).ConfigureAwait(false);
                        if (IsDiscarded(item.Sequence)) continue;

                        BeginAppend();
                        try
                        {
                            StreamPosition id;
                            try
                            {
                                id = await store.AppendAsync(item.Envelope).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                throw;
                            }
                            catch (Exception)
                            {
                                EndAppend();
                                await Task.Delay(timings.RetryDelay, ct).ConfigureAwait(false);
                                continue;
                            }

                            if (!await onSent(id).ConfigureAwait(false)) return;

                            reader.TryRead(out _);
                            await onDrained().ConfigureAwait(false);
                        }
                        finally
                        {
                            EndAppend();
                        }
                    }
                }
            }
            finally
            {
                _consumer.Release();
            }
        }

        private bool IsDiscarded(long sequence)
        {
            lock (_gate)
                return sequence < _discardBelow;
        }

        private Task OpenTask()
        {
            lock (_gate)
                return _open.Task;
        }

        private void BeginAppend()
        {
            lock (_gate)
            {
                if (_idle.Task.IsCompleted)
                    _idle = NewSource();
            }
        }

        private void EndAppend()
        {
            lock (_gate)
                _idle.TrySetResult(true);
        }

        private static TaskCompletionSource<bool> NewSource()
            => new(TaskCreationOptions.RunContinuationsAsynchronously);

        private static TaskCompletionSource<bool> CompletedSource()
        {
            var source = NewSource();
            source.SetResult(true);
            return source;
        }
    }
}
