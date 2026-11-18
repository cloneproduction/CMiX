// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The keys of one view of the tree that the key bindings of the view do not take.
    internal sealed class TreeKeyInput
    {
        private readonly TreeViewContext context;
        private readonly TreeRowDrag rowDrag;
        private readonly TreeMenus menus;
        private readonly TreeFocusKeeper focusKeeper;
        private readonly Action<Action, DispatcherPriority> postWhileAttached;

        internal TreeKeyInput(TreeViewContext context, TreeRowDrag rowDrag, TreeMenus menus, TreeFocusKeeper focusKeeper,
            Action<Action, DispatcherPriority> postWhileAttached)
        {
            this.context = context;
            this.rowDrag = rowDrag;
            this.menus = menus;
            this.focusKeeper = focusKeeper;
            this.postWhileAttached = postWhileAttached;
        }

        internal void Attach(TreeView treeView) => treeView.AddHandler(InputElement.KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);

        // Escape ends a drag of a row without a drop. Else, while a search is active, Escape clears it (see ProjectTree.ClearSearch): the
        // focus and the cursor row stay. A grab, a menu and a name box take their Escape first. With no search, Escape does nothing here.
        // Avalonia runs the key bindings of the view (the gestures of the map) before this handler. The navigation keys move the cursor
        // only (see ProjectTree.CursorNode). The tree view does not see them, so they select nothing. A navigation key
        // with a modifier does nothing. A key in a name box stays in the name box. The keys of IsTreeViewKey do nothing.
        private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && rowDrag.IsDragging)
            {
                rowDrag.Cancel();
                e.Handled = true;
                return;
            }

            if (context.Tree is not { } tree || TreeFocusKeeper.IsInTextBox(e.Source))
                return;

            if (e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None && tree.IsSearchActive && !tree.IsDragging)
            {
                tree.ClearSearchText();
                e.Handled = true;
                return;
            }

            // Tree.Inspect (Space by default) is handled here, before the tree view takes Space for its own selection.
            if (tree.Shortcuts["Tree.Inspect"] is { } inspectGesture && inspectGesture.Matches(e))
            {
                if (tree.InspectCursorCommand.CanExecute(null))
                    tree.InspectCursorCommand.Execute(null);
                e.Handled = true;
                return;
            }

            // The tree view opens and closes rows on these keys by itself. The design does not use them.
            if (IsTreeViewKey(e.Key))
            {
                e.Handled = true;
                return;
            }

            // The Add panel is the leaf after the last row of a branch: Right on a row with nothing under it opens it, at the right of the row,
            // its search field level with the row and a tip at the middle of the row. Ctrl+Right opens it on any row. The arrows then pick
            // an entry and Enter adds it, so a row deep in the tree is reached and filled with Right alone. Left in the panel comes back.
            if (e.Key == Key.Right && (e.KeyModifiers == KeyModifiers.Control
                                       || (e.KeyModifiers == KeyModifiers.None && tree.CursorNode is { Children.Count: 0 })))
            {
                menus.OpenAddBesideTheCursor(tree);
                e.Handled = true;
                return;
            }

            if (!IsNavigationKey(e.Key))
                return;

            e.Handled = true;
            if (e.KeyModifiers != KeyModifiers.None)
                return;

            MoveCursor(tree, e.Key);
            if (tree.CursorNode is { } node)
                postWhileAttached(() => focusKeeper.FocusRow(node), DispatcherPriority.Loaded);
        }

        private static bool IsNavigationKey(Key key) =>
            key is Key.Up or Key.Down or Key.PageUp or Key.PageDown or Key.Home or Key.End or Key.Left or Key.Right;

        // The numpad keys that TreeViewItem uses to open and close rows. Enter and the navigation keys are not in this list.
        internal static bool IsTreeViewKey(Key key) =>
            key is Key.Add or Key.Subtract or Key.Multiply or Key.Divide;

        private void MoveCursor(ProjectTree tree, Key key)
        {
            switch (key)
            {
                case Key.Up: tree.MoveCursor(-1); break;
                case Key.Down: tree.MoveCursor(1); break;
                case Key.PageUp: tree.MoveCursor(-RowsInView()); break;
                case Key.PageDown: tree.MoveCursor(RowsInView()); break;
                case Key.Home: tree.MoveCursorToFirst(); break;
                case Key.End: tree.MoveCursorToLast(); break;
                case Key.Left: tree.MoveCursorOut(); break;
                case Key.Right: tree.MoveCursorIn(); break;
            }
        }

        private int RowsInView() =>
            context.Scroller is { } scroller && context.RowPitch > 0 ? Math.Max(1, (int)(scroller.Viewport.Height / context.RowPitch)) : 1;
    }
}
