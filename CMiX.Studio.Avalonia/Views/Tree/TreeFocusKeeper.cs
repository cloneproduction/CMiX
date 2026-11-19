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
using CMiX.UI.Panels;

namespace CMiX.Studio.Avalonia.Views
{
    // The focus goes back to a row after a rename, a move, or a press beside the rows or on a row button.
    internal sealed class TreeFocusKeeper
    {
        private sealed record FocusBefore(IInputElement? Control);

        private readonly Control owner;
        private readonly TreeViewContext context;
        private readonly TreeRowReveal reveal;
        private readonly TreeViewLifetime lifetime;

        // Also true when the focus went from the tree to no control, so it can come back (see GiveTheFocusBack).
        private bool focusIsInTree;

        private bool focusCheckPosted;

        private bool pressWasBesideRows;

        private FocusBefore? focusBeforeRowButton;

        internal TreeFocusKeeper(Control owner, TreeViewContext context, TreeRowReveal reveal,
            TreeViewLifetime lifetime)
        {
            this.owner = owner;
            this.context = context;
            this.reveal = reveal;
            this.lifetime = lifetime;
        }

        internal void Attach()
        {
            owner.AddHandler(InputElement.GotFocusEvent, OnGotFocusInTree, RoutingStrategies.Bubble, handledEventsToo: true);
            owner.AddHandler(InputElement.LostFocusEvent, OnLostFocusInTree, RoutingStrategies.Bubble, handledEventsToo: true);
            context.TreeView.LayoutUpdated += OnLayoutUpdated;
        }

        internal void Detach()
        {
            owner.RemoveHandler(InputElement.GotFocusEvent, OnGotFocusInTree);
            owner.RemoveHandler(InputElement.LostFocusEvent, OnLostFocusInTree);
            context.TreeView.LayoutUpdated -= OnLayoutUpdated;
            focusCheckPosted = false;
            pressWasBesideRows = false;
            focusBeforeRowButton = null;
        }

        private void OnGotFocusInTree(object? sender, GotFocusEventArgs e) => focusIsInTree = true;

        // The next focus is known only after the event.
        private void OnLostFocusInTree(object? sender, RoutedEventArgs e)
        {
            var node = TreeRows.ItemOf(e.Source)?.DataContext as TreeNode;
            lifetime.Post(() => GiveTheFocusBack(node), DispatcherPriority.Input);
        }

        // A move makes the item of the row again with no focus event, so the layout shows the lost focus.
        private void OnLayoutUpdated(object? sender, EventArgs e) => CheckTheLostFocus();

        private void CheckTheLostFocus()
        {
            if (!focusIsInTree || focusCheckPosted || TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement() != null)
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
            var focused = TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement();
            if (focused != null)
            {
                focusIsInTree = focused is Visual visual && (ReferenceEquals(visual, owner) || owner.IsVisualAncestorOf(visual));
                return;
            }

            if (!focusIsInTree || (node ?? context.Tree?.SelectedNode) is not { } target || context.RowIndex.FindItem(target) is not { } item)
                return;

            item.Focus(NavigationMethod.Tab);
        }

        internal void FocusRow(TreeNode node)
        {
            if (context.RowIndex.FindItem(node) is not { } item)
                return;

            item.Focus(NavigationMethod.Tab);
            reveal.RevealRow(node);
        }

        internal void BeginPress() => focusBeforeRowButton = null;

        internal void RememberFocusBeforeRowButton() => focusBeforeRowButton = new FocusBefore(FocusedControl());

        internal void FocusCursorRowAfterPress() => pressWasBesideRows = true;

        // A toggle or the chevron cannot take the focus, so the focus goes back to the control before the press, else to the cursor row.
        internal void EndPress()
        {
            if (pressWasBesideRows)
            {
                pressWasBesideRows = false;
                if (context.Tree?.CursorNode is { } cursor)
                    lifetime.Post(() => FocusRow(cursor), DispatcherPriority.Input);
                return;
            }

            if (focusBeforeRowButton is not { } before)
                return;

            focusBeforeRowButton = null;
            if (ReferenceEquals(FocusedControl(), before.Control))
                return;

            if (before.Control is InputElement { IsEffectivelyVisible: true } control && IsInPanel(control) && control.GetVisualRoot() != null)
                control.Focus(control is TreeViewItem ? NavigationMethod.Tab : NavigationMethod.Unspecified);
            else if (context.Tree?.CursorNode is { } cursor && context.RowIndex.FindItem(cursor) is { } item)
                item.Focus(NavigationMethod.Tab);
        }

        internal bool IsSearchFocused()
        {
            if (FocusedControl() is not Visual focused || focused.FindAncestorOfType<TreeSearchBox>(true) is not { } box
                || !ReferenceEquals(box.DataContext, context.Tree))
                return false;

            return owner.FindAncestorOfType<SidePanelHost>() is not { } host || host.IsVisualAncestorOf(box);
        }

        internal static bool IsInTextBox(object? source) => TreeRows.HasAncestor<TextBox>(source);

        internal bool IsTextBoxFocused() => IsInTextBox(TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement());

        // The focus on the window itself, or on no control, is in no panel.
        internal bool IsFocusInOtherPanel() => FocusedControl() is { } focused && !IsInPanel(focused);

        internal bool IsPanelFocused() => FocusedControl() is { } focused && IsInPanel(focused);

        private IInputElement? FocusedControl() =>
            TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement() is { } focused and not TopLevel ? focused : null;

        private bool IsInPanel(IInputElement focused)
        {
            Visual panel = owner.FindAncestorOfType<SidePanelHost>() is { } host ? host : owner;
            return focused is Visual visual && (ReferenceEquals(visual, panel) || panel.IsVisualAncestorOf(visual));
        }
    }
}
