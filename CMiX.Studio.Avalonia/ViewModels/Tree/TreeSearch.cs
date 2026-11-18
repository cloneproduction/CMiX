// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The search of the tree: the rows that match the text, and the rows that the search opened to show them.
    // The hub (ProjectTree) owns the text, the cursor and the drag, and passes them in. The search closes rows through the
    // active view (closeRowsInView), so the view keeps the rows still.
    public sealed class TreeSearch
    {
        private readonly Action<TreeNode?, Func<IReadOnlyList<TreeNode>>> closeRowsInView;

        // The rows that the search opened from closed, and that the user did not open or close since. The search closes them again
        // when they lead to no match any more, and at its end. A row that the user opens or closes leaves the list and keeps the
        // state of the user. A row that the search never touched, a row added during the search too, keeps its state.
        private readonly HashSet<TreeNode> opened = new();

        // True while the search opens or closes a row, so the change does not count as a change of the user.
        private bool setsRows;

        // The row that MoveToMatch last moved the cursor to, or the cursor row at ClearSearch. See SyncRows.
        private TreeNode? keptCursor;

        private string text = string.Empty;
        private IReadOnlyList<TreeNode> matches = Array.Empty<TreeNode>();

        public TreeSearch(Action<TreeNode?, Func<IReadOnlyList<TreeNode>>> closeRowsInView)
        {
            this.closeRowsInView = closeRowsInView;
        }

        // The rows that match the search, in the order of the tree from the top.
        public IReadOnlyList<TreeNode> Matches => matches;

        // True when the search holds no match and opened no row: an empty text before and now has nothing to change.
        public bool IsEmpty => matches.Count == 0 && opened.Count == 0;

        // Sets the search state of every row and finds the matches again. The text is trimmed.
        public SearchChange Refresh(string text, IEnumerable<TreeNode> roots)
        {
            this.text = text;
            var found = new List<TreeNode>();
            foreach (var node in TreeWalk.AllRows(roots))
            {
                var match = text.Length > 0 && node.MatchesSearch(text);
                node.SetSearchState(text.Length > 0, match);
                if (match)
                    found.Add(node);
            }

            var changed = !found.SequenceEqual(matches);
            var countChanged = changed && found.Count != matches.Count;
            if (changed)
                matches = found;

            return new SearchChange(changed, countChanged);
        }

        // Opens the parents of every match, so every match is in sight, and closes the rows that the search opened and that lead to no
        // match any more. At the end of the search, the parents of the selected row stay open, so the user can go on with it: they
        // leave the list. The same applies to the cursor row while MoveToMatch or KeepCursorRow put it in keptCursor. These rows keep
        // their state. The other rows that the search opened close.
        public void SyncRows(TreeNode? cursor, TreeNode? selected)
        {
            if (text.Length == 0)
                ReleaseKeptRows(cursor, selected);

            var needed = new HashSet<TreeNode>(matches.SelectMany(TreeWalk.ParentsOf));

            var closing = opened.Where(node => !needed.Contains(node)).ToList();
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

        // The next match after the cursor in the order of the tree, or the match before it with a negative step. The matches go round
        // at the ends. No cursor, or a cursor on a row that does not match: the first match, or the last one going back. The parents
        // of the match open, so its row shows. Returns null when there is no match to go to.
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

        // The end of the search keeps the cursor row and its parents open while the cursor stays on it.
        public void KeepCursorRow(TreeNode? cursor) => keptCursor = cursor;

        // The row leaves the search: the end of the search does not close it.
        public void Release(TreeNode node) => opened.Remove(node);

        // A row opened or closed. A change that the search did not make is a change of the user (or of the drag): the row leaves
        // the search, so the search does not change it back.
        public void ReleaseChangedRow(TreeNode node)
        {
            if (!setsRows)
                opened.Remove(node);
        }

        private void ReleaseKeptRows(TreeNode? cursor, TreeNode? selected)
        {
            var matchCursor = ReferenceEquals(cursor, keptCursor) ? cursor : null;
            keptCursor = null;
            foreach (var kept in new[] { selected, matchCursor })
                if (kept != null)
                    foreach (var row in TreeWalk.ParentsOf(kept).Append(kept))
                        opened.Remove(row);
        }

        private void OpenForSearch(TreeNode node)
        {
            if (node.IsExpanded)
                return;

            ChangeRowAsSearch(() => node.IsExpanded = true);
            opened.Add(node);
        }

        private void CloseForSearch(TreeNode node)
        {
            ChangeRowAsSearch(() => node.IsExpanded = false);
            opened.Remove(node);
        }

        private void ChangeRowAsSearch(Action change)
        {
            setsRows = true;
            try
            {
                change();
            }
            finally
            {
                setsRows = false;
            }
        }
    }
}
