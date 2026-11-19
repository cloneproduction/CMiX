// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The search closes rows through the active view, so the view keeps the rows still.
    public sealed class TreeSearch
    {
        private readonly IRowCloser rowCloser;

        // A row that the user opens or closes leaves the set and keeps its state.
        private readonly HashSet<TreeNode> openedBySearch = new();

        private bool isSearchChangingRows;

        private TreeNode? keptCursor;

        private string text = string.Empty;
        private IReadOnlyList<TreeNode> matches = Array.Empty<TreeNode>();

        internal TreeSearch(IRowCloser rowCloser)
        {
            this.rowCloser = rowCloser;
        }

        public IReadOnlyList<TreeNode> Matches => matches;

        // An empty text before and now has nothing to change.
        public bool IsEmpty => matches.Count == 0 && openedBySearch.Count == 0;

        public bool Refresh(string text, IEnumerable<TreeNode> roots)
        {
            this.text = text;
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
            foreach (var node in TreeWalk.AllRows(roots))
            {
                node.ShowSearchLook(text);
                if (node.IsSearchMatch)
                    found.Add(node);
            }
            return found;
        }

        // At the end of the search, the parents of the selected row and of the kept cursor stay open.
        // The other rows that the search opened close.
        public void SyncRows(TreeNode? cursor, TreeNode? selected)
        {
            if (text.Length == 0)
                ReleaseKeptRows(cursor, selected);

            var needed = new HashSet<TreeNode>(matches.SelectMany(TreeWalk.ParentsOf));

            var closing = openedBySearch.Where(node => !needed.Contains(node)).ToList();
            if (closing.Count > 0)
                rowCloser.CloseRowsInView(selected, () =>
                {
                    foreach (var node in closing)
                        CloseForSearch(node);
                    return closing;
                });

            foreach (var node in needed)
                OpenForSearch(node);
        }

        public TreeNode? MoveToMatch(TreeNode? cursor, int step)
        {
            var count = matches.Count;
            if (count == 0 || step == 0)
                return null;

            var index = cursor == null ? -1 : TreeWalk.IndexOf(matches, cursor);
            var next = index < 0 ? (step > 0 ? 0 : count - 1) : ((index + step) % count + count) % count;
            var node = matches[next];
            foreach (var parent in TreeWalk.ParentsOf(node))
                OpenForSearch(parent);

            keptCursor = node;
            return node;
        }

        public void KeepCursorRow(TreeNode? cursor) => keptCursor = cursor;

        public void Release(TreeNode node) => openedBySearch.Remove(node);

        // A change that the search did not make is a change of the user, so the row leaves the search.
        public void ReleaseChangedRow(TreeNode node)
        {
            if (!isSearchChangingRows)
                openedBySearch.Remove(node);
        }

        private void ReleaseKeptRows(TreeNode? cursor, TreeNode? selected)
        {
            var matchCursor = ReferenceEquals(cursor, keptCursor) ? cursor : null;
            keptCursor = null;
            foreach (var kept in new[] { selected, matchCursor })
                if (kept != null)
                    foreach (var row in TreeWalk.ParentsOf(kept).Append(kept))
                        openedBySearch.Remove(row);
        }

        private void OpenForSearch(TreeNode node)
        {
            if (node.IsExpanded)
                return;

            ChangeRowAsSearch(() => node.IsExpanded = true);
            openedBySearch.Add(node);
        }

        private void CloseForSearch(TreeNode node)
        {
            ChangeRowAsSearch(() => node.IsExpanded = false);
            openedBySearch.Remove(node);
        }

        private void ChangeRowAsSearch(Action change)
        {
            isSearchChangingRows = true;
            try
            {
                change();
            }
            finally
            {
                isSearchChangingRows = false;
            }
        }
    }
}
