// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Collections.Immutable;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The selected node and the drop target, and what they mean for each row and each card area.
    public sealed record ListTreeMarks(TreeNode? Selected, TreeNode? DropTarget)
    {
        // The other selected rows, which look the same as the selected node.
        public IReadOnlySet<TreeNode> AlsoSelected { get; init; } = ImmutableHashSet<TreeNode>.Empty;

        public bool IsSelected(TreeNode? node) => node != null && (ReferenceEquals(node, Selected) || AlsoSelected.Contains(node));

        public bool IsDropTarget(TreeNode? node) => node != null && ReferenceEquals(node, DropTarget);

        // A row under a hidden row looks hidden too.
        public bool IsHidden(TreeNode? node) => node != null && (node.IsHidden || node.IsUnderHiddenParent);
    }
}
