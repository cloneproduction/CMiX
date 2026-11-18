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
        private readonly Action<TreeNode?, Func<IReadOnlyList<TreeNode>>> closeRowsInView;

        // A row that the user opens or closes leaves the set and keeps its state.
        private readonly HashSet<TreeNode> openedBySearch = new();

        private bool isSearchChangingRows;

        private TreeNode? keptCursor;

        private string text = string.Empty;
        private IReadOnlyList<TreeNode> matches = Array.Empty<TreeNode>();

        public TreeSearch(Action<TreeNode?, Func<IReadOnlyList<TreeNode>>> closeRowsInView)
        {
            this.closeRowsInView = closeRowsInView;
        }

        public IReadOnlyList<TreeNode> Matches => matches;

        // An empty text before and now has nothing to change.
        public bool IsEmpty => matches.Count == 0 && openedBySearch.Count == 0;

        public SearchChange Refresh(string text, IEnumerable<TreeNode> roots)
        {
            this.text = text;
            var found = new List<TreeNode>();
            foreach (var node in TreeWalk.AllRows(roots))
            {
                if (text.Length == 0)
                {
                    node.ClearSearchLook();
                    continue;
                }

                var look = new SearchLook(node.MatchesSearch(text));
                node.ShowSearchLook(look);
                if (look.IsMatch)
                    found.Add(node);
            }

            var changed = !found.SequenceEqual(matches);
            var countChanged = changed && found.Count != matches.Count;
            if (changed)
                matches = found;

            return new SearchChange(changed, countChanged);
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
                closeRowsInView(selected, () =>
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
