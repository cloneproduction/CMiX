// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Controls.String;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    // The menu of a row of a list of prefabs, in the look of the other menus (see TitledContextMenu). It starts
    // with the name of the item and acts on the item of the row, not on the selected item.
    // The list box that holds the row has the manager of the list as its data context.
    // The caller picks the rows and the lines between them.
    public sealed class PrefabRowMenu
    {
        // The ids of the rows (see MenuAnchor.Id). The labels can change without touching them.
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
        private readonly TitledContextMenu _builder;
        private readonly List<MenuItem> _towardsTheStart = new();
        private readonly List<MenuItem> _towardsTheEnd = new();

        private PrefabRowMenu(Control row,Func<TextBoxEditable?> nameBox)
        {
            _row = row;
            _nameBox = nameBox;
            _builder = new TitledContextMenu(row, () => (row.DataContext as IPrefab)?.PrefabService.Name.Value?.Trim() ?? string.Empty, DuplicateId);
        }

        // Gives a row of a list the menu of the kind of the list. The menu is built once the row is in the window,
        // because the theme of the menu comes from the resources of the app.
        // The name box is the box that Rename edits. By default it is the first one in the row.
        public static void AttachTo(Control row, RowMenuKind kind, Func<TextBoxEditable?>? nameBox = null)
        {
            if (kind == RowMenuKind.None || row.ContextMenu != null) return;

            nameBox ??= () => row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault();

            if (row.IsAttachedToVisualTree())
                row.ContextMenu = new PrefabRowMenu(row, nameBox).BuildFor(kind);
            else
                row.AttachedToVisualTree += (_, _) => row.ContextMenu ??= new PrefabRowMenu(row, nameBox).BuildFor(kind);
        }

        private ContextMenu BuildFor(RowMenuKind kind)
        {
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

        private void Duplicate() => Row(DuplicateId, "Duplicate", (manager, item) => manager.DuplicateItem(item));

        private void Rename() => Row(RenameId, "Rename", (_, _) =>
        {
            if (_nameBox() is { } nameBox)
                nameBox.IsEditing = true;
        });

        private void Moves()
        {
            _towardsTheStart.Add(Row(MoveUpId, "Move Up", (manager, item) => MoveBy(manager, item, -1)));
            _towardsTheEnd.Add(Row(MoveDownId, "Move Down", (manager, item) => MoveBy(manager, item, 1)));
            _towardsTheStart.Add(Row(MoveToTopId, "Move To Top", (manager, item) => manager.MoveItemTo(item, 0)));
            _towardsTheEnd.Add(Row(MoveToBottomId, "Move To Bottom", (manager, item) => manager.MoveItemTo(item, manager.ManagerData.Items.Count - 1)));
        }

        private void Remove(string header) => Row(RemoveId, header, (manager, item) => manager.RemoveItem(item));

        private void RemoveEverywhere(string header) => Row(RemoveEverywhereId, header, (manager, item) => manager.RemoveEverywhere(item));

        private void Line() => _builder.Line();

        private ContextMenu Build()
        {
            var menu = _builder.Build();
            menu.Opening += (_, _) => RefreshMoveRows();
            return menu;
        }

        private MenuItem Row(string id, string header, Action<PrefabManager, IControl> action) =>
            _builder.Action(id, header, () =>
            {
                if (Target() is not { } target) return;
                // Posted so the popup closes before the bound list changes.
                Dispatcher.UIThread.Post(() => action(target.Manager, target.Item));
            });

        private (PrefabManager Manager, IControl Item)? Target() =>
            _row.DataContext is IControl item && _row.FindAncestorOfType<ListBox>()?.DataContext is PrefabManager manager
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
            foreach (var row in _towardsTheStart)
                row.IsEnabled = index > 0;
            foreach (var row in _towardsTheEnd)
                row.IsEnabled = index >= 0 && index < last;
        }
    }
}
