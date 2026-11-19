// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed partial class ProjectTree
    {
        private string searchText = string.Empty;

        private bool searchActive;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value ?? string.Empty))
                    RefreshSearch();
            }
        }

        public bool IsSearchActive => searchActive;

        public IReadOnlyList<TreeNode> SearchMatches => search.Matches;

        public int SearchMatchCount => search.Matches.Count;

        public void ClearSearchText()
        {
            if (!IsSearchActive || IsDragging)
                return;

            search.KeepCursorRow(CursorNode);
            SearchText = string.Empty;
        }

        public void MoveCursorToMatch(int step)
        {
            if (IsDragging)
                return;

            if (search.MoveToMatch(CursorNode, step) is not { } node)
                return;

            CursorNode = node;
            RevealRequested?.Invoke(this, node);
        }

        // A search that found nothing still dimmed every row, so its end must run.
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

            var countBefore = search.Matches.Count;
            var matchesChanged = search.Refresh(text, Roots);
            if (matchesChanged)
            {
                OnPropertyChanged(nameof(SearchMatches));
                if (search.Matches.Count != countBefore)
                    OnPropertyChanged(nameof(SearchMatchCount));
            }

            // A row that the user closes during the search stays closed until the matches change.
            if (matchesChanged || text.Length == 0)
                SyncSearchRows();
        }

        // EndDrag runs the search once the drag is over.
        private void SyncSearchRows()
        {
            if (IsDragging)
                return;

            search.SyncRows(CursorNode, selectedNode);
        }

        // A row that closes over the cursor gives the cursor to the highest closed parent, so the cursor never hides.
        internal void RowExpandedChanged(TreeNode node)
        {
            search.ReleaseChangedRow(node);

            if (!node.IsExpanded && CursorNode is { } cursor && !ReferenceEquals(cursor, node) && cursor.IsOrIsUnder(node))
                CursorNode = TreeWalk.ParentsOf(cursor).LastOrDefault(parent => !parent.IsExpanded) ?? node;
        }
    }
}
