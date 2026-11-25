// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The search marks the rows and never changes IsExpanded: the filter of the view shows the parents of the matches open.
    public sealed class TreeSearch
    {
        private IReadOnlyList<TreeNode> matches = Array.Empty<TreeNode>();

        public IReadOnlyList<TreeNode> Matches => matches;

        public bool Refresh(string text, IEnumerable<TreeNode> roots)
        {
            var found = text.Length == 0 ? ClearLooks(roots) : ShowLooks(text, roots);
            if (found.SequenceEqual(matches))
                return false;

            matches = found;
            return true;
        }

        private static List<TreeNode> ClearLooks(IEnumerable<TreeNode> roots)
        {
            foreach (var node in TreeWalk.AllRows(roots))
                node.ClearSearchLook();
            return new List<TreeNode>();
        }

        private static List<TreeNode> ShowLooks(string text, IEnumerable<TreeNode> roots)
        {
            var found = new List<TreeNode>();
            ShowLooksOf(text, roots, found);
            return found;
        }

        // Adds the matches in tree order and returns true when a node of the list or a node below it matches.
        private static bool ShowLooksOf(string text, IEnumerable<TreeNode> nodes, List<TreeNode> found)
        {
            var hasMatch = false;
            foreach (var node in nodes)
            {
                node.ShowSearchLook(text);
                if (node.IsSearchMatch)
                    found.Add(node);

                node.HasSearchMatchBelow = ShowLooksOf(text, node.Children, found);
                if (node.IsSearchMatch || node.HasSearchMatchBelow)
                    hasMatch = true;
            }

            return hasMatch;
        }

        public TreeNode? MoveToMatch(TreeNode? cursor, int step)
        {
            var count = matches.Count;
            if (count == 0 || step == 0)
                return null;

            var index = cursor == null ? -1 : TreeWalk.IndexOf(matches, cursor);
            var next = index < 0 ? (step > 0 ? 0 : count - 1) : ((index + step) % count + count) % count;
            return matches[next];
        }
    }
}
