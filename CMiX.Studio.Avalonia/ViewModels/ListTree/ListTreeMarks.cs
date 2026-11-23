// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The selected node and the card under the pointer, and what they mean for each row and each card area.
    public sealed record ListTreeMarks(TreeNode? Selected, TreeNode? HoveredCard)
    {
        public bool IsSelected(TreeNode? node) => node != null && ReferenceEquals(node, Selected);

        public bool IsHoveredCard(TreeNode? node) => node != null && ReferenceEquals(node, HoveredCard);

        public bool IsInSelection(TreeNode? node) => IsInside(node, Selected);

        public bool IsInHoveredCard(TreeNode? node) => IsInside(node, HoveredCard);

        private static bool IsInside(TreeNode? node, TreeNode? card) =>
            node != null && card != null && !ReferenceEquals(node, card) && node.IsOrIsUnder(card);
    }
}
