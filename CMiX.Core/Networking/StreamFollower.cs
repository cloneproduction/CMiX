// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;

namespace CMiX.Core.Networking
{
    // Reads the stream after the last applied entry and hands every entry to the peer. On a store
    // error it waits with backoff and lets the peer check the snapshot before it reads again.
    public sealed class StreamFollower
    {
        private readonly ISyncStore _store;
        private readonly Func<StreamPosition> _position;
        private readonly Func<StreamEntry, Task> _apply;
        private readonly Func<Task> _onError;

        public StreamFollower(ISyncStore store, Func<StreamPosition> position, Func<StreamEntry, Task> apply, Func<Task> onError)
        {
            _store = store;
            _position = position;
            _apply = apply;
            _onError = onError;
        }

        public async Task RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var entries = await _store.ReadBlockingAsync(_position(), SyncTimings.ReadTimeout, ct).ConfigureAwait(false);
                    foreach (var entry in entries)
                        await _apply(entry).ConfigureAwait(false);
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
            var backoff = SyncTimings.MinBackoff;
            while (true)
            {
                try
                {
                    await Task.Delay(backoff, ct).ConfigureAwait(false);
                    await _onError().ConfigureAwait(false);
                    return true;
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    backoff = TimeSpan.FromTicks(Math.Min(backoff.Ticks * 2, SyncTimings.MaxBackoff.Ticks));
                }
            }
        }
    }
}
