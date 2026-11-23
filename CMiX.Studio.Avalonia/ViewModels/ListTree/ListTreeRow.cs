// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The key is the identity. The node is null for a row that the live tree does not have.
    public sealed record ListTreeRow(ListTreeRowKey Key, TreeNode? Node, int Depth, bool IsCard, bool IsGroup, bool IsExpanded, IReadOnlyList<ListTreeRow> Children)
    {
        // A group row shows a list, and option E leaves out a list with no row.
        public bool IsShown => !IsGroup || Children.Count > 0;
    }
}
