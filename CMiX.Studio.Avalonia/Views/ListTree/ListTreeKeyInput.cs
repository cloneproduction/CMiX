// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Behaviors.ListTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The keys of the new tree that need the focus in its rows, as TreeKeyInput gives them to the old tree. They move the cursor and select nothing.
    internal sealed class ListTreeKeyInput
    {
        private readonly Control owner;
        private readonly PanelListTreeRows rows;
        private readonly ListTreeFocusKeeper focusKeeper;
        private readonly ListTreeAddBeside addBeside;
        private readonly TreeViewLifetime lifetime;

        private ProjectTree? tree;
        private ListTreeCursor? cursor;

        public ListTreeKeyInput(Control owner, PanelListTreeRows rows, ListTreeFocusKeeper focusKeeper, ListTreeAddBeside addBeside,
            TreeViewLifetime lifetime)
        {
            this.owner = owner;
            this.rows = rows;
            this.focusKeeper = focusKeeper;
            this.addBeside = addBeside;
            this.lifetime = lifetime;
        }

        public void Attach(ProjectTree attached)
        {
            tree = attached;
            cursor = new ListTreeCursor(attached);
            owner.AddHandler(InputElement.KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);
        }

        public void Detach()
        {
            owner.RemoveHandler(InputElement.KeyDownEvent, OnKeyDownTunnel);
            tree = null;
            cursor = null;
        }

        private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
        {
            if (tree == null || cursor == null || TreeFocusKeeper.IsInTextBox(e.Source))
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
                addBeside.Open();
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
                lifetime.Post(() => focusKeeper.FocusRow(node), DispatcherPriority.Loaded);
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
            var pitch = rows.RowHeight + rows.RowGap;
            return rows.FindAncestorOfType<ScrollViewer>() is { } scroller && pitch > 0 ? Math.Max(1, (int)(scroller.Viewport.Height / pitch)) : 1;
        }
    }
}
