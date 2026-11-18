// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Panels;

namespace CMiX.Studio.Avalonia.Views
{
    // The tree does not scroll by itself when the user clicks a row: a click on a row that is at the edge of the view would scroll to
    // show the whole block of the row, and the second click of a double click would land on another row. A row that a program or a
    // key selects is brought into view here, and only its own row, not the rows under it.
    internal sealed class TreeRowReveal
    {
        private readonly TreeViewContext context;
        private readonly Func<bool> isTreeFocused;

        // The row border that BringRowIntoView brings into view.
        private Visual? revealTarget;

        // The reveal that runs now. A new reveal ends the one before.
        private int revealId;

        internal TreeRowReveal(TreeViewContext context, Func<bool> isTreeFocused)
        {
            this.context = context;
            this.isTreeFocused = isTreeFocused;
        }

        internal void Attach(TreeView treeView) => treeView.TemplateApplied += OnTemplateApplied;

        private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e) =>
            e.NameScope.Find<ItemsPresenter>("PART_ItemsPresenter")?.AddHandler(Control.RequestBringIntoViewEvent, OnRequestBringIntoView);

        // A row asks to be brought into view when it gets the focus or the selection. That request scrolls to the whole block of the
        // row, so it stops here, under the scroller of the tree. Only the request of BringRowIntoView goes through.
        private void OnRequestBringIntoView(object? sender, RequestBringIntoViewEventArgs e)
        {
            if (!ReferenceEquals(e.TargetObject, revealTarget))
                e.Handled = true;
        }

        internal void RevealRow(TreeNode node) => BringRowIntoView(node, border => border.BringIntoView());

        // The item of the row of the node, brought into view, so that a popup can open on it. Null when the row is not shown.
        internal TreeViewItem? ShownItemOf(TreeNode node)
        {
            if (context.RowIndex.FindItem(node) is not { } item)
                return null;

            RevealRow(node);
            context.TreeView.UpdateLayout();
            return item;
        }

        // Tree.Reveal: the row goes to the middle of the view (see ProjectTree.RevealSelected). The parents of the row open over a
        // few frames (CollapsePanel), so the tree is not yet high enough at the first frame. The row is put back in the middle once in each
        // frame until every parent is fully open. While the tree has the focus, the row takes it, as with the arrow keys.
        internal void RevealRowToMiddle(TreeNode node)
        {
            var id = ++revealId;
            Dispatcher.UIThread.Post(() =>
            {
                if (id != revealId || context.RowIndex.FindItem(node) is not { } item)
                    return;

                if (isTreeFocused())
                    item.Focus(NavigationMethod.Tab);

                BringRowToMiddle(node);
                if (TopLevel.GetTopLevel(context.TreeView) is not { } top)
                    return;

                var frames = 0;
                void Keep(TimeSpan _)
                {
                    if (id != revealId)
                        return;

                    BringRowToMiddle(node);
                    if (IsOpening(item) && ++frames < MaxRevealFrames)
                        top.RequestAnimationFrame(Keep);
                }

                if (IsOpening(item))
                    top.RequestAnimationFrame(Keep);
            }, DispatcherPriority.Loaded);
        }

        // Ends the reveal that runs. Its next step and its next frame then do nothing. The view calls it when it leaves the window.
        internal void EndReveal() => revealId++;

        // True while a parent of the row still opens.
        private static bool IsOpening(TreeViewItem item) =>
            item.GetVisualAncestors().OfType<CollapsePanel>().Any(panel => panel.IsExpanded && panel.Progress < 1);

        // A safe end, in case a panel never reaches its end.
        private const int MaxRevealFrames = 120;

        private void BringRowToMiddle(TreeNode node) => BringRowIntoView(node, BringToMiddle);

        // The request asks for a block as high as the view, with the row at its middle. The scroller stops at the ends of the tree.
        private void BringToMiddle(Border border)
        {
            if (context.Scroller is { Viewport.Height: > 0 } scroller)
            {
                var height = scroller.Viewport.Height;
                border.BringIntoView(new Rect(0, (border.Bounds.Height - height) / 2, border.Bounds.Width, height));
            }
            else
            {
                border.BringIntoView();
            }
        }

        private void BringRowIntoView(TreeNode node, Action<Border> bring)
        {
            if (context.RowIndex.FindItem(node) is not { } item || TreeRows.RowBorderOf(item) is not { } border)
                return;

            revealTarget = border;
            try
            {
                bring(border);
            }
            finally
            {
                revealTarget = null;
            }
        }
    }
}
