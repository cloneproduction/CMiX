// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed partial class ProjectTree
    {
        private string searchText = string.Empty;

        // True while the text of the search is not empty, as at the last refresh of the search.
        private bool searchActive;

        // The text of the search of the tree. A row matches when its name or its kind holds the text (see TreeNode.MatchesSearch).
        // An empty text matches no row and dims no row.
        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value ?? string.Empty))
                    RefreshSearch();
            }
        }

        // True while the search filters the rows: the text holds more than spaces, and the rows that do not match are dimmed.
        public bool IsSearchActive => searchActive;

        // The rows that match the search, in the order of the tree from the top.
        public IReadOnlyList<TreeNode> SearchMatches => search.Matches;

        public int SearchMatchCount => search.Matches.Count;

        // Clears the text of the search. The cursor row stays, and its parents stay open (see SyncSearchRows). The focus does not move.
        // Nothing changes during a drag or with no active search.
        public void ClearSearchText()
        {
            if (!IsSearchActive || IsDragging)
                return;

            search.KeepCursorRow(CursorNode);
            SearchText = string.Empty;
        }

        // Moves the cursor to the next match of the search in the order of the tree, or to the match before it with a negative step.
        // The cursor goes round at the ends (see TreeSearch.MoveToMatch). The view brings the row into view (RevealRequested). The
        // selection does not change: no undo step and no wire message. Nothing changes during a drag.
        public void MoveCursorToMatch(int step)
        {
            if (IsDragging)
                return;

            if (search.MoveToMatch(CursorNode, step) is not { } node)
                return;

            CursorNode = node;
            RevealRequested?.Invoke(this, node);
        }

        // Sets the search state of every row and finds the matches again. It runs when the text changes, when a row gets a new
        // name, and when a list of the tree changes. With an empty text before and now, there is nothing to change. A search that found
        // nothing still dimmed every row, so its end must run to take the dim away.
        internal void RefreshSearch()
        {
            var text = searchText.Trim();
            var wasActive = searchActive;
            searchActive = text.Length > 0;
            if (searchActive != wasActive)
            {
                OnPropertyChanged(nameof(IsSearchActive));
                ClearSearchCommand.NotifyCanExecuteChanged();
            }

            if (!searchActive && !wasActive && search.IsEmpty)
                return;

            var change = search.Refresh(text, Roots);
            if (change.MatchesChanged)
            {
                OnPropertyChanged(nameof(SearchMatches));
                if (change.CountChanged)
                    OnPropertyChanged(nameof(SearchMatchCount));
            }

            // The rows change only when the matches change, or when the search ends: a row that the user closes during the search
            // stays closed until the matches change.
            if (change.MatchesChanged || text.Length == 0)
                SyncSearchRows();
        }

        // There is no search during a drag: EndDrag does it once the drag is over.
        private void SyncSearchRows()
        {
            if (IsDragging)
                return;

            search.SyncRows(CursorNode, selectedNode);
        }

        // A row opens or closes. A change that the search did not make leaves the search (see TreeSearch.ReleaseChangedRow).
        // A row that closes over the cursor takes the cursor, so the cursor is never on a hidden row. The cursor goes to the highest
        // closed parent, the one row of its parents that shows. The selection does not change.
        internal void OnRowExpandedChanged(TreeNode node)
        {
            search.ReleaseChangedRow(node);

            if (!node.IsExpanded && CursorNode is { } cursor && !ReferenceEquals(cursor, node) && cursor.IsOrIsUnder(node))
                CursorNode = TreeWalk.ParentsOf(cursor).LastOrDefault(parent => !parent.IsExpanded) ?? node;
        }
    }
}
