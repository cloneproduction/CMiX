// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using CMiX.Core.Compositing;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public static class TreeRowOrder
    {
        public static TreeNode LastVisible(TreeNode node) =>
            node.IsExpanded && node.Children.Count > 0 ? LastVisible(node.Children[^1]) : node;

        // Its space below is the place of a drop before the node. Null for the first row of the tree.
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
