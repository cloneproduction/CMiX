// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The arrow keys of the new tree. They move the cursor of ProjectTree as in the old tree, but only over the rows that the new tree shows.
    public sealed class ListTreeCursor
    {
        private readonly ProjectTree tree;

        public ListTreeCursor(ProjectTree tree)
        {
            this.tree = tree;
        }

        public void Move(int rows)
        {
            var nodes = ShownNodes();
            if (nodes.Count == 0)
                return;

            var index = tree.CursorNode == null ? -1 : TreeWalk.IndexOf(nodes, tree.CursorNode);
            var next = index < 0 ? (rows >= 0 ? 0 : nodes.Count - 1) : Math.Clamp(index + rows, 0, nodes.Count - 1);
            tree.CursorNode = nodes[next];
        }

        public void MoveToFirst()
        {
            var nodes = ShownNodes();
            if (nodes.Count > 0)
                tree.CursorNode = nodes[0];
        }

        public void MoveToLast()
        {
            var nodes = ShownNodes();
            if (nodes.Count > 0)
                tree.CursorNode = nodes[^1];
        }

        public void MoveOut()
        {
            if (tree.CursorNode is not { } node)
                return;

            if (node.IsExpanded && ShownChildrenOf(node).Count > 0)
                node.IsExpanded = false;
            else if (node.Parent is { Parent: not null } parent)
                tree.CursorNode = parent;
        }

        public void MoveIn()
        {
            if (tree.CursorNode is not { } node || ShownChildrenOf(node) is not { Count: > 0 } children)
                return;

            if (!node.IsExpanded)
                node.IsExpanded = true;
            else
                tree.CursorNode = children[0];
        }

        // A row with no shown row under it is a leaf of the new tree, so Right opens the Add panel there.
        public bool IsCursorOnLeaf() => tree.CursorNode is { } node && ShownChildrenOf(node).Count == 0;

        private IReadOnlyList<TreeNode> ShownNodes() =>
            ListTreeSnapshot.Of(tree.Roots).VisibleRows().Select(row => row.Node).OfType<TreeNode>().ToArray();

        private IReadOnlyList<TreeNode> ShownChildrenOf(TreeNode node) =>
            ListTreeSnapshot.Of(tree.Roots).RowOf(node)?.Children.Select(row => row.Node).OfType<TreeNode>().ToArray()
            ?? Array.Empty<TreeNode>();
    }
}
