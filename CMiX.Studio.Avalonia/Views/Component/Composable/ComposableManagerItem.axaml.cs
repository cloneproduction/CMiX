// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ComposableManagerItem : UserControl
    {
        public ComposableManagerItem()
        {
            InitializeComponent();
            // The menu is built once the row is in the window, because the theme of the menu comes from the resources of the app.
            AttachedToVisualTree += (_, _) => ContextMenu ??= BuildContextMenu();
        }

        private MenuItem _moveUp = null!;
        private MenuItem _moveDown = null!;
        private MenuItem _moveToTop = null!;
        private MenuItem _moveToBottom = null!;

        // The ids of the rows (see MenuAnchor.Id). The labels can change without touching them.
        private const string DuplicateId = "composable.duplicate";
        private const string RenameId = "composable.rename";
        private const string MoveUpId = "composable.move-up";
        private const string MoveDownId = "composable.move-down";
        private const string MoveToTopId = "composable.move-to-top";
        private const string MoveToBottomId = "composable.move-to-bottom";
        private const string DeleteId = "composable.delete";

        // The menu of a row, in the look of the other menus (see TitledContextMenu). It starts with the name
        // of the item and acts on the item of the row, not on the selected item.
        // The list box that holds the row has the manager of the list as its data context.
        private ContextMenu BuildContextMenu()
        {
            var builder = new TitledContextMenu(this, () => (DataContext as IPrefab)?.PrefabService.Name.Value?.Trim() ?? string.Empty, DuplicateId);

            MenuItem Row(string id, string header, Action<PrefabManager, IControl> action) =>
                builder.Action(id, header, () =>
                {
                    if (Target() is not { } target) return;
                    // Posted so the popup closes before the bound list changes.
                    Dispatcher.UIThread.Post(() => action(target.Manager, target.Item));
                });

            Row(DuplicateId, "Duplicate", (manager, item) => manager.DuplicateItem(item));
            Row(RenameId, "Rename", (_, _) => editableTextBox.IsEditing = true);
            builder.Line();
            _moveUp = Row(MoveUpId, "Move Up", (manager, item) => MoveBy(manager, item, -1));
            _moveDown = Row(MoveDownId, "Move Down", (manager, item) => MoveBy(manager, item, 1));
            _moveToTop = Row(MoveToTopId, "Move To Top", (manager, item) => manager.MoveItemTo(item, 0));
            _moveToBottom = Row(MoveToBottomId, "Move To Bottom", (manager, item) => manager.MoveItemTo(item, manager.ManagerData.Items.Count - 1));
            builder.Line();
            Row(DeleteId, "Delete", (manager, item) => manager.DeleteItem(item));

            var menu = builder.Build();
            menu.Opening += (_, _) => RefreshMoveRows();
            return menu;
        }

        private (PrefabManager Manager, IControl Item)? Target() =>
            DataContext is IControl item && this.FindAncestorOfType<ListBox>()?.DataContext is PrefabManager manager
                ? (manager, item)
                : null;

        private static void MoveBy(PrefabManager manager, IControl item, int step)
        {
            var index = manager.ManagerData.Items.IndexOf(item);
            if (index >= 0) manager.MoveItemTo(item, index + step);
        }

        // A move row that cannot move the item is disabled.
        private void RefreshMoveRows()
        {
            var target = Target();
            var index = target is { } t ? t.Manager.ManagerData.Items.IndexOf(t.Item) : -1;
            var last = target is { } l ? l.Manager.ManagerData.Items.Count - 1 : -1;
            _moveUp.IsEnabled = _moveToTop.IsEnabled = index > 0;
            _moveDown.IsEnabled = _moveToBottom.IsEnabled = index >= 0 && index < last;
        }

        public static readonly StyledProperty<bool> DragHandleIsPressedProperty =
            AvaloniaProperty.Register<ComposableManagerItem, bool>(nameof(DragHandleIsPressed), false, defaultBindingMode: BindingMode.TwoWay);
        public bool DragHandleIsPressed
        {
            get => GetValue(DragHandleIsPressedProperty);
            set => SetValue(DragHandleIsPressedProperty, value);
        }

        private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Control control && e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                DragHandleIsPressed = true;
        }

        private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
                DragHandleIsPressed = false;
        }
    }
}
