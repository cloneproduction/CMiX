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
    // The parts of the view that the menus of the tree use.
    internal sealed record TreeMenuInputs(Func<TreeNode, TreeViewItem?> ShownItemOf, Func<Point> LastPointer, Func<bool> TakeMenuBlock,
        Action<TreeNode> FocusRow);

    // The menus of the rows and of the space under them, and the one Add panel that every menu and key opens.
    internal sealed class TreeMenus
    {
        // The title of the Add panel when no row opens it.
        private const string AddTitle = "Add";

        private readonly TreeViewContext context;
        private readonly TreeMenuInputs inputs;
        private readonly Action updateHints;
        private readonly Action<Action, DispatcherPriority> postWhileAttached;

        // The panel of the Add row of the menus of the rows. One panel serves every menu.
        private readonly AddPanel addPanel = new();

        // True while the Add panel that opened beside the cursor row is open (see OpenAddBesideTheCursor).
        private bool addOpenedBesideTheCursor;

        internal TreeMenus(TreeViewContext context, TreeMenuInputs inputs, Action updateHints, Action<Action, DispatcherPriority> postWhileAttached)
        {
            this.context = context;
            this.inputs = inputs;
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
            // A panel opened beside the cursor row (Right, Ctrl+Right) is a leaf of the tree: when it closes, Left or Escape or a pick,
            // the cursor row takes the focus back, so the keys go on in the tree.
            if (addOpenedBesideTheCursor)
            {
                addOpenedBesideTheCursor = false;
                if (context.Tree?.CursorNode is { } cursor)
                    postWhileAttached(() => inputs.FocusRow(cursor), DispatcherPriority.Input);
            }
        }

        // The Add panel is the leaf after the last row of a branch: it opens at the right of the cursor row, its search field level with
        // the row and a tip at the middle of the row.
        internal void OpenAddBesideTheCursor(ProjectTree tree)
        {
            if (tree.CursorNode is not { } node || inputs.ShownItemOf(node) is not { } item || TopLevel.GetTopLevel(context.TreeView) is not { } window)
                return;

            var row = TreeRows.RowBorderOf(item);
            var anchor = row?.TranslatePoint(default, window) is { } topLeft ? new Rect(topLeft, row.Bounds.Size) : (Rect?)null;
            addPanel.OpenWithTip(item, PopupSide.Right, tree.GetAddGroups(node), AddTitle, anchor);
            addOpenedBesideTheCursor = addPanel.IsOpen;
            updateHints();
        }

        // Tree.Add, a key: the Add panel of the target row (ProjectTree.TargetNode) opens with the middle of its search field on the middle
        // of that row, and the title Add. With no target, or a target row that is not shown, it opens under the pointer. With no target it
        // holds what the project takes, a composition. Nothing is added until the user picks an entry of the panel.
        internal void OpenAddAtTarget(ProjectTree tree)
        {
            var treeView = context.TreeView;
            var point = tree.TargetNode is { } node && inputs.ShownItemOf(node) is { } item ? TreeRowAnchor.CenterOf(item, treeView) : null;
            addPanel.OpenAtPoint(treeView, point ?? inputs.LastPointer(), tree.GetAddGroups(tree.TargetNode), AddTitle);
            updateHints();
        }

        // Tree.Menu, a key: the menu of the target row opens under that row (see OpenMenuUnderRow). With no target, it is the menu of the project,
        // at the pointer. A target row that is not shown (its parent is collapsed) opens no menu.
        internal void OpenMenuAtTarget(ProjectTree tree)
        {
            if (tree.TargetNode is not { } node)
                OpenMenuAtPointer(null);
            else if (inputs.ShownItemOf(node) is { } item)
                OpenMenuUnderRow(item);
        }

        // The menu of the row under the pointer, built and opened when it is asked for. A menu that is set during the event
        // does not open on that event, so it is opened here.
        // The tree listens in the bubble phase, as the menu of a row of a list does. A name that is being edited
        // blocks the request in the tunnel phase (ContextMenuGuard), so a right click during a rename opens no menu.
        // The menu belongs to the tree and no row holds one. A row with a menu would open it for the right clicks on its
        // children too, because a request goes up through every parent row.
        // A right click has a place of the pointer: the menu opens at the pointer, and the cursor goes to the clicked row, also when the
        // row is already selected. The Menu key has no place: the menu of the focused row, else of the target row, opens
        // under that row.
        private void OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            // A click of the grab asks for no menu: the grab takes it (see TreeGrab).
            if (inputs.TakeMenuBlock())
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

            var item = TreeRows.ItemOf(e.Source) ?? (context.Tree?.TargetNode is { } target ? inputs.ShownItemOf(target) : null);
            if (OpenMenuUnderRow(item))
                e.Handled = true;
        }

        // On the space under the rows, the menu is the menu of the project, which has the one entry that the project takes, a composition.
        // Both menus add through the Add panel. Returns false when there is no menu.
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
                // The panel goes under the row, with its left edge on the left edge of the name. The popup is larger than the panel by
                // the margin of the shadow.
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

        // The menu of the empty space of the tree: its Add row opens the panel of the project.
        private ContextMenu CreateSpaceMenu(ProjectTree tree) =>
            PrefabRowMenu.Create(context.TreeView, RowMenuKind.Group, () => null, Array.Empty<RowMenuAdd>(), () => "Project",
                addRow: new AddMenuRow(addPanel, () => tree.GetAddGroups(null)),
                gestureOf: command => tree.Shortcuts[command]);

        // The menu of a row. Its Add row opens the Add panel beside the menu, with what the row takes. The menu opens with the Add row
        // under the pointer, on every row, until the user picks another row of a menu: then that row is under the pointer (see MenuAnchor).
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
