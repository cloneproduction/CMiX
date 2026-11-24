// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    // Carries a value change to the undo stack and to the engine.
    public interface IValueSync
    {
        // Records the undo step, then sends the value.
        void Commit(IControl value, IControlModel before, IControlModel after);
        // Sends the value. No undo step.
        void Send(IControl value);
    }
}
