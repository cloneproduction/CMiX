// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Networking
{
    // The time values of the sync protocol, in one place so all peers of a run agree on them. A peer
    // takes the set of its Start. The apps use Default, the unit tests use a short set.
    public sealed record SyncTimings(
        // How long a reader waits for new entries before it reads again.
        TimeSpan ReadTimeout,
        TimeSpan HeartbeatInterval,
        TimeSpan HeartbeatTtl,
        // Entries younger than this are never trimmed, so a peer that lags a little does not lose
        // them. A peer that lags more re-joins from the snapshot.
        TimeSpan Retention,
        // A follower that made no read for this long checks the gap before it applies anything.
        TimeSpan StalePause,
        // A value change waits this long before the Studio writes a new snapshot.
        TimeSpan CompactionDelay,
        TimeSpan RetryDelay,
        TimeSpan MinBackoff,
        TimeSpan MaxBackoff,
        // How long the stop work waits for the loops of the run before it disposes the store.
        TimeSpan StopTimeout,
        // How many entries one read returns, and so how many one dispatched action applies.
        int ReadBatch)
    {
        public static readonly SyncTimings Default = new(
            ReadTimeout: TimeSpan.FromMilliseconds(250),
            HeartbeatInterval: TimeSpan.FromSeconds(2),
            HeartbeatTtl: TimeSpan.FromSeconds(6),
            Retention: TimeSpan.FromSeconds(60),
            StalePause: TimeSpan.FromSeconds(30),
            CompactionDelay: TimeSpan.FromSeconds(5),
            RetryDelay: TimeSpan.FromMilliseconds(500),
            MinBackoff: TimeSpan.FromSeconds(1),
            MaxBackoff: TimeSpan.FromSeconds(10),
            StopTimeout: TimeSpan.FromSeconds(5),
            ReadBatch: 256);
    }
}
