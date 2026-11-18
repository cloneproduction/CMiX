// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
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
    // Keeps the keyboard focus on the rows of one view of the tree. The focus goes back to a row when a name box hides at the end of a
    // rename, when a move makes the item of a row again, or after a press beside the rows or on a button of a row. It also answers
    // the focus questions of the tree (see TreeViewHooks).
    internal sealed class TreeFocusKeeper
    {
        // The focused control before a press on a button of a row. Null when the press was not on such a button.
        private sealed record FocusBefore(IInputElement? Control);

        private readonly Control owner;
        private readonly TreeViewContext context;
        private readonly TreeRowReveal reveal;
        private readonly Action<Action, DispatcherPriority> postWhileAttached;
        private readonly Func<bool> isAttached;

        // True while the keyboard focus is in the tree, and also when it went from the tree to no control: a name box that hides at the
        // end of a rename, or a row that a move makes again. The focus then goes back to the row (see GiveTheFocusBack).
        private bool focusIsInTree;

        // True while a check of the focus waits to run.
        private bool focusCheckPosted;

        // Set by a press beside the rows: the cursor row takes the focus once the press is done.
        private bool focusBesideRows;

        private FocusBefore? focusBeforeRowButton;

        internal TreeFocusKeeper(Control owner, TreeViewContext context, TreeRowReveal reveal,
            Action<Action, DispatcherPriority> postWhileAttached, Func<bool> isAttached)
        {
            this.owner = owner;
            this.context = context;
            this.reveal = reveal;
            this.postWhileAttached = postWhileAttached;
            this.isAttached = isAttached;
        }

        internal TreeViewHooks BuildHooks(TreeDragSession dragSession, TreeRowDrag rowDrag, TreeGrab grab,
            Action<TreeNode?, Func<IReadOnlyList<TreeNode>>> closeRowsInView) =>
            new()
            {
                IsPointerOverTree = () => owner.IsPointerOver,
                IsTreeFocused = () => owner.IsKeyboardFocusWithin,
                IsTextBoxFocused = () => IsInTextBox(TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement()),
                IsFocusInOtherPanel = IsFocusInOtherPanel,
                IsPanelFocused = IsPanelFocused,
                ToggleDragRow = dragSession.ToggleHovered,
                CancelDrag = () =>
                {
                    rowDrag.Cancel();
                    grab.Cancel();
                },
                CloseRowsInView = closeRowsInView
            };

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
            focusIsInTree = false;
            focusCheckPosted = false;
            focusBesideRows = false;
            focusBeforeRowButton = null;
        }

        private void OnGotFocusInTree(object? sender, GotFocusEventArgs e) => focusIsInTree = true;

        // A name box loses the focus when a rename ends. The next focus is known only after the event.
        private void OnLostFocusInTree(object? sender, RoutedEventArgs e)
        {
            var node = TreeRows.ItemOf(e.Source)?.DataContext as TreeNode;
            postWhileAttached(() => GiveTheFocusBack(node), DispatcherPriority.Input);
        }

        // A move makes the item of the row again, and the old item takes the focus away with it, with no event that reaches the
        // tree. The layout that follows shows that no control has the focus.
        private void OnLayoutUpdated(object? sender, EventArgs e) => CheckTheLostFocus();

        private void CheckTheLostFocus()
        {
            if (!focusIsInTree || focusCheckPosted || TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement() != null)
                return;

            focusCheckPosted = true;
            Dispatcher.UIThread.Post(() =>
            {
                focusCheckPosted = false;
                if (isAttached())
                    GiveTheFocusBack(null);
            }, DispatcherPriority.Input);
        }

        // When no control has the focus and the focus was in the tree, the row of the node gets it, else the selected row. A control
        // that the user clicked keeps the focus.
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

        // Moves the keyboard focus to the row of the node and brings the row into view. The focus shows the focus look of the row
        // (:focus-visible, see ProjectTree.axaml). It does not select the row.
        internal void FocusRow(TreeNode node)
        {
            if (context.RowIndex.FindItem(node) is not { } item)
                return;

            item.Focus(NavigationMethod.Tab);
            reveal.RevealRow(node);
        }

        // A press starts: the focus before a button of a row is not known yet.
        internal void BeginPress() => focusBeforeRowButton = null;

        // The press is on a toggle or the chevron of a row (see EndPress).
        internal void RememberFocusBeforeRowButton() => focusBeforeRowButton = new FocusBefore(FocusedControl());

        // The press is beside the rows: the cursor row takes the focus once the press is done (see EndPress).
        internal void FocusCursorRowAfterPress() => focusBesideRows = true;

        // A toggle or the chevron of a row cannot take the focus, so a click on it focuses its row. The click acts on that row only:
        // a focused control of the panel of the tree keeps the focus. Else the focus goes to the cursor row. The cursor and the
        // selection do not move.
        internal void EndPress()
        {
            if (focusBesideRows)
            {
                focusBesideRows = false;
                if (context.Tree?.CursorNode is { } cursor)
                    postWhileAttached(() => FocusRow(cursor), DispatcherPriority.Input);
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

        // True when the focus is in the search field of the tree of this view, in the panel of this view.
        internal bool IsSearchFocused()
        {
            if (FocusedControl() is not Visual focused || focused.FindAncestorOfType<TreeSearchBox>(true) is not { } box
                || !ReferenceEquals(box.DataContext, context.Tree))
                return false;

            return owner.FindAncestorOfType<SidePanelHost>() is not { } host || host.IsVisualAncestorOf(box);
        }

        internal static bool IsInTextBox(object? source)
        {
            for (var visual = source as Visual; visual != null; visual = visual.GetVisualParent())
                if (visual is TextBox)
                    return true;

            return false;
        }

        // True when a control outside the panel of this view has the focus (see ProjectTree.IsFocusInOtherPanel). The panel is the
        // side panel that shows the view, with its bar. The focus on the window itself, or on no control, is in no panel.
        private bool IsFocusInOtherPanel() => FocusedControl() is { } focused && !IsInPanel(focused);

        // True when the focused control is in the panel of this view (see TreeViewHooks.IsPanelFocused).
        private bool IsPanelFocused() => FocusedControl() is { } focused && IsInPanel(focused);

        // The focused control. Null when the window itself or no control has the focus.
        private IInputElement? FocusedControl() =>
            TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement() is { } focused and not TopLevel ? focused : null;

        private bool IsInPanel(IInputElement focused)
        {
            Visual panel = owner.FindAncestorOfType<SidePanelHost>() is { } host ? host : owner;
            return focused is Visual visual && (ReferenceEquals(visual, panel) || panel.IsVisualAncestorOf(visual));
        }
    }
}
