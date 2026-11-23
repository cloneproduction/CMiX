// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.Behaviors.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Popups;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The one Add panel of the new tree. Open puts it beside the cursor row, as TreeMenus.OpenAddBesideTheCursor does in the old tree.
    internal sealed class ListTreeAddBeside : ListTreeBehavior
    {
        private const string AddTitle = "Add";

        private ProjectTree? tree;

        private bool openedBesideTheCursor;

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            Context.AddPanel.Closed += OnAddPanelClosed;
        }

        protected override void OnUnwatch()
        {
            Context.AddPanel.Closed -= OnAddPanelClosed;
            Context.AddPanel.Close();
            openedBesideTheCursor = false;
            tree = null;
        }

        public void Open()
        {
            var addPanel = Context.AddPanel;
            if (tree?.CursorNode is not { } node || Context.Rows.RowOf(node) is not { } row || TopLevel.GetTopLevel(row) is not { } window)
                return;

            var anchor = row.TranslatePoint(default, window) is { } topLeft ? new Rect(topLeft, row.Bounds.Size) : (Rect?)null;
            addPanel.OpenWithTip(row, PopupSide.Right, tree.GetAddGroups(node), AddTitle, anchor);
            openedBesideTheCursor = addPanel.IsOpen;
            Part<ListTreeHints>().Update();
        }

        // A panel beside the cursor row is a leaf of the tree, so the cursor row takes the focus back.
        private void OnAddPanelClosed(object? sender, EventArgs e)
        {
            if (!openedBesideTheCursor)
                return;

            openedBesideTheCursor = false;
            if (tree?.CursorNode is { } cursor)
                Part<ListTreeFocusKeeper>().FocusRowLater(cursor, DispatcherPriority.Input);
        }
    }
}
