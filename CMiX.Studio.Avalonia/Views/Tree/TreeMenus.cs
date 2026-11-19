// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;
using CompositionModel = CMiX.Core.Compositing.Composition;
using LayerModel = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Views
{
    internal sealed class TreeMenus
    {
        private const string AddTitle = "Add";

        private readonly TreeViewContext context;
        private readonly TreeRowReveal reveal;
        private readonly TreePointerInput pointerInput;
        private readonly TreeGrab grab;
        private readonly TreeFocusKeeper focusKeeper;
        private readonly Action updateHints;
        private readonly Action<Action, DispatcherPriority> postWhileAttached;

        private readonly AddPanel addPanel = new();

        private bool addOpenedBesideTheCursor;

        internal TreeMenus(TreeViewContext context, TreeRowReveal reveal, TreePointerInput pointerInput, TreeGrab grab, TreeFocusKeeper focusKeeper,
            Action updateHints, Action<Action, DispatcherPriority> postWhileAttached)
        {
            this.context = context;
            this.reveal = reveal;
            this.pointerInput = pointerInput;
            this.grab = grab;
            this.focusKeeper = focusKeeper;
            this.updateHints = updateHints;
            this.postWhileAttached = postWhileAttached;
        }

        internal AddPanel AddPanel => addPanel;

        internal bool IsMenuOpen() => context.TreeView.ContextMenu?.IsOpen == true || addPanel.IsOpen;

        internal void Attach(TreeView treeView)
        {
            treeView.AddHandler(Control.ContextRequestedEvent, OnContextRequested, RoutingStrategies.Bubble);
            addPanel.Closed += OnAddPanelClosed;
        }

        private void OnAddPanelClosed(object? sender, EventArgs e)
        {
            updateHints();
            // A panel beside the cursor row is a leaf of the tree, so the cursor row takes the focus back.
            if (addOpenedBesideTheCursor)
            {
                addOpenedBesideTheCursor = false;
                if (context.Tree?.CursorNode is { } cursor)
                    postWhileAttached(() => focusKeeper.FocusRow(cursor), DispatcherPriority.Input);
            }
        }

        internal void OpenAddBesideTheCursor(ProjectTree tree)
        {
            if (tree.CursorNode is not { } node || reveal.ShownItemOf(node) is not { } item || TopLevel.GetTopLevel(context.TreeView) is not { } window)
                return;

            var row = TreeRows.RowBorderOf(item);
            var anchor = row?.TranslatePoint(default, window) is { } topLeft ? new Rect(topLeft, row.Bounds.Size) : (Rect?)null;
            addPanel.OpenWithTip(item, PopupSide.Right, tree.GetAddGroups(node), AddTitle, anchor);
            addOpenedBesideTheCursor = addPanel.IsOpen;
            updateHints();
        }

        // With no target, or a target row that is not shown, the panel opens under the pointer.
        internal void OpenAddAtTarget(ProjectTree tree)
        {
            var treeView = context.TreeView;
            var point = tree.TargetNode is { } node && reveal.ShownItemOf(node) is { } item ? TreeRowAnchor.CenterOf(item, treeView) : null;
            addPanel.OpenAtPoint(treeView, point ?? pointerInput.LastPointer, tree.GetAddGroups(tree.TargetNode), AddTitle);
            updateHints();
        }

        // A target row that is not shown opens no menu.
        internal void OpenMenuAtTarget(ProjectTree tree)
        {
            if (tree.TargetNode is not { } node)
                OpenMenuAtPointer(null);
            else if (reveal.ShownItemOf(node) is { } item)
                OpenMenuUnderRow(item);
        }

        // A menu set during the event does not open on that event, so it opens here.
        // No row holds a menu, because a request goes up through every parent row.
        private void OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            if (grab.TakeMenuBlock())
            {
                e.Handled = true;
                return;
            }

            if (e.TryGetPosition(context.TreeView, out _))
            {
                if (TreeRows.ItemOf(e.Source)?.DataContext is TreeNode node && context.Tree is { } tree)
                    tree.CursorNode = node;

                if (OpenMenuAtPointer(e.Source))
                    e.Handled = true;
                return;
            }

            var item = TreeRows.ItemOf(e.Source) ?? (context.Tree?.TargetNode is { } target ? reveal.ShownItemOf(target) : null);
            if (OpenMenuUnderRow(item))
                e.Handled = true;
        }

        private bool OpenMenuAtPointer(object? source)
        {
            if (PrepareMenu(TreeRows.ItemOf(source)) is not { } menu)
                return false;

            menu.Open(context.TreeView);
            return true;
        }

        // A source that is not a row opens the menu of the project at the pointer.
        private bool OpenMenuUnderRow(object? source)
        {
            var treeView = context.TreeView;
            var item = TreeRows.ItemOf(source);
            if (PrepareMenu(item) is not { } menu)
                return false;

            if (item != null && TreeRowAnchor.FromNameOf(item, treeView) is { } name)
            {
                // The popup is larger than the panel by the margin of the shadow.
                menu.PlacementTarget = treeView;
                menu.Placement = PlacementMode.BottomEdgeAlignedLeft;
                menu.PlacementRect = name;
                menu.HorizontalOffset = -PopupFrame.ShadowMargin.Left;
                menu.VerticalOffset = -PopupFrame.ShadowMargin.Top;
            }

            menu.Open(treeView);
            return true;
        }

        private ContextMenu? PrepareMenu(TreeViewItem? item)
        {
            var treeView = context.TreeView;
            treeView.ContextMenu?.Close();

            var menu = item?.DataContext is TreeNode node && context.Tree is { } tree ? CreateMenu(item, node, tree)
                : item == null && context.Tree is { } project ? CreateSpaceMenu(project)
                : null;
            treeView.ContextMenu = menu;
            if (menu == null)
                return null;

            menu.Opened += OnMenuOpenedOrClosed;
            menu.Closed += OnMenuOpenedOrClosed;
            return menu;
        }

        private void OnMenuOpenedOrClosed(object? sender, RoutedEventArgs e) => updateHints();

        private ContextMenu CreateSpaceMenu(ProjectTree tree) =>
            PrefabRowMenu.Create(context.TreeView, RowMenuKind.Group, () => null, Array.Empty<RowMenuAdd>(), () => "Project",
                addRow: new AddMenuRow(addPanel, () => tree.GetAddGroups(null)),
                gestureOf: command => tree.Shortcuts[command]);

        // The menu opens with the Add row under the pointer until the user picks another row (see MenuAnchor).
        private ContextMenu? CreateMenu(TreeViewItem row, TreeNode node, ProjectTree tree)
        {
            var kind = node.Item == null ? RowMenuKind.Group
                : node.Item is CompositionModel or LayerModel ? RowMenuKind.Composable
                : RowMenuKind.Slot;

            return PrefabRowMenu.Create(
                row,
                kind,
                () => node.Owner != null && node.Item != null ? (node.Owner, node.Item) : null,
                Array.Empty<RowMenuAdd>(),
                () => node.Header,
                addRow: new AddMenuRow(addPanel, () => tree.GetAddGroups(node)),
                gestureOf: command => tree.Shortcuts[command]);
        }
    }
}
