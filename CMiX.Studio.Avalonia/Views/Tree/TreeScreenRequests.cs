// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    internal sealed class TreeScreenRequests
    {
        private readonly TreeViewContext context;
        private readonly TreeMenus menus;
        private readonly TreeRowReveal reveal;
        private readonly TreeGrab grab;
        private readonly TreeScrollAnchor scrollAnchor;
        private readonly TreePointerInput pointerInput;
        private readonly Func<bool> isActiveView;
        private readonly Action<Action, DispatcherPriority> postWhileAttached;

        internal TreeScreenRequests(TreeViewContext context, TreeMenus menus, TreeRowReveal reveal, TreeGrab grab, TreeScrollAnchor scrollAnchor,
            TreePointerInput pointerInput, Func<bool> isActiveView, Action<Action, DispatcherPriority> postWhileAttached)
        {
            this.context = context;
            this.menus = menus;
            this.reveal = reveal;
            this.grab = grab;
            this.scrollAnchor = scrollAnchor;
            this.pointerInput = pointerInput;
            this.isActiveView = isActiveView;
            this.postWhileAttached = postWhileAttached;
        }

        internal void Watch(ProjectTree tree)
        {
            tree.RenameRequested += OnRenameRequested;
            tree.AddRequested += OnAddRequested;
            tree.MenuRequested += OnMenuRequested;
            tree.CollapseOthersRequested += OnCollapseOthersRequested;
            tree.RevealRequested += OnRevealRequested;
            tree.GrabRequested += OnGrabRequested;
            tree.TreeFocusRequested += OnTreeFocusRequested;
        }

        internal void Unwatch(ProjectTree tree)
        {
            tree.RenameRequested -= OnRenameRequested;
            tree.AddRequested -= OnAddRequested;
            tree.MenuRequested -= OnMenuRequested;
            tree.CollapseOthersRequested -= OnCollapseOthersRequested;
            tree.RevealRequested -= OnRevealRequested;
            tree.GrabRequested -= OnGrabRequested;
            tree.TreeFocusRequested -= OnTreeFocusRequested;
        }

        private void AnswerWhenActive(Action<ProjectTree> answer)
        {
            if (isActiveView() && context.Tree is { } tree)
                answer(tree);
        }

        private void OnAddRequested(object? sender, EventArgs e) => AnswerWhenActive(menus.OpenAddAtTarget);

        private void OnMenuRequested(object? sender, EventArgs e) => AnswerWhenActive(menus.OpenMenuAtTarget);

        private void OnGrabRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => GrabRow(node));

        private void GrabRow(TreeNode node)
        {
            if (reveal.ShownItemOf(node) != null)
                grab.Start(node, pointerInput.LastPointer);
        }

        // The first row in view cannot be the anchor, because it can close away under a parent.
        private void OnCollapseOthersRequested(object? sender, EventArgs e) => AnswerWhenActive(CloseOtherRows);

        private void CloseOtherRows(ProjectTree tree)
        {
            if (tree.TargetNode is not { } target)
                return;

            var anchor = scrollAnchor.CaptureRow(target);
            var closed = tree.CloseOtherRows();
            if (anchor != null)
            {
                scrollAnchor.RestoreAnchor(anchor);
                scrollAnchor.HoldWhileClosing(anchor, closed);
                return;
            }

            postWhileAttached(() => reveal.RevealRow(target), DispatcherPriority.Loaded);
        }

        private void OnRevealRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => reveal.RevealRowToMiddle(node));

        internal void CloseRowsInView(TreeNode? node, Func<IReadOnlyList<TreeNode>> close)
        {
            var anchor = (node != null ? scrollAnchor.CaptureRow(node) : null) ?? scrollAnchor.CaptureAnchor(null);
            var closed = close();
            scrollAnchor.RestoreAnchor(anchor);
            scrollAnchor.HoldWhileClosing(anchor, closed);
        }

        private void OnRenameRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => RenameRow(node));

        private void RenameRow(TreeNode node)
        {
            if (context.RowIndex.FindItem(node) is not { } item || TreeRows.RowBorderOf(item) is not { } row)
                return;

            if (TreeRows.NameBoxOf(row) is { } box)
                box.IsEditing = true;
        }

        private void OnTreeFocusRequested(object? sender, EventArgs e) => AnswerWhenActive(FocusTree);

        private void FocusTree(ProjectTree tree)
        {
            var node = tree.TargetNode ?? tree.VisibleNodes().FirstOrDefault();
            if (node != null && context.RowIndex.FindItem(node) is { } item && item.Focus(NavigationMethod.Tab))
                return;

            context.TreeView.Focus();
        }
    }
}
