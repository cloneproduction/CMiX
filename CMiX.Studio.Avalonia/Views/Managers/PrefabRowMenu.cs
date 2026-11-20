// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Controls.String;
using CMiX.UI.Popups;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    // The menu of a row of a list of prefabs, in the look of the other menus (see ContextMenuTitled). It starts
    // with the name of the item and acts on the item of the row, not on the selected item.
    // The list box that holds the row has the manager of the list as its data context.
    // The caller picks the rows and the lines between them.
    public sealed class PrefabRowMenu
    {
        // The ids of the rows (see MenuAnchor.Id). The labels can change without touching them.
        private const string AddId = "row.add";
        private const string DuplicateId = "row.duplicate";
        private const string RenameId = "row.rename";
        private const string MoveUpId = "row.move-up";
        private const string MoveDownId = "row.move-down";
        private const string MoveToTopId = "row.move-to-top";
        private const string MoveToBottomId = "row.move-to-bottom";
        private const string RemoveId = "row.remove";
        private const string RemoveEverywhereId = "row.remove-everywhere";

        private readonly Control _row;
        private readonly Func<TextBoxEditable?> _nameBox;
        private readonly Func<(PrefabManager Manager, IControl Item)?> _target;
        private readonly IReadOnlyList<RowMenuAdd> _adds;
        private readonly AddMenuRow? _addRow;
        private readonly Func<string, KeyGesture?>? _gestureOf;
        private readonly ContextMenuTitled _builder;
        private readonly List<MenuItem> _towardsTheStart = new();
        private readonly List<MenuItem> _towardsTheEnd = new();
        private readonly List<MenuItem> _inertRows = new();
        private bool _inert;
        private MenuItem? _addItem;
        private ContextMenu? _menu;
        private DispatcherTimer? _openTimer;

        private PrefabRowMenu(Control row, Func<TextBoxEditable?> nameBox, Func<(PrefabManager Manager, IControl Item)?> target,
                              IReadOnlyList<RowMenuAdd> adds, Func<string>? title, AddMenuRow? addRow = null, Func<string, KeyGesture?>? gestureOf = null)
        {
            _row = row;
            _nameBox = nameBox;
            _target = target;
            _adds = adds;
            _addRow = addRow;
            _gestureOf = gestureOf;
            // The menu opens on the row that was used last, if it has it. Before that it opens on the first row that adds, so a click
            // right after the creation of an item can add what goes in it (a layer in a composition). Rows without such an add open on Duplicate.
            // The Add row of the panel counts as such an add.
            _builder = new ContextMenuTitled(row, title ?? (() => (row.DataContext as IPrefab)?.PrefabService.Name.Value?.Trim() ?? string.Empty),
                                             addRow is { Primary: true } ? AddId : adds.FirstOrDefault()?.Id ?? DuplicateId);
        }

        // Gives a row of a list the menu of the kind of the list. The menu is built once the row is in the window,
        // because the theme of the menu comes from the resources of the app.
        // The name box is the box that Rename edits. By default it is the first one in the row.
        public static void AttachTo(Control row, RowMenuKind kind, Func<TextBoxEditable?>? nameBox = null)
        {
            if (kind == RowMenuKind.None || row.ContextMenu != null) return;

            nameBox ??= () => row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault();

            if (row.IsAttachedToVisualTree())
                row.ContextMenu = new PrefabRowMenu(row, nameBox, () => ListTarget(row), Array.Empty<RowMenuAdd>(), null).BuildFor(kind);
            else
                row.AttachedToVisualTree += (_, _) => row.ContextMenu ??= new PrefabRowMenu(row, nameBox, () => ListTarget(row), Array.Empty<RowMenuAdd>(), null).BuildFor(kind);
        }

        // Makes the menu of a row that is not in a list box, such as a row of the project tree. The target says which item
        // the menu acts on, and in which list it is. The adds are the rows at the top. The title is the first line of the menu.
        public static ContextMenu Create(Control row, RowMenuKind kind, Func<(PrefabManager Manager, IControl Item)?> target,
                                         IReadOnlyList<RowMenuAdd> adds, Func<string> title, Func<TextBoxEditable?>? nameBox = null,
                                         AddMenuRow? addRow = null, Func<string, KeyGesture?>? gestureOf = null)
        {
            nameBox ??= () => row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault();
            var builder = new PrefabRowMenu(row, nameBox, target, adds, title, addRow, gestureOf);
            var menu = builder.BuildFor(kind);

            // The caller opens the menu in code, which does not raise Opening, and builds a new menu for each opening.
            builder.RefreshMoveRows();
            return menu;
        }

        private ContextMenu BuildFor(RowMenuKind kind)
        {
            if (_addRow != null)
                AddPanelRow();

            foreach (var add in _adds)
                Add(add);

            if (_adds.Count > 0 || _addRow != null)
                Line();

            // A group has no item of its own, so it shows the rows of an item greyed, and they do nothing.
            _inert = kind == RowMenuKind.Group;
            if (_inert)
                kind = RowMenuKind.Slot;

            Duplicate();
            Rename();
            Line();
            switch (kind)
            {
                case RowMenuKind.Slot:
                    Moves();
                    Line();
                    Remove("Remove");
                    break;
                case RowMenuKind.Composable:
                    Moves();
                    Line();
                    Remove("Delete");
                    break;
                case RowMenuKind.Repository:
                    RemoveEverywhere("Delete Everywhere");
                    break;
            }
            return Build();
        }

        // A row that adds one thing. Posted so the popup closes before the bound list changes.
        private void Add(RowMenuAdd add) =>
            Iconed(_builder.Action(add.Id, add.Header, () => Dispatcher.UIThread.Post(() => add.Execute?.Invoke())), "Add");

        // The time the pointer rests on the Add row before the panel opens beside the menu.
        private static readonly TimeSpan PanelDelay = TimeSpan.FromMilliseconds(250);

        // The Add row opens the Add panel beside the menu. The pointer resting on the row, a click, Enter and the Right key open it.
        // The menu stays open while the panel is open. The row does not close the menu itself.
        private void AddPanelRow()
        {
            _addItem = Iconed(_builder.Action(AddId, new MenuItem { Header = "Add", StaysOpenOnClick = true }), "Add");
            _addItem.Classes.Add("opens");
            ShowShortcut(_addItem, "Tree.Add");
            _addItem.Click += (_, _) => OpenAddPanel();
            _addItem.PointerEntered += (_, _) => ScheduleAddPanel();
            _addItem.PointerExited += (_, _) =>
            {
                if (_addRow?.Panel.IsOpen != true)
                    StopTimer();
            };
            _addItem.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Right)
                {
                    OpenAddPanel();
                    e.Handled = true;
                }
            };
        }

        private void ScheduleAddPanel()
        {
            if (_openTimer == null)
            {
                _openTimer = new DispatcherTimer { Interval = PanelDelay };
                _openTimer.Tick += (_, _) =>
                {
                    StopTimer();
                    OpenAddPanel();
                };
            }

            _openTimer.Stop();
            _openTimer.Start();
        }

        private void StopTimer() => _openTimer?.Stop();

        private void CloseAddPanel()
        {
            StopTimer();
            _addRow?.Panel.Close();
        }

        // Opens the panel on the right of the menu, with its search field on the Add row. The panel keeps away from the card of the menu,
        // not from the row, because the row is inside the padding of the card.
        private void OpenAddPanel()
        {
            StopTimer();
            if (_addRow == null || _addItem == null || _menu is not { IsOpen: true } || _addRow.Panel.IsOpen)
                return;

            var groups = _addRow.Groups();
            if (groups.Count == 0)
                return;

            _addRow.Panel.Open(_addItem, PopupSide.Right, groups, null, CardEdgeRect(_addItem), _addItem.FindAncestorOfType<PopupFrame>());
            ContextMenuPointerAway.SetCompanion(_menu, _addRow.Panel.Card);
        }

        // The rectangle of the row, made wider up to the right edge of the visible card of the menu.
        private static Rect CardEdgeRect(Control row)
        {
            var rect = PopupAnchoring.RectOf(row);
            if (row.FindAncestorOfType<PopupFrame>() is not { } frame || TopLevel.GetTopLevel(row) is not { } top
                || frame.TranslatePoint(new Point(frame.Bounds.Width, 0), top) is not { } corner)
                return rect;

            var edge = corner.X - PopupFrame.ShadowMargin.Right;
            return new Rect(rect.X, rect.Y, Math.Max(rect.Width, edge - rect.X), rect.Height);
        }

        // The shortcut of a command, on the right of its row. Only a menu that is given the gestures shows them (the menus of the tree).
        private MenuItem ShowShortcut(MenuItem row, string command)
        {
            if (_gestureOf?.Invoke(command) is { } gesture)
                row.InputGesture = gesture;

            return row;
        }
        private void Duplicate() => ShowShortcut(Row(DuplicateId, "Duplicate", "Duplicate", (manager, item) => manager.DuplicateItem(item)), "Tree.Duplicate");

        private void Rename() => ShowShortcut(Row(RenameId, "Rename", "Rename", (_, _) =>
        {
            if (_nameBox() is { } nameBox)
                nameBox.IsEditing = true;
        }), "Tree.Rename");

        private void Moves()
        {
            _towardsTheStart.Add(ShowShortcut(Row(MoveUpId, "Move Up", "ArrowUp", (manager, item) => MoveBy(manager, item, -1)), "Tree.MoveUp"));
            _towardsTheEnd.Add(ShowShortcut(Row(MoveDownId, "Move Down", "ArrowDown", (manager, item) => MoveBy(manager, item, 1)), "Tree.MoveDown"));
            _towardsTheStart.Add(Row(MoveToTopId, "Move To Top", "MoveToTop", (manager, item) => manager.MoveItemTo(item, 0)));
            _towardsTheEnd.Add(Row(MoveToBottomId, "Move To Bottom", "MoveToBottom", (manager, item) => manager.MoveItemTo(item, manager.ManagerData.Items.Count - 1)));
        }

        private void Remove(string header) => ShowShortcut(Row(RemoveId, header, "Remove", (manager, item) => manager.RemoveItem(item)), "Tree.Remove");

        private void RemoveEverywhere(string header) => Row(RemoveEverywhereId, header, "Remove", (manager, item) => manager.RemoveEverywhere(item));

        private void Line() => _builder.Line();

        private ContextMenu Build()
        {
            var menu = _builder.Build();
            foreach (var row in _inertRows)
                row.IsEnabled = false;

            _menu = menu;
            menu.Opening += (_, _) => RefreshMoveRows();
            if (_addRow != null)
                WireAddPanel(menu, _addRow);

            return menu;
        }

        // The panel and the menu close together. A pick or a click away from the panel closes the menu too. Escape in the panel, and
        // the pointer on another row, close only the panel. The menu looks after the panel only while it is open, because one panel
        // serves many menus.
        private void WireAddPanel(ContextMenu menu, AddMenuRow addRow)
        {
            var panel = addRow.Panel;
            EventHandler closeMenu = (_, _) => menu.Close();

            foreach (var item in menu.ItemsSource?.OfType<MenuItem>() ?? Enumerable.Empty<MenuItem>())
            {
                if (!ReferenceEquals(item, _addItem))
                    item.PointerEntered += (_, _) => CloseAddPanel();
            }

            menu.Opened += (_, _) =>
            {
                panel.Picked += closeMenu;
                panel.Dismissed += closeMenu;

                // The Add row can open under the pointer (see MenuAnchor), and then no enter comes.
                if (_addItem != null && MenuAnchor.GetIsAnchor(_addItem))
                    ScheduleAddPanel();
            };
            menu.Closed += (_, _) =>
            {
                panel.Picked -= closeMenu;
                panel.Dismissed -= closeMenu;
                CloseAddPanel();
                ContextMenuPointerAway.SetCompanion(menu, null);
            };
        }

        private MenuItem Row(string id, string header, string icon, Action<PrefabManager, IControl> action) =>
            Iconed(_builder.Action(id, header, () =>
            {
                if (Target() is not { } target) return;
                // Posted so the popup closes before the bound list changes.
                Dispatcher.UIThread.Post(() => action(target.Manager, target.Item));
            }), icon);

        // The icon of a row, at the size of the icons of the menus.
        private MenuItem Iconed(MenuItem row, string icon)
        {
            if (_inert)
                _inertRows.Add(row);

            row.Icon = new AppIcon { IconKey = icon, Size = _row.TryFindResource("IconSizeXS", out var size) && size is double number ? number : 16 };
            return row;
        }

        // The item of a row of a list box, and the manager of the list box.
        private static (PrefabManager Manager, IControl Item)? ListTarget(Control row) =>
            row.DataContext is IControl item && row.FindAncestorOfType<ListBox>()?.DataContext is PrefabManager manager
                ? (manager, item)
                : null;

        private (PrefabManager Manager, IControl Item)? Target() => _target();

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
            foreach (var row in _towardsTheStart)
                row.IsEnabled = index > 0;
            foreach (var row in _towardsTheEnd)
                row.IsEnabled = index >= 0 && index < last;
        }
    }
}
