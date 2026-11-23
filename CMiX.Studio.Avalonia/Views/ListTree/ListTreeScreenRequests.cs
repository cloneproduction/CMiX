// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using CMiX.Studio.Avalonia.Behaviors.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The requests of the tree that the new tree answers while it is the active view, as TreeScreenRequests answers them for the old tree.
    internal sealed class ListTreeScreenRequests
    {
        private readonly PanelListTreeRows rows;
        private readonly ListTreeMenus menus;
        private readonly ListTreeRowReveal reveal;
        private readonly ListTreeScrollAnchor scrollAnchor;
        private readonly Func<bool> isActiveView;

        private ProjectTree? tree;

        public ListTreeScreenRequests(PanelListTreeRows rows, ListTreeMenus menus, ListTreeRowReveal reveal, ListTreeScrollAnchor scrollAnchor,
            Func<bool> isActiveView)
        {
            this.rows = rows;
            this.menus = menus;
            this.reveal = reveal;
            this.scrollAnchor = scrollAnchor;
            this.isActiveView = isActiveView;
        }

        public void Watch(ProjectTree watched)
        {
            tree = watched;
            watched.TreeFocusRequested += OnTreeFocusRequested;
            watched.CollapseOthersRequested += OnCollapseOthersRequested;
            watched.RenameRequested += OnRenameRequested;
            watched.AddRequested += OnAddRequested;
            watched.MenuRequested += OnMenuRequested;
            watched.RevealRequested += OnRevealRequested;
        }

        public void Unwatch()
        {
            if (tree == null)
                return;

            tree.TreeFocusRequested -= OnTreeFocusRequested;
            tree.CollapseOthersRequested -= OnCollapseOthersRequested;
            tree.RenameRequested -= OnRenameRequested;
            tree.AddRequested -= OnAddRequested;
            tree.MenuRequested -= OnMenuRequested;
            tree.RevealRequested -= OnRevealRequested;
            scrollAnchor.EndHold();
            tree = null;
        }

        private void AnswerWhenActive(Action<ProjectTree> answer)
        {
            if (isActiveView() && tree is { } active)
                answer(active);
        }

        private void OnAddRequested(object? sender, EventArgs e) => AnswerWhenActive(menus.OpenAddAtTarget);

        private void OnMenuRequested(object? sender, EventArgs e) => AnswerWhenActive(menus.OpenMenuAtTarget);

        private void OnRevealRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => reveal.RevealRowToMiddle(node));

        private void OnTreeFocusRequested(object? sender, EventArgs e) => AnswerWhenActive(FocusTree);

        private void FocusTree(ProjectTree active)
        {
            var node = active.TargetNode ?? active.VisibleNodes().FirstOrDefault();
            if (node != null && rows.RowOf(node) is { } row && row.Focus(NavigationMethod.Tab))
                return;

            rows.Children.OfType<RowListTree>().FirstOrDefault()?.Focus(NavigationMethod.Tab);
        }

        private void OnRenameRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => rows.RowOf(node)?.StartRename());

        // The first row in view cannot be the anchor, because it can close away under a parent.
        private void OnCollapseOthersRequested(object? sender, EventArgs e) => AnswerWhenActive(CloseOtherRows);

        private void CloseOtherRows(ProjectTree active)
        {
            if (active.TargetNode is not { } target)
                return;

            if (scrollAnchor.CaptureRow(target) is { } anchor)
            {
                scrollAnchor.KeepStill(anchor, active.CloseOtherRows);
                return;
            }

            active.CloseOtherRows();
            reveal.RevealRow(target);
        }

        // The hook CloseRowsInView of the view: the row of the node stays still when it is in view, else the first row in view.
        public void CloseRowsInView(TreeNode? node, Func<IReadOnlyList<TreeNode>> close)
        {
            var anchor = (node != null ? scrollAnchor.CaptureRow(node) : null) ?? scrollAnchor.CaptureFirstRowInView();
            scrollAnchor.KeepStill(anchor, close);
        }
    }
}
