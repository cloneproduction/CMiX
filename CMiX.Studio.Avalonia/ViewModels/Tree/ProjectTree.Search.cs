// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

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

            SearchText = string.Empty;
        }

        [RelayCommand]
        public void LeaveSearch()
        {
            if (IsDragging)
                Views.CancelDrag();
            else if (SearchText.Length > 0)
                SearchText = string.Empty;
            else
                FocusTree();
        }

        [RelayCommand(CanExecute = nameof(CanRunSearchKey))]
        public void MoveCursorToNextMatch()
        {
            if (!IsDragging)
                MoveCursorTo(search.MoveToMatch(CursorNode, 1));
        }

        [RelayCommand(CanExecute = nameof(CanRunSearchKey))]
        public void MoveCursorToPreviousMatch()
        {
            if (!IsDragging)
                MoveCursorTo(search.MoveToMatch(CursorNode, -1));
        }

        private bool CanRunSearchKey() => !IsDragging;

        private void MoveCursorTo(TreeNode? match)
        {
            if (match == null)
                return;

            CursorNode = match;
            RevealRequested?.Invoke(this, match);
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

            if (matchesChanged && searchActive)
                MoveCursorTo(search.Matches.FirstOrDefault());
            if (wasActive && !searchActive)
                KeepTargetsInView();
        }

        // The own open state shows again: the parents of the selected row and of the cursor row open, and the view shows the cursor row.
        private void KeepTargetsInView()
        {
            if (selectedNode != null)
                openState.OpenParents(selectedNode);
            if (CursorNode is not { } cursor)
                return;

            openState.OpenParents(cursor);
            RevealRequested?.Invoke(this, cursor);
        }

        // A row that closes over the cursor gives the cursor to the highest closed parent, so the cursor never hides.
        void TreeNodeHost.RowExpandedChanged(TreeNode node)
        {
            if (!node.IsExpanded && CursorNode is { } cursor && !ReferenceEquals(cursor, node) && cursor.IsOrIsUnder(node))
                CursorNode = TreeWalk.ParentsOf(cursor).LastOrDefault(parent => !parent.IsExpanded) ?? node;
        }
    }
}
