// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The menus of the tree: a right click, the Menu key, W and Shift+A.
    internal sealed class ListTreeMenus : ListTreeBehavior
    {
        private const string AddTitle = "Add";

        private ProjectTree? tree;

        private Control Host => Context.Scroller;

        private AddPanel AddPanel => Context.AddPanel;

        private ListTreeRowReveal Reveal => Part<ListTreeRowReveal>();

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            Host.AddHandler(Control.ContextRequestedEvent, OnContextRequested, RoutingStrategies.Bubble);
        }

        // A set menu adds its own handler, so the menu goes too, and the handler of the next watch runs first.
        protected override void OnUnwatch()
        {
            var host = Host;
            host.RemoveHandler(Control.ContextRequestedEvent, OnContextRequested);
            host.ContextMenu?.Close();
            host.ContextMenu = null;
            tree = null;
        }

        // With no target, or a target row that is not shown, the panel opens under the pointer.
        public void OpenAddAtTarget(ProjectTree target)
        {
            var point = target.TargetNode is { } node && Reveal.ShownRowOf(node) is { } row ? ListTreeRowAnchor.CenterOf(row, Host) : null;
            AddPanel.OpenAtPoint(Host, point ?? Part<ListTreeLastPointer>().Last, target.GetAddGroups(target.TargetNode), AddTitle);
        }

        // A target row that is not shown opens no menu.
        public void OpenMenuAtTarget(ProjectTree target)
        {
            if (target.TargetNode is not { } node)
                OpenMenuAtPointer(null);
            else if (Reveal.ShownRowOf(node) is { } row)
                OpenMenuUnderRow(row);
        }

        // A menu set during the event does not open on that event, so it opens here.
        private void OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            if (Part<ListTreeGrab>().TakeMenuBlock())
            {
                e.Handled = true;
                return;
            }

            if (e.TryGetPosition(Host, out _))
            {
                if (RowListTree.Of(e.Source)?.DataContext is TreeNode node && tree != null)
                    tree.CursorNode = node;

                if (OpenMenuAtPointer(RowListTree.Of(e.Source)))
                    e.Handled = true;
                return;
            }

            var row = RowListTree.Of(e.Source) ?? (tree?.TargetNode is { } target ? Reveal.ShownRowOf(target) : null);
            if (OpenMenuUnderRow(row))
                e.Handled = true;
        }

        private bool OpenMenuAtPointer(RowListTree? row)
        {
            if (PrepareMenu(row) is not { } menu)
                return false;

            menu.Open(Host);
            return true;
        }

        // No row opens the menu of the project at the pointer.
        private bool OpenMenuUnderRow(RowListTree? row)
        {
            if (PrepareMenu(row) is not { } menu)
                return false;

            if (row != null && ListTreeRowAnchor.FromNameOf(row, Host) is { } name)
            {
                // The popup is larger than the panel by the margin of the shadow.
                menu.PlacementTarget = Host;
                menu.Placement = PlacementMode.BottomEdgeAlignedLeft;
                menu.PlacementRect = name;
                menu.HorizontalOffset = -PopupFrame.ShadowMargin.Left;
                menu.VerticalOffset = -PopupFrame.ShadowMargin.Top;
            }

            menu.Open(Host);
            return true;
        }

        private ContextMenu? PrepareMenu(RowListTree? row)
        {
            Host.ContextMenu?.Close();

            var menu = row?.DataContext is TreeNode node && tree != null ? CreateMenu(row, node, tree)
                : row == null && tree != null ? CreateSpaceMenu(tree)
                : null;
            Host.ContextMenu = menu;
            if (menu == null)
                return null;

            menu.Opened += OnMenuOpenedOrClosed;
            menu.Closed += OnMenuOpenedOrClosed;
            return menu;
        }

        private void OnMenuOpenedOrClosed(object? sender, RoutedEventArgs e) => Part<ListTreeHints>().Update();

        private ContextMenu CreateSpaceMenu(ProjectTree project) =>
            PrefabRowMenu.Create(Host, RowMenuKind.Group, () => null, Array.Empty<RowMenuAdd>(), () => "Project",
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
    }
}
