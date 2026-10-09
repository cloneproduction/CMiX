// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    // Implemented by controls that coalesce their undo step and throttle their outgoing messages
    // while a ValueInteraction scope is open. EndInteraction must always flush the final value.
    public interface IInteractiveValue
    {
        void EndInteraction();
    }
}
