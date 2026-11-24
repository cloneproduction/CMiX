// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The keys of the tree that need the focus in its rows. They move the cursor and select nothing.
    internal sealed class ListTreeKeyInput : ListTreeBehavior
    {
        private ProjectTree? tree;
        private ListTreeCursor? cursor;

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            cursor = new ListTreeCursor(attached, ShownRows);
            Context.Scroller.AddHandler(InputElement.KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);
        }

        protected override void OnUnwatch()
        {
            Context.Scroller.RemoveHandler(InputElement.KeyDownEvent, OnKeyDownTunnel);
            tree = null;
            cursor = null;
        }

        private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
        {
            if (tree == null || cursor == null || ListTreeFocus.IsInTextBox(e.Source))
                return;

            if (e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None && tree.IsSearchActive && !tree.IsDragging)
            {
                tree.ClearSearchText();
                e.Handled = true;
                return;
            }

            if (tree.Shortcuts["Tree.Inspect"] is { } inspectGesture && inspectGesture.Matches(e))
            {
                if (tree.InspectCursorCommand.CanExecute(null))
                    tree.InspectCursorCommand.Execute(null);
                e.Handled = true;
                return;
            }

            // The Add panel is the leaf after the last row of a branch, so Right on a row with nothing under it opens it.
            if (e.Key == Key.Right && (e.KeyModifiers == KeyModifiers.Control
                                       || (e.KeyModifiers == KeyModifiers.None && cursor.IsCursorOnLeaf())))
            {
                Part<ListTreeAddBeside>().Open();
                e.Handled = true;
                return;
            }

            if (!IsNavigationKey(e.Key))
                return;

            e.Handled = true;
            if (e.KeyModifiers != KeyModifiers.None)
                return;

            MoveCursor(cursor, e.Key);
            if (tree.CursorNode is { } node)
                Part<ListTreeFocusKeeper>().FocusRowLater(node, DispatcherPriority.Loaded);
        }

        // The rows of the panel are the shown rows, after a posted rebuild.
        private IReadOnlyList<ListTreeRow> ShownRows()
        {
            Context.Rows.ApplyTreeChanges();
            return Context.Rows.LiveRows;
        }

        private static bool IsNavigationKey(Key key) =>
            key is Key.Up or Key.Down or Key.PageUp or Key.PageDown or Key.Home or Key.End or Key.Left or Key.Right;

        private void MoveCursor(ListTreeCursor moving, Key key)
        {
            switch (key)
            {
                case Key.Up: moving.Move(-1); break;
                case Key.Down: moving.Move(1); break;
                case Key.PageUp: moving.Move(-RowsInView()); break;
                case Key.PageDown: moving.Move(RowsInView()); break;
                case Key.Home: moving.MoveToFirst(); break;
                case Key.End: moving.MoveToLast(); break;
                case Key.Left: moving.MoveOut(); break;
                case Key.Right: moving.MoveIn(); break;
            }
        }

        private int RowsInView()
        {
            var rows = Context.Rows;
            var pitch = rows.Sizes.Pitch;
            return rows.FindAncestorOfType<ScrollViewer>() is { } scroller && pitch > 0 ? Math.Max(1, (int)(scroller.Viewport.Height / pitch)) : 1;
        }
    }
}
