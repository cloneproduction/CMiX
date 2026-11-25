// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core
{
    // Carries a value change to the undo stack and to the engine.
    public interface IValueSync
    {
        // Before the value changes. The sync keeps the model when it will record an undo step.
        void Changing(IControl value);

        // After the value changed. Records the undo step, then sends the value.
        void Changed(IControl value);

        // Sends the value. No undo step. For a momentary value.
        void Send(IControl value);

        // Opens a drag of the value. The handle ends it: it sends the last value and records one undo step.
        IDisposable BeginInteraction();
    }
}
