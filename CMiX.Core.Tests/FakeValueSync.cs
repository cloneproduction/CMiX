// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Tests
{
    // A value sync for tests. It records each call in Calls and does nothing else.
    internal sealed class FakeValueSync : IValueSync
    {
        public List<string> Calls { get; } = new();

        public int Changes { get; private set; }

        public IControlModel LastBefore { get; private set; }

        public IControlModel LastAfter { get; private set; }

        public void Changed(IControl value, IControlModel before, IControlModel after)
        {
            Calls.Add("Changed");
            Changes++;
            LastBefore = before;
            LastAfter = after;
        }
    }
}
