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

        public bool IsEmpty => matches.Count == 0;

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
            var rows = TreeWalk.AllRows(roots);
            var found = new List<TreeNode>();
            foreach (var node in rows)
            {
                node.ShowSearchLook(text);
                if (node.IsSearchMatch)
                    found.Add(node);
            }

            var parents = new HashSet<TreeNode>(found.SelectMany(TreeWalk.ParentsOf));
            foreach (var node in rows)
                node.HasSearchMatchBelow = parents.Contains(node);
            return found;
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
