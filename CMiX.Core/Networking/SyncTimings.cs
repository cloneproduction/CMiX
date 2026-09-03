// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking
{
    // The time constants of the sync protocol, in one place so all peers agree on them.
    public static class SyncTimings
    {
        // How long a reader waits for new entries before it reads again.
        public static readonly TimeSpan ReadTimeout = TimeSpan.FromMilliseconds(250);

        public static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(2);
        public static readonly TimeSpan HeartbeatTtl = TimeSpan.FromSeconds(6);

        // Entries younger than this are never trimmed, so a peer that lags a little does not lose
        // them. A peer that lags more re-joins from the snapshot.
        public static readonly TimeSpan Retention = TimeSpan.FromSeconds(60);

        // A follower that made no read for this long checks the gap before it applies anything.
        public static readonly TimeSpan StalePause = TimeSpan.FromSeconds(30);

        // A value change waits this long before the Studio writes a new snapshot.
        public static readonly TimeSpan CompactionDelay = TimeSpan.FromSeconds(5);

        public static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(500);
        public static readonly TimeSpan MinBackoff = TimeSpan.FromSeconds(1);
        public static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(10);
    }
}
