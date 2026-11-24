// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core
{
    // Carries a value change to the undo stack and to the engine.
    public interface IValueSync
    {
        // A write of the value. A null before means no undo step.
        void Changed(IControl value, IControlModel before, IControlModel after);
    }
}
