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
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The focus goes back to a row of the tree after a rename or a move. ListTreePressFocus keeps it after a press.
    internal sealed class ListTreeFocusKeeper : ListTreeBehavior
    {
        private ProjectTree? tree;

        // Also true when the focus went from the tree to no control, so it can come back (see GiveTheFocusBack).
        private bool focusIsInTree;

        private bool focusCheckPosted;

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            Context.Owner.AddHandler(InputElement.GotFocusEvent, OnGotFocusInTree, RoutingStrategies.Bubble, handledEventsToo: true);
            Context.Owner.AddHandler(InputElement.LostFocusEvent, OnLostFocusInTree, RoutingStrategies.Bubble, handledEventsToo: true);
            Context.Rows.LayoutUpdated += OnLayoutUpdated;
        }

        protected override void OnUnwatch()
        {
            Context.Owner.RemoveHandler(InputElement.GotFocusEvent, OnGotFocusInTree);
            Context.Owner.RemoveHandler(InputElement.LostFocusEvent, OnLostFocusInTree);
            Context.Rows.LayoutUpdated -= OnLayoutUpdated;
            focusCheckPosted = false;
            focusIsInTree = false;
            tree = null;
        }

        private void OnGotFocusInTree(object? sender, GotFocusEventArgs e) => focusIsInTree = true;

        // The next focus is known only after the event.
        private void OnLostFocusInTree(object? sender, RoutedEventArgs e)
        {
            var node = RowListTree.Of(e.Source)?.DataContext as TreeNode;
            Context.Lifetime.Post(() => GiveTheFocusBack(node), DispatcherPriority.Input);
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
                if (Context.Lifetime.IsAttached)
                    GiveTheFocusBack(null);
            }, DispatcherPriority.Input);
        }

        // A control that the user clicked keeps the focus.
        private void GiveTheFocusBack(TreeNode? node)
        {
            var owner = Context.Owner;
            if (FocusedElement() is { } focused)
            {
                focusIsInTree = focused is Visual visual && (ReferenceEquals(visual, owner) || owner.IsVisualAncestorOf(visual));
                return;
            }

            if (focusIsInTree && (node ?? RowToFocus()) is { } target)
                Context.Rows.RowOf(target)?.Focus(NavigationMethod.Tab);
        }

        // During a search, the filter can hide the selected row, so the focus goes to the cursor row.
        private TreeNode? RowToFocus()
        {
            if (tree is { IsSearchActive: true })
                return tree.CursorNode;

            return tree?.SelectedNode;
        }

        public void FocusRow(TreeNode node)
        {
            if (Context.Rows.RowOf(node) is not { } row)
                return;

            row.Focus(NavigationMethod.Tab);
            row.BringIntoView();
        }

        public void FocusRowLater(TreeNode node, DispatcherPriority priority) => Context.Lifetime.Post(() => FocusRow(node), priority);

        private IInputElement? FocusedElement() => TopLevel.GetTopLevel(Context.Owner)?.FocusManager?.GetFocusedElement();
    }
}
