// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The arrow keys of the tree. They move the cursor of ProjectTree, but only over the rows that the tree shows.
    public sealed class ListTreeCursor
    {
        private readonly ProjectTree tree;
        private readonly Func<IReadOnlyList<ListTreeRow>> shownRows;

        // With no view, the shown rows come from a snapshot of the tree.
        public ListTreeCursor(ProjectTree tree)
            : this(tree, () => ListTreeSnapshot.Of(tree.Roots).VisibleRows())
        {
        }

        // The view gives its shown rows, so a key builds no snapshot.
        public ListTreeCursor(ProjectTree tree, Func<IReadOnlyList<ListTreeRow>> shownRows)
        {
            this.tree = tree;
            this.shownRows = shownRows;
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

        // A row with no shown row under it is a leaf of the tree, so Right opens the Add panel there.
        public bool IsCursorOnLeaf() => tree.CursorNode is { } node && ShownChildrenOf(node).Count == 0;

        private IReadOnlyList<TreeNode> ShownNodes() => shownRows().Select(row => row.Node).OfType<TreeNode>().ToArray();

        // A cursor row under a closed card is not shown, so its row comes from a snapshot.
        private IReadOnlyList<TreeNode> ShownChildrenOf(TreeNode node)
        {
            var row = shownRows().FirstOrDefault(shown => ReferenceEquals(shown.Node, node)) ?? ListTreeSnapshot.Of(tree.Roots).RowOf(node);
            return row?.Children.Select(child => child.Node).OfType<TreeNode>().ToArray() ?? Array.Empty<TreeNode>();
        }
    }
}
