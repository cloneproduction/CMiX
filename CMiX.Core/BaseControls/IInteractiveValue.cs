// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    // A value sync that takes part in the open drag. EndInteraction flushes the last value and records the undo step.
    public interface IInteractiveValue
    {
        void EndInteraction();
    }
}
