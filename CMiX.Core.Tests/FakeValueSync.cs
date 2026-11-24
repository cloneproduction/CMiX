// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Tests
{
    // A value sync for tests. It records each call in Calls and does nothing else.
    internal sealed class FakeValueSync : IValueSync
    {
        public List<string> Calls { get; } = new();

        public void Changing(IControl value) => Calls.Add("Changing");

        public void Changed(IControl value) => Calls.Add("Changed");

        public void Send(IControl value) => Calls.Add("Send");

        // An active value that writes to this sync.
        public GenericValue<T> CreateActive<T>()
        {
            var value = new GenericValue<T>(this, new ControlActivationService());
            value.Activate();
            return value;
        }
    }
}
