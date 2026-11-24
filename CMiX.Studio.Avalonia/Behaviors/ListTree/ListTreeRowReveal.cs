// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // Brings a row into view. The rows have known places, so a reveal needs no frame hold.
    internal sealed class ListTreeRowReveal : ListTreeBehavior
    {
        private ProjectTree? tree;

        // A selection by a program or a peer shows its row.
        protected override void OnWatch(ProjectTree watched)
        {
            tree = watched;
            watched.PropertyChanged += OnTreeChanged;
        }

        protected override void OnUnwatch()
        {
            if (tree != null)
                tree.PropertyChanged -= OnTreeChanged;
            tree = null;
        }

        public void RevealRow(TreeNode node) => ShownRowOf(node);

        // A row under a closed card is not shown, so it gives null.
        public RowListTree? ShownRowOf(TreeNode node)
        {
            var rows = Context.Rows;
            rows.ApplyTreeChanges();
            if (rows.RowOf(node) is not { } row)
                return null;

            row.BringIntoView();
            rows.UpdateLayout();
            return row;
        }

        // During a drag the rows show a candidate, so the place comes from its layout and not from a row control.
        public void RevealPlace(ListTreePlace place)
        {
            var scroller = Context.Scroller;
            scroller.UpdateLayout();
            var top = Context.Rows.Bounds.Y + place.Y;
            var offset = scroller.Offset.Y;
            var height = scroller.Viewport.Height;
            var target = top < offset ? top : top + place.RowHeight > offset + height ? top + place.RowHeight - height : offset;
            Context.ScrollTo(target);
        }

        // The parents open first, so the reveal runs after the rebuild.
        public void RevealRowToMiddle(TreeNode node) => Context.Lifetime.Post(() => BringRowToMiddle(node), DispatcherPriority.Loaded);

        private void BringRowToMiddle(TreeNode node)
        {
            var rows = Context.Rows;
            var scroller = Context.Scroller;
            rows.ApplyTreeChanges();
            if (rows.RowOf(node) is not { Place: { } place } row)
                return;

            if (Context.Owner.IsKeyboardFocusWithin)
                row.Focus(NavigationMethod.Tab);

            scroller.UpdateLayout();
            var middle = rows.Bounds.Y + place.Y + place.RowHeight / 2 - scroller.Viewport.Height / 2;
            Context.ScrollTo(middle);
        }

        private void OnTreeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ProjectTree.SelectedNode) || tree?.SelectedNode is not { } node || !tree.RevealsSelection)
                return;

            Context.Lifetime.Post(() => RevealRow(node), DispatcherPriority.Loaded);
        }
    }
}
