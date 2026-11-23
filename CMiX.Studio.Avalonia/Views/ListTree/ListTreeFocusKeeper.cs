// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The focus goes back to a row of the new tree after a rename or a move, as TreeFocusKeeper does. ListTreePressFocus keeps it after a press.
    internal sealed class ListTreeFocusKeeper
    {
        private readonly Control owner;
        private readonly PanelListTreeRows rows;
        private readonly TreeViewLifetime lifetime;

        private ProjectTree? tree;

        // Also true when the focus went from the tree to no control, so it can come back (see GiveTheFocusBack).
        private bool focusIsInTree;

        private bool focusCheckPosted;

        public ListTreeFocusKeeper(Control owner, PanelListTreeRows rows, TreeViewLifetime lifetime)
        {
            this.owner = owner;
            this.rows = rows;
            this.lifetime = lifetime;
        }

        public void Attach(ProjectTree attached)
        {
            tree = attached;
            owner.AddHandler(InputElement.GotFocusEvent, OnGotFocusInTree, RoutingStrategies.Bubble, handledEventsToo: true);
            owner.AddHandler(InputElement.LostFocusEvent, OnLostFocusInTree, RoutingStrategies.Bubble, handledEventsToo: true);
            rows.LayoutUpdated += OnLayoutUpdated;
        }

        public void Detach()
        {
            owner.RemoveHandler(InputElement.GotFocusEvent, OnGotFocusInTree);
            owner.RemoveHandler(InputElement.LostFocusEvent, OnLostFocusInTree);
            rows.LayoutUpdated -= OnLayoutUpdated;
            focusCheckPosted = false;
            tree = null;
        }

        private void OnGotFocusInTree(object? sender, GotFocusEventArgs e) => focusIsInTree = true;

        // The next focus is known only after the event.
        private void OnLostFocusInTree(object? sender, RoutedEventArgs e)
        {
            var node = (e.Source as Visual)?.FindAncestorOfType<RowListTree>(includeSelf: true)?.DataContext as TreeNode;
            lifetime.Post(() => GiveTheFocusBack(node), DispatcherPriority.Input);
        }

        // A rebuild takes away the row that had the focus with no focus event, so the layout shows the lost focus.
        private void OnLayoutUpdated(object? sender, EventArgs e) => CheckTheLostFocus();

        private void CheckTheLostFocus()
        {
            if (!focusIsInTree || focusCheckPosted || FocusedElement() != null)
                return;

            focusCheckPosted = true;
            Dispatcher.UIThread.Post(() =>
            {
                focusCheckPosted = false;
                if (lifetime.IsAttached)
                    GiveTheFocusBack(null);
            }, DispatcherPriority.Input);
        }

        // A control that the user clicked keeps the focus.
        private void GiveTheFocusBack(TreeNode? node)
        {
            if (FocusedElement() is { } focused)
            {
                focusIsInTree = focused is Visual visual && (ReferenceEquals(visual, owner) || owner.IsVisualAncestorOf(visual));
                return;
            }

            if (focusIsInTree && (node ?? tree?.SelectedNode) is { } target)
                rows.RowOf(target)?.Focus(NavigationMethod.Tab);
        }

        public void FocusRow(TreeNode node)
        {
            if (rows.RowOf(node) is not { } row)
                return;

            row.Focus(NavigationMethod.Tab);
            row.BringIntoView();
        }

        private IInputElement? FocusedElement() => TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement();
    }
}
