// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.String;

namespace CMiX.Studio.Avalonia.Views
{
    // The parts of the view that answer the requests of the tree.
    internal sealed record TreeRequestParts(TreeMenus Menus, TreeRowReveal Reveal, TreeGrab Grab, TreeScrollAnchor ScrollAnchor);

    // What the answers read from the view: whether this view is the active one, and the last place of the pointer over the tree.
    internal sealed record TreeRequestInputs(Func<bool> IsActiveView, Func<Point> LastPointer);

    // The answers of one view to the requests of the tree (see ProjectTree.Requests). Only the active view answers (see
    // ProjectTree.IsActiveView): both side panels can show the tree, and one request must have one answer.
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

        // Tree.Grab: the grab of the target row starts at the last place of the pointer over the tree (see TreeGrab). A row that is
        // not shown (its parent is collapsed) is not grabbed.
        private void OnGrabRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => GrabRow(node));

        private void GrabRow(TreeNode node)
        {
            if (parts.Reveal.ShownItemOf(node) != null)
                parts.Grab.Start(node, inputs.LastPointer());
        }

        // Tree.CollapseOthers: every row closes but the path to the target row. A target row in view stays at its place on the screen,
        // also while the rows close (CollapsePanel closes over a few frames). The first row in view can not be the anchor: it can close
        // away under a parent. A target row out of view is brought into view after the close.
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

        // Rows of the search close (see ProjectTree.CloseRowsInView). The tree calls the active view. The row of the node stays at its
        // place on the screen when it is in view, else the first row in view does, also while the rows close.
        internal void CloseRowsInView(TreeNode? node, Func<IReadOnlyList<TreeNode>> close)
        {
            var anchor = (node != null ? parts.ScrollAnchor.CaptureRow(node) : null) ?? parts.ScrollAnchor.CaptureAnchor(null);
            var closed = close();
            parts.ScrollAnchor.RestoreAnchor(anchor);
            parts.ScrollAnchor.HoldWhileClosing(anchor, closed);
        }

        // Starts the rename of the row of the node: the name box of the row goes to edit mode.
        private void OnRenameRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => RenameRow(node));

        private void RenameRow(TreeNode node)
        {
            if (context.RowIndex.FindItem(node) is not { } item || TreeRows.RowBorderOf(item) is not { } row)
                return;

            if (row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault() is { } box)
                box.IsEditing = true;
        }

        // The focus goes to a row: the target row, else the first row (see ProjectTree.FocusTree).
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
