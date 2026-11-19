// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public static class TreeWalk
    {
        public static IReadOnlyList<TreeNode> AllRows(IEnumerable<TreeNode> roots)
        {
            var rows = new List<TreeNode>();
            AddAllRows(roots, rows);
            return rows;
        }

        private static void AddAllRows(IEnumerable<TreeNode> nodes, List<TreeNode> rows)
        {
            foreach (var node in nodes)
            {
                rows.Add(node);
                AddAllRows(node.Children, rows);
            }
        }

        public static IReadOnlyList<TreeNode> VisibleRows(IEnumerable<TreeNode> roots)
        {
            var rows = new List<TreeNode>();
            AddVisibleRows(roots, rows);
            return rows;
        }

        private static void AddVisibleRows(IEnumerable<TreeNode> nodes, List<TreeNode> rows)
        {
            foreach (var node in nodes)
            {
                rows.Add(node);
                if (node.IsExpanded)
                    AddVisibleRows(node.Children, rows);
            }
        }

        // The hidden root of the tree has no parent. It is not one of the parents.
        public static IEnumerable<TreeNode> ParentsOf(TreeNode node)
        {
            for (var parent = node.Parent; parent?.Parent != null; parent = parent.Parent)
                yield return parent;
        }

        public static int DepthOf(TreeNode node) => ParentsOf(node).Count();

        public static TreeNode? FindNode(IEnumerable<TreeNode> roots, Func<TreeNode, bool> match) =>
            AllRows(roots).FirstOrDefault(match);

        public static int IndexOf(IReadOnlyList<TreeNode> rows, TreeNode node)
        {
            for (var i = 0; i < rows.Count; i++)
                if (ReferenceEquals(rows[i], node))
                    return i;

            return -1;
        }
    }
}
