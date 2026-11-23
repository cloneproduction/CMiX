// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // Brings a row of the new tree into view, as TreeRowReveal does in the old tree. The rows have known places, so a reveal needs no frame hold.
    internal sealed class ListTreeRowReveal
    {
        private readonly Control owner;
        private readonly PanelListTreeRows rows;
        private readonly ScrollViewer scroller;
        private readonly TreeViewLifetime lifetime;

        private ProjectTree? tree;

        public ListTreeRowReveal(Control owner, PanelListTreeRows rows, ScrollViewer scroller, TreeViewLifetime lifetime)
        {
            this.owner = owner;
            this.rows = rows;
            this.scroller = scroller;
            this.lifetime = lifetime;
        }

        // A selection by a program or a peer shows its row, as in the old tree.
        public void Watch(ProjectTree watched)
        {
            tree = watched;
            watched.PropertyChanged += OnTreeChanged;
        }

        public void Unwatch()
        {
            if (tree != null)
                tree.PropertyChanged -= OnTreeChanged;
            tree = null;
        }

        public void RevealRow(TreeNode node) => ShownRowOf(node);

        // A row under a closed card is not shown, so it gives null.
        public RowListTree? ShownRowOf(TreeNode node)
        {
            rows.ApplyTreeChanges();
            if (rows.RowOf(node) is not { } row)
                return null;

            row.BringIntoView();
            rows.UpdateLayout();
            return row;
        }

        // The parents open first, so the reveal runs after the rebuild.
        public void RevealRowToMiddle(TreeNode node) => lifetime.Post(() => BringRowToMiddle(node), DispatcherPriority.Loaded);

        private void BringRowToMiddle(TreeNode node)
        {
            rows.ApplyTreeChanges();
            if (rows.RowOf(node) is not { Place: { } place } row)
                return;

            if (owner.IsKeyboardFocusWithin)
                row.Focus(NavigationMethod.Tab);

            scroller.UpdateLayout();
            var middle = rows.Bounds.Y + place.Y + place.RowHeight / 2 - scroller.Viewport.Height / 2;
            var maxOffset = Math.Max(0, scroller.Extent.Height - scroller.Viewport.Height);
            scroller.Offset = scroller.Offset.WithY(Math.Clamp(middle, 0, maxOffset));
        }

        private void OnTreeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ProjectTree.SelectedNode) || tree?.SelectedNode is not { } node || !tree.RevealsSelection)
                return;

            lifetime.Post(() => RevealRow(node), DispatcherPriority.Loaded);
        }
    }
}
