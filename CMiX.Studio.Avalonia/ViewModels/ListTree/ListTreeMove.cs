// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The snapshot as if the source were dropped on the slot. The live tree does not change.
    public static class ListTreeMove
    {
        public static ListTreeSnapshot Of(ListTreeSnapshot snapshot, ListTreeRow source, ListTreeSlot slot)
        {
            var plan = PlanOf(source, slot);
            var holders = new List<ListTreeRowKey>();
            var moved = RowsOf(snapshot.Roots, plan, holders);
            return new ListTreeSnapshot(Opened(moved, OpenedBy(HoldersToOpen(plan, holders))));
        }

        // A move in its own list opens only the place of the slot. A move to another list opens every place of that list.
        private static IReadOnlyList<ListTreeRowKey> HoldersToOpen(MovePlan plan, IReadOnlyList<ListTreeRowKey> holders) =>
            ReferenceEquals(plan.From, plan.To) ? new[] { plan.Holder } : holders;

        private static MovePlan PlanOf(ListTreeRow source, ListTreeSlot slot)
        {
            var node = source.Node ?? throw new ArgumentException("The source is not a row of the live tree.", nameof(source));
            var item = node.Item!;
            var from = node.Owner!;
            var to = slot.ListPlace.List;
            return new MovePlan(item, from, to, IndexOf(item, from, slot.ListPlace), source, HolderOf(slot));
        }

        // The index is counted after the item leaves its list, as in TreeDropService.
        private static int IndexOf(IControl item, PrefabManager from, ListPlace place)
        {
            var oldIndex = from.ManagerData.Items.IndexOf(item);
            return ReferenceEquals(place.List, from) && place.Index > oldIndex ? place.Index - 1 : place.Index;
        }

        // The key of the row that shows the list of the slot.
        private static ListTreeRowKey HolderOf(ListTreeSlot slot)
        {
            if (slot.Position != DropPosition.Inside)
                return slot.ShownTarget.Parent!;

            return ReferenceEquals(slot.ShownTarget.Model, slot.ListPlace.List) ? slot.ShownTarget : new ListTreeRowKey(slot.ListPlace.List, slot.ShownTarget);
        }

        private static IReadOnlyList<ListTreeRow> RowsOf(IEnumerable<ListTreeRow> rows, MovePlan plan, List<ListTreeRowKey> holders) =>
            rows.Select(row => RowOf(row, plan, holders)).Where(IsShown).ToArray();

        private static ListTreeRow RowOf(ListTreeRow row, MovePlan plan, List<ListTreeRowKey> holders)
        {
            var children = RowsOf(row.Children.Where(child => !IsSourceIn(row, child, plan)), plan, holders);
            if (ReferenceEquals(row.Key.Model, plan.To))
                return row with { Children = WithMovedRow(row, children, plan, holders) };

            if (HasHiddenList(row, children, plan))
                return row with { Children = WithGroupRow(row, children, plan, holders) };

            return row with { Children = children };
        }

        private static bool IsSourceIn(ListTreeRow row, ListTreeRow child, MovePlan plan) =>
            ReferenceEquals(row.Key.Model, plan.From) && ReferenceEquals(child.Key.Model, plan.Item);

        private static IReadOnlyList<ListTreeRow> WithMovedRow(ListTreeRow holder, IReadOnlyList<ListTreeRow> children, MovePlan plan, List<ListTreeRowKey> holders)
        {
            holders.Add(holder.Key);
            var rows = children.ToList();
            rows.Insert(Math.Clamp(plan.Index, 0, rows.Count), MovedRowIn(holder, plan));
            return rows;
        }

        // In its own list, every place keeps its own row.
        private static ListTreeRow MovedRowIn(ListTreeRow holder, MovePlan plan)
        {
            if (holder.Children.FirstOrDefault(child => ReferenceEquals(child.Key.Model, plan.Item)) is { } own)
                return own.Key.Equals(plan.Source.Key) ? Closed(own) : own;

            return NewRowUnder(holder.Key, holder.Depth + 1, plan);
        }

        // The list has no row yet because option E hides an empty list.
        private static IReadOnlyList<ListTreeRow> WithGroupRow(ListTreeRow parent, IReadOnlyList<ListTreeRow> children, MovePlan plan, List<ListTreeRowKey> holders)
        {
            var key = new ListTreeRowKey(plan.To, parent.Key);
            holders.Add(key);
            var group = GroupNodeOf(parent, plan);
            var groupRow = new ListTreeRow(key, group, parent.Depth + 1, true, true, group?.IsExpanded ?? false, new[] { NewRowUnder(key, parent.Depth + 2, plan) });
            var lists = ListsOf(parent);
            var groupIndex = ListIndexOf(lists, plan.To);
            var rows = children.ToList();
            rows.Insert(children.Count(child => ListIndexOf(lists, child.Key.Model) < groupIndex), groupRow);
            return rows;
        }

        private static bool HasHiddenList(ListTreeRow row, IReadOnlyList<ListTreeRow> children, MovePlan plan) =>
            !children.Any(child => ReferenceEquals(child.Key.Model, plan.To)) && ListIndexOf(ListsOf(row), plan.To) >= 0;

        // The lists of the model of the row, in the order of their group rows.
        private static IReadOnlyList<PrefabManager> ListsOf(ListTreeRow row) =>
            row.Key.Model is IControl owner ? TreeGroup.GroupsOf(owner).Select(group => group.Manager).ToArray() : Array.Empty<PrefabManager>();

        private static int ListIndexOf(IReadOnlyList<PrefabManager> lists, object model)
        {
            for (var index = 0; index < lists.Count; index++)
                if (ReferenceEquals(lists[index], model))
                    return index;

            return -1;
        }

        // The second place of a shared card has no node, so its group row has none.
        private static TreeNode? GroupNodeOf(ListTreeRow parent, MovePlan plan) =>
            parent.Node?.Children.FirstOrDefault(child => ReferenceEquals(child.Group?.Manager, plan.To));

        // The row at the place of the slot keeps the source node. A second place of the same list has no node.
        private static ListTreeRow NewRowUnder(ListTreeRowKey parent, int depth, MovePlan plan) =>
            parent.Equals(plan.Holder)
                ? Closed(Moved(plan.Source, parent, depth))
                : Closed(Mirrored(plan.Source, parent, depth));

        private static ListTreeRow Closed(ListTreeRow row) => row with { IsExpanded = false };

        private static ListTreeRow Moved(ListTreeRow row, ListTreeRowKey parent, int depth)
        {
            var key = new ListTreeRowKey(row.Key.Model, parent);
            return row with { Key = key, Depth = depth, Children = row.Children.Select(child => Moved(child, key, depth + 1)).ToArray() };
        }

        private static ListTreeRow Mirrored(ListTreeRow row, ListTreeRowKey parent, int depth)
        {
            var key = new ListTreeRowKey(row.Key.Model, parent);
            return row with { Key = key, Node = null, Depth = depth, Children = row.Children.Select(child => Mirrored(child, key, depth + 1)).ToArray() };
        }

        // A group row shows a list, and option E leaves out a list with no row.
        private static bool IsShown(ListTreeRow row) => !row.IsGroup || row.Children.Count > 0;

        // A drop opens every row on the path to a list that gets the row.
        private static IReadOnlySet<ListTreeRowKey> OpenedBy(IEnumerable<ListTreeRowKey> holders)
        {
            var opened = new HashSet<ListTreeRowKey>();
            foreach (var holder in holders)
                for (var key = holder; key != null; key = key.Parent)
                    opened.Add(key);

            return opened;
        }

        private static IReadOnlyList<ListTreeRow> Opened(IEnumerable<ListTreeRow> rows, IReadOnlySet<ListTreeRowKey> opened) =>
            rows.Select(row => row with { IsExpanded = row.IsExpanded || opened.Contains(row.Key), Children = Opened(row.Children, opened) }).ToArray();

        private sealed record MovePlan(IControl Item, PrefabManager From, PrefabManager To, int Index, ListTreeRow Source, ListTreeRowKey Holder);
    }
}
