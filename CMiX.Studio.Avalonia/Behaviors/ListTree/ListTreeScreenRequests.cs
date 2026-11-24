// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The requests of the tree that the view answers while it is the active view.
    internal sealed class ListTreeScreenRequests : ListTreeBehavior
    {
        private ProjectTree? tree;

        private PanelListTreeRows Rows => Context.Rows;

        private ListTreeRowReveal Reveal => Part<ListTreeRowReveal>();

        private ListTreeScrollAnchor ScrollAnchor => Part<ListTreeScrollAnchor>();

        protected override void OnWatch(ProjectTree watched)
        {
            tree = watched;
            watched.TreeFocusRequested += OnTreeFocusRequested;
            watched.CollapseOthersRequested += OnCollapseOthersRequested;
            watched.RenameRequested += OnRenameRequested;
            watched.AddRequested += OnAddRequested;
            watched.MenuRequested += OnMenuRequested;
            watched.RevealRequested += OnRevealRequested;
            watched.GrabRequested += OnGrabRequested;
        }

        protected override void OnUnwatch()
        {
            if (tree == null)
                return;

            tree.TreeFocusRequested -= OnTreeFocusRequested;
            tree.CollapseOthersRequested -= OnCollapseOthersRequested;
            tree.RenameRequested -= OnRenameRequested;
            tree.AddRequested -= OnAddRequested;
            tree.MenuRequested -= OnMenuRequested;
            tree.RevealRequested -= OnRevealRequested;
            tree.GrabRequested -= OnGrabRequested;
            ScrollAnchor.EndHold();
            tree = null;
        }

        private void AnswerWhenActive(Action<ProjectTree> answer)
        {
            if (Context.Owner.IsActiveView && tree is { } active)
                answer(active);
        }

        private void OnAddRequested(object? sender, EventArgs e) => AnswerWhenActive(Part<ListTreeMenus>().OpenAddAtTarget);

        private void OnMenuRequested(object? sender, EventArgs e) => AnswerWhenActive(Part<ListTreeMenus>().OpenMenuAtTarget);

        private void OnRevealRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => Reveal.RevealRowToMiddle(node));

        private void OnGrabRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => Part<ListTreeGrab>().Start(node));

        private void OnTreeFocusRequested(object? sender, EventArgs e) => AnswerWhenActive(FocusTree);

        private void FocusTree(ProjectTree active)
        {
            var node = active.TargetNode ?? active.VisibleNodes().FirstOrDefault();
            if (node != null && Rows.RowOf(node) is { } row && row.Focus(NavigationMethod.Tab))
                return;

            Rows.Children.OfType<RowListTree>().FirstOrDefault()?.Focus(NavigationMethod.Tab);
        }

        private void OnRenameRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => Rows.RowOf(node)?.StartRename());

        // The first row in view cannot be the anchor, because it can close away under a parent.
        private void OnCollapseOthersRequested(object? sender, EventArgs e) => AnswerWhenActive(CloseOtherRows);

        private void CloseOtherRows(ProjectTree active)
        {
            if (active.TargetNode is not { } target)
                return;

            if (ScrollAnchor.CaptureRow(target) is { } anchor)
            {
                ScrollAnchor.KeepStill(anchor, active.CloseOtherRows);
                return;
            }

            active.CloseOtherRows();
            Reveal.RevealRow(target);
        }

        // The hook CloseRowsInView of the view: the row of the node stays still when it is in view, else the first row in view.
        public void CloseRowsInView(TreeNode? node, Func<IReadOnlyList<TreeNode>> close)
        {
            var anchor = (node != null ? ScrollAnchor.CaptureRow(node) : null) ?? ScrollAnchor.CaptureFirstRowInView();
            ScrollAnchor.KeepStill(anchor, close);
        }
    }
}
