// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;

namespace CMiX.Core.Networking
{
    // Reads the stream after the last applied entry and hands every read batch to the peer. After a
    // store error or a long pause it lets the peer check the gap before it reads again.
    public sealed class StreamFollower
    {
        private readonly ISyncStore _store;
        private readonly Func<StreamPosition> _position;
        private readonly Func<IReadOnlyList<StreamEntry>, Task> _apply;
        private readonly Func<Task> _checkGap;
        private readonly SyncTimings _timings;
        private readonly SemaphoreSlim _wakeSignal = new(0, 1);

        // When the last read returned. A pause without a store error leaves no other trace.
        private long _lastRead = Stopwatch.GetTimestamp();

        public StreamFollower(ISyncStore store, Func<StreamPosition> position, Func<IReadOnlyList<StreamEntry>, Task> apply, Func<Task> checkGap, SyncTimings timings)
        {
            _store = store;
            _position = position;
            _apply = apply;
            _checkGap = checkGap;
            _timings = timings;
        }

        // Ends the recovery wait. The peer calls this on a reconnect, so the follower does not sleep
        // out a long backoff while the store is back.
        public void Wake()
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

        public async Task RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    // The process was away, for example suspended. The stream can be trimmed past
                    // the own position, and the next entries would hide the gap.
                    if (Stopwatch.GetElapsedTime(_lastRead) > _timings.StalePause)
                    {
                        await _checkGap().ConfigureAwait(false);
                        _lastRead = Stopwatch.GetTimestamp();
                    }

                    var started = Stopwatch.GetTimestamp();
                    var entries = await _store.ReadBlockingAsync(_position(), _timings.ReadTimeout, ct).ConfigureAwait(false);
                    _lastRead = Stopwatch.GetTimestamp();

                    // The read itself was away that long. Its entries can start after a trim point,
                    // so they are dropped and read again after the check.
                    if (entries.Count > 0 && Stopwatch.GetElapsedTime(started) > _timings.StalePause)
                    {
                        await _checkGap().ConfigureAwait(false);
                        continue;
                    }

                    if (entries.Count > 0)
                        await _apply(entries).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (!await RecoverAsync(ct).ConfigureAwait(false))
                        break;
                }
            }
        }

        // Reads continue only after the error check has run once without an error, so a trimmed
        // stream is never read past the own position.
        private async Task<bool> RecoverAsync(CancellationToken ct)
        {
            var backoff = _timings.MinBackoff;
            while (true)
            {
                try
                {
                    await _wakeSignal.WaitAsync(backoff, ct).ConfigureAwait(false);
                    await _checkGap().ConfigureAwait(false);
                    _lastRead = Stopwatch.GetTimestamp();
                    return true;
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    backoff = TimeSpan.FromTicks(Math.Min(backoff.Ticks * 2, _timings.MaxBackoff.Ticks));
                }
            }
        }
    }
}
