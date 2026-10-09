// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Prefabs
{
    // Shared by the collection teardown paths and by the undo commands that own a removed control
    // while it waits on a stack. A control may only be disposed once nothing references it any
    // more, so an undo that put it back into a collection makes the later disposal a no op.
    internal static class OwnedControl
    {
        public static void DisposeIfOrphaned(ControlRepository repository, IControl control)
        {
            if (control is not IDisposable disposable) return;
            if (repository == null) return;
            if (repository.GetControl(control.ID) != null) return;
            disposable.Dispose();
        }
    }
}
