// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The node is the identity, because one item can show under several layers.
    public sealed record ListTreeRow(TreeNode Node, int Depth, bool IsCard, bool IsExpanded, IReadOnlyList<ListTreeRow> Children);
}
