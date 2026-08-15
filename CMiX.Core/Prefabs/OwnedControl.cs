// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
