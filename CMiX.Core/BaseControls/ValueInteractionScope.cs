// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    // Handle over the scope its holder opened. Disposing ends that scope and nothing else: a
    // handle whose scope already ended, or was replaced by a later Begin, ends nothing, so a
    // control that never opened a scope, or whose gesture is long over, cannot flush a gesture
    // it does not own. A default handle owns no scope and disposing it is always a no op.
    public readonly struct ValueInteractionScope : IDisposable
    {
        internal ValueInteractionScope(ValueInteraction owner, long token)
        {
            _owner = owner;
            _token = token;
        }

        private readonly ValueInteraction _owner;
        private readonly long _token;

        public void Dispose()
        {
            _owner?.End(_token);
        }
    }
}
