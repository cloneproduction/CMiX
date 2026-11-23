// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // A legal slot with the tree as if the source were dropped on it.
    public sealed record ListTreeCandidate(ListTreeSlot Slot, ListTreeSnapshot Snapshot, ListTreeLayout Layout, ListTreeSpot Spot);
}
