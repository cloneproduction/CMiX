// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The shown target is the key of the visible card when the target is a group row that option E hides.
    public sealed record ListTreeSlot(TreeNode Target, DropPosition Position, ListPlace ListPlace, ListTreeRowKey ShownTarget);
}
