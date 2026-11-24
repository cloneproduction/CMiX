// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Tests
{
    // A value sync for tests. It records each call in Calls and does nothing else.
    internal sealed class FakeValueSync : IValueSync
    {
        public List<string> Calls { get; } = new();

        public int Records { get; private set; }

        public int Sends { get; private set; }

        public int Commits { get; private set; }

        public IControlModel LastBefore { get; private set; }

        public IControlModel LastAfter { get; private set; }

        public void Record(IControl value, IControlModel before, IControlModel after)
        {
            Calls.Add("Record");
            Records++;
            LastBefore = before;
            LastAfter = after;
        }

        public void Commit(IControl value, IControlModel before, IControlModel after)
        {
            Calls.Add("Commit");
            Commits++;
            LastBefore = before;
            LastAfter = after;
        }

        public void Send(IControl value)
        {
            Calls.Add("Send");
            Sends++;
        }
    }
}
