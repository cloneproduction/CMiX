// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Behaviors.ListTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views
{
    // The menus of the new tree: a right click, the Menu key, W and Shift+A, with the items and the places of TreeMenus in the old tree.
    internal sealed class ListTreeMenus
    {
        private const string AddTitle = "Add";

        private readonly Control host;
        private readonly ListTreeRowReveal reveal;
        private readonly ListTreeAddBeside addBeside;
        private readonly ListTreeLastPointer pointer;
        private readonly Action updateHints;

        private ProjectTree? tree;

        public ListTreeMenus(Control host, ListTreeRowReveal reveal, ListTreeAddBeside addBeside, ListTreeLastPointer pointer, Action updateHints)
        {
            this.host = host;
            this.reveal = reveal;
            this.addBeside = addBeside;
            this.pointer = pointer;
            this.updateHints = updateHints;
        }

        private AddPanel AddPanel => addBeside.AddPanel;

        public void Attach(ProjectTree attached)
        {
            tree = attached;
            host.AddHandler(Control.ContextRequestedEvent, OnContextRequested, RoutingStrategies.Bubble);
        }

        // A set menu adds its own handler, so the menu goes too, and the handler of the next Attach runs first.
        public void Detach()
        {
            host.RemoveHandler(Control.ContextRequestedEvent, OnContextRequested);
            host.ContextMenu?.Close();
            host.ContextMenu = null;
            tree = null;
        }

        // With no target, or a target row that is not shown, the panel opens under the pointer.
        public void OpenAddAtTarget(ProjectTree target)
        {
            var point = target.TargetNode is { } node && reveal.ShownRowOf(node) is { } row ? ListTreeRowAnchor.CenterOf(row, host) : null;
            AddPanel.OpenAtPoint(host, point ?? pointer.Last, target.GetAddGroups(target.TargetNode), AddTitle);
            updateHints();
        }

        // A target row that is not shown opens no menu.
        public void OpenMenuAtTarget(ProjectTree target)
        {
            if (target.TargetNode is not { } node)
                OpenMenuAtPointer(null);
            else if (reveal.ShownRowOf(node) is { } row)
                OpenMenuUnderRow(row);
        }

        // A menu set during the event does not open on that event, so it opens here.
        private void OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            if (e.TryGetPosition(host, out _))
            {
                if (RowOf(e.Source)?.DataContext is TreeNode node && tree != null)
                    tree.CursorNode = node;

                if (OpenMenuAtPointer(RowOf(e.Source)))
                    e.Handled = true;
                return;
            }

            var row = RowOf(e.Source) ?? (tree?.TargetNode is { } target ? reveal.ShownRowOf(target) : null);
            if (OpenMenuUnderRow(row))
                e.Handled = true;
        }

        private bool OpenMenuAtPointer(RowListTree? row)
        {
            if (PrepareMenu(row) is not { } menu)
                return false;

            menu.Open(host);
            return true;
        }

        // No row opens the menu of the project at the pointer.
        private bool OpenMenuUnderRow(RowListTree? row)
        {
            if (PrepareMenu(row) is not { } menu)
                return false;

            if (row != null && ListTreeRowAnchor.FromNameOf(row, host) is { } name)
            {
                // The popup is larger than the panel by the margin of the shadow.
                menu.PlacementTarget = host;
                menu.Placement = PlacementMode.BottomEdgeAlignedLeft;
                menu.PlacementRect = name;
                menu.HorizontalOffset = -PopupFrame.ShadowMargin.Left;
                menu.VerticalOffset = -PopupFrame.ShadowMargin.Top;
            }

            menu.Open(host);
            return true;
        }

        private ContextMenu? PrepareMenu(RowListTree? row)
        {
            host.ContextMenu?.Close();

            var menu = row?.DataContext is TreeNode node && tree != null ? CreateMenu(row, node, tree)
                : row == null && tree != null ? CreateSpaceMenu(tree)
                : null;
            host.ContextMenu = menu;
            if (menu == null)
                return null;

            menu.Opened += OnMenuOpenedOrClosed;
            menu.Closed += OnMenuOpenedOrClosed;
            return menu;
        }

        private void OnMenuOpenedOrClosed(object? sender, RoutedEventArgs e) => updateHints();

        private ContextMenu CreateSpaceMenu(ProjectTree project) =>
            PrefabRowMenu.Create(host, RowMenuKind.Group, () => null, Array.Empty<RowMenuAdd>(), () => "Project",
                addRow: new AddMenuRow(AddPanel, () => project.GetAddGroups(null)),
                gestureOf: command => project.Shortcuts[command]);

        private ContextMenu CreateMenu(RowListTree row, TreeNode node, ProjectTree project)
        {
            return PrefabRowMenu.Create(
                row,
                ListTreeMenuKind.Of(node),
                () => node.Owner != null && node.Item != null ? (node.Owner, node.Item) : null,
                Array.Empty<RowMenuAdd>(),
                () => node.Header,
                addRow: new AddMenuRow(AddPanel, () => project.GetAddGroups(node)),
                gestureOf: command => project.Shortcuts[command]);
        }

        private static RowListTree? RowOf(object? source) => (source as Visual)?.FindAncestorOfType<RowListTree>(includeSelf: true);
    }
}
