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
    internal sealed record TreeRequestParts(TreeMenus Menus, TreeRowReveal Reveal, TreeGrab Grab, TreeScrollAnchor ScrollAnchor);

    internal sealed record TreeRequestInputs(Func<bool> IsActiveView, Func<Point> LastPointer);

    internal sealed class TreeScreenRequests
    {
        private readonly TreeViewContext context;
        private readonly TreeRequestParts parts;
        private readonly TreeRequestInputs inputs;
        private readonly Action<Action, DispatcherPriority> postWhileAttached;

        internal TreeScreenRequests(TreeViewContext context, TreeRequestParts parts, TreeRequestInputs inputs,
            Action<Action, DispatcherPriority> postWhileAttached)
        {
            this.context = context;
            this.parts = parts;
            this.inputs = inputs;
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
            if (inputs.IsActiveView() && context.Tree is { } tree)
                answer(tree);
        }

        private void OnAddRequested(object? sender, EventArgs e) => AnswerWhenActive(parts.Menus.OpenAddAtTarget);

        private void OnMenuRequested(object? sender, EventArgs e) => AnswerWhenActive(parts.Menus.OpenMenuAtTarget);

        private void OnGrabRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => GrabRow(node));

        private void GrabRow(TreeNode node)
        {
            if (parts.Reveal.ShownItemOf(node) != null)
                parts.Grab.Start(node, inputs.LastPointer());
        }

        // The first row in view cannot be the anchor, because it can close away under a parent.
        private void OnCollapseOthersRequested(object? sender, EventArgs e) => AnswerWhenActive(CloseOtherRows);

        private void CloseOtherRows(ProjectTree tree)
        {
            if (tree.TargetNode is not { } target)
                return;

            var anchor = parts.ScrollAnchor.CaptureRow(target);
            var closed = tree.CloseOtherRows();
            if (anchor != null)
            {
                parts.ScrollAnchor.RestoreAnchor(anchor);
                parts.ScrollAnchor.HoldWhileClosing(anchor, closed);
                return;
            }

            postWhileAttached(() => parts.Reveal.RevealRow(target), DispatcherPriority.Loaded);
        }

        private void OnRevealRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => parts.Reveal.RevealRowToMiddle(node));

        internal void CloseRowsInView(TreeNode? node, Func<IReadOnlyList<TreeNode>> close)
        {
            var anchor = (node != null ? parts.ScrollAnchor.CaptureRow(node) : null) ?? parts.ScrollAnchor.CaptureAnchor(null);
            var closed = close();
            parts.ScrollAnchor.RestoreAnchor(anchor);
            parts.ScrollAnchor.HoldWhileClosing(anchor, closed);
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
