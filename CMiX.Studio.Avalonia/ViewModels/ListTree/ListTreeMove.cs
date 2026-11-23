// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The snapshot as if the source, or the block, were dropped on the slot. The live tree does not change.
    public static class ListTreeMove
    {
        public static ListTreeSnapshot Of(ListTreeSnapshot snapshot, ListTreeRow source, ListTreeSlot slot) => Of(snapshot, ListTreeBlock.Of(source), slot);

        public static ListTreeSnapshot Of(ListTreeSnapshot snapshot, ListTreeBlock block, ListTreeSlot slot)
        {
            var plan = ListTreeMovePlan.Of(block, slot);
            var holders = new List<ListTreeRowKey>();
            var moved = RowsOf(snapshot.Roots, plan, holders);
            return new ListTreeSnapshot(Opened(moved, OpenedBy(HoldersToOpen(plan, holders))));
        }

        // A move in its own list opens only the holder of the slot. A move to another list opens every holder of that list.
        private static IReadOnlyList<ListTreeRowKey> HoldersToOpen(ListTreeMovePlan plan, IReadOnlyList<ListTreeRowKey> holders) =>
            plan.IsInItsOwnList ? new[] { plan.Holder } : holders;

        private static IReadOnlyList<ListTreeRow> RowsOf(IEnumerable<ListTreeRow> rows, ListTreeMovePlan plan, List<ListTreeRowKey> holders) =>
            rows.Select(row => RowOf(row, plan, holders)).Where(IsShown).ToArray();

        private static ListTreeRow RowOf(ListTreeRow row, ListTreeMovePlan plan, List<ListTreeRowKey> holders)
        {
            var children = RowsOf(row.Children.Where(child => !IsSourceIn(row, child, plan)), plan, holders);
            if (ReferenceEquals(row.Key.Model, plan.To))
                return row with { Children = WithMovedRow(row, children, plan, holders) };

            if (HasHiddenList(row, children, plan))
                return row with { Children = WithGroupRow(row, children, plan, holders) };

            return row with { Children = children };
        }

        private static bool IsSourceIn(ListTreeRow row, ListTreeRow child, ListTreeMovePlan plan) => plan.IsLeaving(row.Key.Model, child.Key.Model);

        private static IReadOnlyList<ListTreeRow> WithMovedRow(ListTreeRow holder, IReadOnlyList<ListTreeRow> children, ListTreeMovePlan plan, List<ListTreeRowKey> holders)
        {
            holders.Add(holder.Key);
            var rows = children.ToList();
            rows.InsertRange(Math.Clamp(plan.Index, 0, rows.Count), plan.Members.Select(member => MovedRowIn(holder, member, plan)));
            return rows;
        }

        private static ListTreeRow MovedRowIn(ListTreeRow holder, ListTreeMoveMember member, ListTreeMovePlan plan)
        {
            if (holder.Children.FirstOrDefault(child => ReferenceEquals(child.Key.Model, member.Item)) is { } own)
                return ListTreeMovePlan.KeepsItsOwnRowOpen(member, own) ? own : Closed(own);

            return NewRowUnder(holder.Key, holder.Depth + 1, member, plan);
        }

        // The list has no row yet because option E hides an empty list.
        private static IReadOnlyList<ListTreeRow> WithGroupRow(ListTreeRow parent, IReadOnlyList<ListTreeRow> children, ListTreeMovePlan plan, List<ListTreeRowKey> holders)
        {
            var key = new ListTreeRowKey(plan.To, parent.Key);
            holders.Add(key);
            var group = plan.GroupNodeOf(parent);
            var groupRow = new ListTreeRow(key, group, parent.Depth + 1, IsCard: true, IsGroup: true, group?.IsExpanded ?? false,
                plan.Members.Select(member => NewRowUnder(key, parent.Depth + 2, member, plan)).ToArray());
            var lists = ListTreeRowLists.Of(parent);
            var groupIndex = ListTreeRowLists.IndexOf(lists, plan.To);
            var rows = children.ToList();
            rows.Insert(children.Count(child => ListTreeRowLists.IndexOf(lists, child.Key.Model) < groupIndex), groupRow);
            return rows;
        }

        private static bool HasHiddenList(ListTreeRow row, IReadOnlyList<ListTreeRow> children, ListTreeMovePlan plan) =>
            !children.Any(child => ReferenceEquals(child.Key.Model, plan.To)) && ListTreeRowLists.IndexOf(ListTreeRowLists.Of(row), plan.To) >= 0;

        // The row in the holder of the slot keeps the source node. A second holder of the same list has no node.
        private static ListTreeRow NewRowUnder(ListTreeRowKey parent, int depth, ListTreeMoveMember member, ListTreeMovePlan plan) =>
            parent.Equals(plan.Holder)
                ? Closed(Moved(member.Row, parent, depth))
                : Closed(Mirrored(member.Row, parent, depth));

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
    }
}
