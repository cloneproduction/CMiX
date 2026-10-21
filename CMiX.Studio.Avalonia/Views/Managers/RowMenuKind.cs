// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Views.Managers
{
    // The menu that the rows of a list get.
    public enum RowMenuKind
    {
        None,
        // The list of a layer: the items are in this list only by reference, so a row can be moved and removed from it.
        Slot,
        // The list of all the items of the project: a row has no place to move to, and it is deleted from every list.
        Repository,
        // A row of the layer list or the composition list: it moves and is deleted like a slot row.
        Composable
    }
}
