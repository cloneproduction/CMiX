// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The shown rows of a tree and their places, in tree order.
    public sealed record ListTreeArrangement(IReadOnlyList<ListTreeRow> Rows, ListTreeLayout Layout)
    {
        public static ListTreeArrangement Empty { get; } = Of(Array.Empty<TreeNode>(), ListTreeSizes.Default);

        public static ListTreeArrangement Of(IEnumerable<TreeNode> roots, ListTreeSizes sizes)
        {
            var snapshot = ListTreeSnapshot.Of(roots);
            return new ListTreeArrangement(snapshot.VisibleRows(), ListTreeLayout.Of(snapshot, sizes));
        }
    }
}
