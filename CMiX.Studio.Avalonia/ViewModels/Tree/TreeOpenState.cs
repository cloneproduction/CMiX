// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The open state of the rows: open and close by branch, by level, all, and all but the path to the target.
    // The hub (ProjectTree) owns the drag, the target and the cursor, and passes them in.
    public sealed class TreeOpenState
    {
        private readonly Action<TreeNode?, Func<IReadOnlyList<TreeNode>>> closeRowsInView;
        private readonly TreeSearch search;

        public TreeOpenState(Action<TreeNode?, Func<IReadOnlyList<TreeNode>>> closeRowsInView, TreeSearch search)
        {
            this.closeRowsInView = closeRowsInView;
            this.search = search;
        }

        public void ExpandBranch(TreeNode target)
        {
            // A row that the search opened is already open. It leaves the search, so the end of the search does not close it.
            foreach (var node in TreeWalk.AllRows(new[] { target }).Where(node => node.Children.Count > 0))
            {
                node.IsExpanded = true;
                search.Release(node);
            }
        }

        // Every open row with children at the greatest depth among the rows that show closes.
        public void CloseLevel(IEnumerable<TreeNode> roots, TreeNode? target)
        {
            var open = TreeWalk.VisibleRows(roots).Where(node => node.IsExpanded && node.Children.Count > 0).ToList();
            if (open.Count == 0)
                return;

            var depth = open.Max(TreeWalk.DepthOf);
            CloseRows(open.Where(node => TreeWalk.DepthOf(node) == depth).ToList(), target);
        }

        // Every closed row with children at the smallest depth among the rows that show opens.
        public void OpenLevel(IEnumerable<TreeNode> roots)
        {
            var closed = TreeWalk.VisibleRows(roots).Where(node => !node.IsExpanded && node.Children.Count > 0).ToList();
            if (closed.Count == 0)
                return;

            var depth = closed.Min(TreeWalk.DepthOf);
            foreach (var node in closed.Where(node => TreeWalk.DepthOf(node) == depth))
                node.IsExpanded = true;
        }

        // Every open row with children closes, also a row under a closed row, so a row opens later with its children closed.
        public void CollapseAll(IEnumerable<TreeNode> roots, TreeNode? target)
        {
            var open = TreeWalk.AllRows(roots).Where(node => node.IsExpanded && node.Children.Count > 0).ToList();
            if (open.Count > 0)
                CloseRows(open, target);
        }

        // Opens the parents of the target row and closes every other row. The target keeps its state. Returns the rows that it closed.
        public IReadOnlyList<TreeNode> CollapseOthers(IEnumerable<TreeNode> roots, TreeNode target)
        {
            var closed = new List<TreeNode>();
            var path = new HashSet<TreeNode>(TreeWalk.ParentsOf(target));
            foreach (var node in TreeWalk.AllRows(roots))
            {
                // A parent that the search opened is already open, so no change tells the search. It leaves the search here, so
                // the end of the search does not close it.
                if (path.Contains(node))
                {
                    node.IsExpanded = true;
                    search.Release(node);
                }
                else if (!ReferenceEquals(node, target) && node.IsExpanded && node.Children.Count > 0)
                {
                    node.IsExpanded = false;
                    closed.Add(node);
                }
            }

            return closed;
        }

        // The opened parents leave the search, so the end of the search does not close them.
        public void OpenParents(TreeNode node)
        {
            foreach (var parent in TreeWalk.ParentsOf(node))
            {
                parent.IsExpanded = true;
                search.Release(parent);
            }
        }

        // Closes the rows in tree order through the active view. The view keeps still the row that shows the target after the
        // close: the highest of its parents that closes, else the target itself.
        private void CloseRows(IReadOnlyList<TreeNode> rows, TreeNode? target)
        {
            var closing = new HashSet<TreeNode>(rows);
            var anchor = target != null ? TreeWalk.ParentsOf(target).LastOrDefault(closing.Contains) ?? target : null;
            closeRowsInView(anchor, () =>
            {
                foreach (var node in rows)
                    node.IsExpanded = false;
                return rows;
            });
        }
    }
}
