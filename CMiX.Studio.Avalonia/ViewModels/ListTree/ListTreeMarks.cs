// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Collections.Immutable;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The selected node, the card under the pointer and the drop target, and what they mean for each row and each card area.
    public sealed record ListTreeMarks(TreeNode? Selected, TreeNode? HoveredCard, TreeNode? DropTarget)
    {
        // The other selected rows, which look the same as the selected node.
        public IReadOnlySet<TreeNode> AlsoSelected { get; init; } = ImmutableHashSet<TreeNode>.Empty;

        public bool IsSelected(TreeNode? node) => node != null && (ReferenceEquals(node, Selected) || AlsoSelected.Contains(node));

        public bool IsDropTarget(TreeNode? node) => node != null && ReferenceEquals(node, DropTarget);

        public bool IsHoveredCard(TreeNode? node) => node != null && ReferenceEquals(node, HoveredCard);

        public bool IsInSelection(TreeNode? node)
        {
            for (var parent = node?.Parent; parent != null; parent = parent.Parent)
                if (IsSelected(parent))
                    return true;

            return false;
        }

        public bool IsInHoveredCard(TreeNode? node) => IsInside(node, HoveredCard);

        // A row under a hidden row looks hidden too.
        public bool IsHidden(TreeNode? node) => node != null && (node.IsHidden || node.IsUnderHiddenParent);

        private static bool IsInside(TreeNode? node, TreeNode? card) =>
            node != null && card != null && !ReferenceEquals(node, card) && node.IsOrIsUnder(card);
    }
}
