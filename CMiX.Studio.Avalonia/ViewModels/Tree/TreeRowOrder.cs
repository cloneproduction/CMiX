// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using CMiX.Core.Compositing;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public static class TreeRowOrder
    {
        // The last row that is shown in the block of a node: itself when it is collapsed or has no children.
        public static TreeNode LastVisible(TreeNode node) =>
            node.IsExpanded && node.Children.Count > 0 ? LastVisible(node.Children[^1]) : node;

        // The row whose space below it is the place of a drop before the node: the last visible row of the sibling above,
        // or the parent when the node is the first child. Null when the node is the first row of the tree.
        public static TreeNode? RowAbove(TreeNode node)
        {
            if (node.Parent == null)
                return null;

            var siblings = node.Parent.Children;
            var index = siblings.IndexOf(node);
            if (index > 0)
                return LastVisible(siblings[index - 1]);

            return node.Parent;
        }

        public static TreeNode? CompositionOf(TreeNode node) =>
            TreeWalk.ParentsOf(node).Prepend(node).FirstOrDefault(row => row.Item is Composition);
    }
}
