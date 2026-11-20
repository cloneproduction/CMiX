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
    // A click does not scroll: the second click of a double click would land on another row.
    internal sealed class TreeRowReveal
    {
        private readonly TreeViewContext context;

        private Visual? revealTarget;

        // A new reveal ends the one before.
        private readonly FrameHold hold = new();

        internal TreeRowReveal(TreeViewContext context)
        {
            this.context = context;
        }

        internal void Attach() => context.TreeView.TemplateApplied += OnTemplateApplied;

        private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e) =>
            e.NameScope.Find<ItemsPresenter>("PART_ItemsPresenter")?.AddHandler(Control.RequestBringIntoViewEvent, OnRequestBringIntoView);

        // The request of a row scrolls to its whole block, so it stops here.
        private void OnRequestBringIntoView(object? sender, RequestBringIntoViewEventArgs e)
        {
            if (!ReferenceEquals(e.TargetObject, revealTarget))
                e.Handled = true;
        }

        internal void RevealRow(TreeNode node) => BringRowIntoView(node, border => border.BringIntoView());

        internal TreeViewItem? ShownItemOf(TreeNode node)
        {
            if (context.RowIndex.FindItem(node) is not { } item)
                return null;

            RevealRow(node);
            context.TreeView.UpdateLayout();
            return item;
        }

        // The parents open over a few frames, so the row is put back in the middle in each frame.
        internal void RevealRowToMiddle(TreeNode node)
        {
            var isCurrent = hold.Claim();
            Dispatcher.UIThread.Post(() =>
            {
                if (!isCurrent() || context.RowIndex.FindItem(node) is not { } item)
                    return;

                if (context.IsTreeFocused)
                    item.Focus(NavigationMethod.Tab);

                BringRowToMiddle(node);
                if (IsOpening(item) && TopLevel.GetTopLevel(context.TreeView) is { } top)
                    hold.Run(top, () => BringRowToMiddle(node), () => IsOpening(item));
            }, DispatcherPriority.Loaded);
        }

        internal void EndReveal() => hold.Stop();

        private static bool IsOpening(TreeViewItem item) =>
            item.GetVisualAncestors().OfType<PanelCollapse>().Any(panel => panel.IsExpanded && panel.Progress < 1);

        private void BringRowToMiddle(TreeNode node) => BringRowIntoView(node, BringToMiddle);

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
