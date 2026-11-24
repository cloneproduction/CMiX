// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Tests
{
    // A clock for tests. One send interval of ValueSync is 1000 ticks.
    internal sealed class FakeTimeProvider : TimeProvider
    {
        public long Now { get; set; }

        public override long GetTimestamp() => Now;

        public override long TimestampFrequency => 60_000;
    }
}
