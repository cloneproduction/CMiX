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
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ComposableManagerItem : UserControl
    {
        public ComposableManagerItem()
        {
            InitializeComponent();
            ContextMenu = BuildContextMenu();
        }

        private MenuItem _moveUp = null!;
        private MenuItem _moveDown = null!;
        private MenuItem _moveToTop = null!;
        private MenuItem _moveToBottom = null!;

        // The menu of a row. It acts on the item of the row, not on the selected item.
        // The list box that holds the row has the manager of the list as its data context.
        private ContextMenu BuildContextMenu()
        {
            MenuItem Row(string header, Action<PrefabManager, IControl> action, bool deferred = true)
            {
                var row = new MenuItem { Header = header };
                row.Click += (_, _) =>
                {
                    if (Target() is not { } target) return;
                    // Posted so the popup closes before the bound list changes.
                    if (deferred) Dispatcher.UIThread.Post(() => action(target.Manager, target.Item));
                    else action(target.Manager, target.Item);
                };
                return row;
            }

            var duplicate = Row("Duplicate", (manager, item) => manager.DuplicateItem(item));
            var rename = Row("Rename", (_, _) => editableTextBox.IsEditing = true);
            _moveUp = Row("Move Up", (manager, item) => MoveBy(manager, item, -1));
            _moveDown = Row("Move Down", (manager, item) => MoveBy(manager, item, 1));
            _moveToTop = Row("Move To Top", (manager, item) => manager.MoveItemTo(item, 0));
            _moveToBottom = Row("Move To Bottom", (manager, item) => manager.MoveItemTo(item, manager.ManagerData.Items.Count - 1));
            var delete = Row("Delete", (manager, item) => manager.DeleteItem(item));

            var menu = new ContextMenu
            {
                ItemsSource = new object[]
                {
                    duplicate, rename,
                    new Separator(),
                    _moveUp, _moveDown, _moveToTop, _moveToBottom,
                    new Separator(),
                    delete
                }
            };
            menu.Opening += (_, _) => RefreshMoveRows();
            if (this.TryFindResource("ContextMenuDefault", out var theme) && theme is global::Avalonia.Styling.ControlTheme controlTheme)
                menu.Theme = controlTheme;
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
