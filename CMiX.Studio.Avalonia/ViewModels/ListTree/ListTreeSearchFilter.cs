// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // Which rows the view shows and which it shows open, with or without a search.
    public sealed class ListTreeSearchFilter
    {
        private readonly bool showsOnlyMatches;

        private ListTreeSearchFilter(bool showsOnlyMatches)
        {
            this.showsOnlyMatches = showsOnlyMatches;
        }

        public static ListTreeSearchFilter None { get; } = new(false);

        public static ListTreeSearchFilter Matches { get; } = new(true);

        public bool Shows(TreeNode node)
        {
            if (!showsOnlyMatches)
                return true;

            return node.IsSearchMatch || node.HasSearchMatchBelow;
        }

        // A match keeps its own open state.
        public bool ShowsOpen(TreeNode node)
        {
            if (!showsOnlyMatches)
                return node.IsExpanded;

            return node.HasSearchMatchBelow || node.IsExpanded;
        }
    }
}
