// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Popups;

namespace CMiX.Studio.Avalonia.Views
{
    // The one Add panel of the new tree. Open puts it beside the cursor row, as TreeMenus.OpenAddBesideTheCursor does in the old tree.
    internal sealed class ListTreeAddBeside
    {
        private const string AddTitle = "Add";

        private readonly PanelListTreeRows rows;
        private readonly ListTreeFocusKeeper focusKeeper;
        private readonly Action updateHints;
        private readonly TreeViewLifetime lifetime;
        private readonly AddPanel addPanel = new();

        private ProjectTree? tree;

        private bool openedBesideTheCursor;

        public ListTreeAddBeside(PanelListTreeRows rows, ListTreeFocusKeeper focusKeeper, Action updateHints, TreeViewLifetime lifetime)
        {
            this.rows = rows;
            this.focusKeeper = focusKeeper;
            this.updateHints = updateHints;
            this.lifetime = lifetime;
        }

        public AddPanel AddPanel => addPanel;

        public void Attach(ProjectTree attached)
        {
            tree = attached;
            addPanel.Closed += OnAddPanelClosed;
        }

        public void Detach()
        {
            addPanel.Closed -= OnAddPanelClosed;
            addPanel.Close();
            openedBesideTheCursor = false;
            tree = null;
        }

        public void Open()
        {
            if (tree?.CursorNode is not { } node || rows.RowOf(node) is not { } row || TopLevel.GetTopLevel(row) is not { } window)
                return;

            var anchor = row.TranslatePoint(default, window) is { } topLeft ? new Rect(topLeft, row.Bounds.Size) : (Rect?)null;
            addPanel.OpenWithTip(row, PopupSide.Right, tree.GetAddGroups(node), AddTitle, anchor);
            openedBesideTheCursor = addPanel.IsOpen;
            updateHints();
        }

        // A panel beside the cursor row is a leaf of the tree, so the cursor row takes the focus back.
        private void OnAddPanelClosed(object? sender, EventArgs e)
        {
            updateHints();
            if (!openedBesideTheCursor)
                return;

            openedBesideTheCursor = false;
            if (tree?.CursorNode is { } cursor)
                lifetime.Post(() => focusKeeper.FocusRow(cursor), DispatcherPriority.Input);
        }
    }
}
