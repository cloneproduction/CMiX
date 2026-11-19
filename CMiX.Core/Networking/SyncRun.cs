// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Networking
{
    // The resources of one Start. The follower starts after the join, so its trio is set later.
    internal sealed class SyncRun
    {
        public SyncRun(ISyncStore store, SyncTimings timings, CancellationTokenSource cts, bool autoJoin)
        {
            Store = store;
            Timings = timings;
            Cts = cts;
            AutoJoin = autoJoin;
        }

        public ISyncStore Store { get; }
        public SyncTimings Timings { get; }
        public CancellationTokenSource Cts { get; }
        public bool AutoJoin { get; }

        public StreamFollower Follower { get; set; }
        public CancellationTokenSource FollowerCts { get; set; }
        public Task FollowerTask { get; set; }
    }
}
