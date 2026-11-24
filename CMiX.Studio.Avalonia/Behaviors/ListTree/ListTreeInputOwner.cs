// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The part of the tree that holds the pointer and the keys. A grab holds them until the release of its last click.
    internal enum ListTreeInputOwner
    {
        None,
        Drag,
        Grab
    }
}
