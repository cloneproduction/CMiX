// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The rows of one move, counted on the live rows as ListTreeMove and ListTreeLayout give them. Only the rows that the move changes are counted again.
    // The place is the place of the pressed row of the block.
    internal sealed class ListTreeMoveCount
    {
        private readonly ListTreeCountedTree tree;
        private readonly ListTreeMovePlan plan;
        private readonly ListTreeMoveChanges changes;
        private readonly Dictionary<ListTreeCountedRow, int> counts = new();
        private readonly Dictionary<ListTreeCountedRow, int> keptCounts = new();

        private ListTreeMoveCount(ListTreeCountedTree tree, ListTreeMovePlan plan, ListTreeMoveChanges changes)
        {
            this.tree = tree;
            this.plan = plan;
            this.changes = changes;
        }

        public static ListTreeMoveCount Of(ListTreeCountedTree tree, ListTreeMovePlan plan) => new(tree, plan, ListTreeMoveChanges.Of(tree, plan));

        // Null when the move gives the pressed row no visible row.
        public ListTreePlace? Place()
        {
            if (tree.CountedPressed is not { } pressed || tree.ParentOf(pressed) is not { } list || !IsLeaving(list, pressed))
                return null;

            return plan.IsPressedInItsOwnList ? PlaceInItsOwnList(list) : PlaceInHolder() ?? PlaceInHiddenList();
        }

        // Every row that shows the list keeps its own row. The row of the pressed row shows closed.
        private ListTreePlace? PlaceInItsOwnList(ListTreeCountedRow list) =>
            IndexAt(list, Math.Clamp(plan.Index, 0, KeptCountOf(list))) is { } index ? PlaceAt(Pressed.Key, index + CountBeforePressedIn(list), Pressed.Depth) : null;

        // A list that already has the item gives the pressed row no row.
        private ListTreePlace? PlaceInHolder()
        {
            if (tree.RowOf(plan.Holder) is not { } holder || !changes.Holders.Contains(holder) || tree.RowOfTheItemIn(holder, plan.Pressed.Item) != null)
                return null;

            return IndexAt(holder, InsertOf(holder)!.Index) is { } index
                ? PlaceAt(new ListTreeRowKey(Pressed.Key.Model, holder.Row.Key), index + CountBeforePressedIn(holder), holder.Row.Depth + 1)
                : null;
        }

        private ListTreeRow Pressed => plan.Pressed.Row;

        // The rows that the members before the pressed member show in the holder.
        private int CountBeforePressedIn(ListTreeCountedRow holder) => plan.Members.Take(plan.PressedIndex).Sum(member => InsertCountOf(holder, member));

        // Every other holder gets a closed row.
        private int InsertCountOf(ListTreeCountedRow holder, ListTreeMoveMember member) =>
            tree.RowOfTheItemIn(holder, member.Item) is { } own && ListTreeMovePlan.KeepsItsOwnRowOpen(member, own.Row) ? own.Count : 1;

        // Option E hides an empty list. The drop shows its group row with the source as its only row.
        private ListTreePlace? PlaceInHiddenList()
        {
            var owner = changes.HiddenListOwners.FirstOrDefault(candidate => changes.GroupKeyOf(candidate).Equals(plan.Holder));
            if (owner == null || !IsGroupOpen(owner) || IndexAt(owner, GroupIndexIn(owner)) is not { } groupIndex)
                return null;

            return PlaceAt(new ListTreeRowKey(Pressed.Key.Model, changes.GroupKeyOf(owner)), groupIndex + 1 + plan.PressedIndex, owner.Row.Depth + 2);
        }

        private ListTreePlace PlaceAt(ListTreeRowKey key, int index, int depth)
        {
            var sizes = tree.Sizes;
            return new ListTreePlace(key, index * sizes.Pitch, sizes.RowHeight, sizes.Pitch - sizes.Gap, depth, depth * sizes.Indent, Pressed.Children.Count > 0);
        }

        // The visible index of a kept row of the live tree, -1 for the root. Null when a closed row hides it.
        private int? IndexOf(ListTreeCountedRow row)
        {
            if (tree.ParentOf(row) is not { } parent)
                return -1;

            var keptIndex = KeptIndexOf(parent, row.Index);
            return IndexAt(parent, keptIndex) is { } index ? index + InsertedBefore(parent, keptIndex) : null;
        }

        // The visible index of a row at the kept index in the children of the parent, before any inserted row.
        private int? IndexAt(ListTreeCountedRow parent, int keptIndex) =>
            IsOpen(parent) && IndexOf(parent) is { } parentIndex ? parentIndex + 1 + CountBefore(parent, LiveIndexOf(parent, keptIndex)) : null;

        private int InsertedBefore(ListTreeCountedRow parent, int keptIndex) =>
            InsertOf(parent) is { } insert && insert.Index <= keptIndex ? insert.Count : 0;

        // The rows that the kept children before the live index show.
        private int CountBefore(ListTreeCountedRow parent, int liveIndex)
        {
            var count = parent.CountBefore(liveIndex);
            foreach (var child in changes.ChangedChildrenOf(parent))
                if (child.Index < liveIndex)
                    count += ShownCountOf(parent, child) - child.Count;

            return count;
        }

        private int ShownCountOf(ListTreeCountedRow parent, ListTreeCountedRow child) => IsKeptIn(parent, child) ? CountOf(child) : 0;

        private int CountOf(ListTreeCountedRow row)
        {
            if (!changes.IsChanged(row))
                return row.Count;

            if (!counts.TryGetValue(row, out var count))
                counts.Add(row, count = IsOpen(row) ? 1 + CountBefore(row, row.Children.Count) + (InsertOf(row)?.Count ?? 0) : 1);

            return count;
        }

        private int KeptIndexOf(ListTreeCountedRow parent, int liveIndex)
        {
            var keptIndex = liveIndex;
            foreach (var child in changes.ChangedChildrenOf(parent))
                if (child.Index < liveIndex && !IsKeptIn(parent, child))
                    keptIndex--;

            return keptIndex;
        }

        private int LiveIndexOf(ListTreeCountedRow parent, int keptIndex)
        {
            var liveIndex = keptIndex;
            foreach (var child in changes.ChangedChildrenOf(parent))
                if (child.Index < liveIndex && !IsKeptIn(parent, child))
                    liveIndex++;

            return liveIndex;
        }

        private int KeptCountOf(ListTreeCountedRow row)
        {
            if (!keptCounts.TryGetValue(row, out var count))
                keptCounts.Add(row, count = KeptIndexOf(row, row.Children.Count));

            return count;
        }

        // A group row shows a list, and option E leaves out a list with no row.
        private bool IsKeptIn(ListTreeCountedRow parent, ListTreeCountedRow child)
        {
            if (IsLeaving(parent, child))
                return false;

            return !changes.IsChanged(child) || !child.Row.IsGroup || KeptCountOf(child) + (InsertOf(child) != null ? 1 : 0) > 0;
        }

        private bool IsLeaving(ListTreeCountedRow parent, ListTreeCountedRow child) => plan.IsLeaving(parent.Model, child.Model);

        private bool IsOpen(ListTreeCountedRow row) => row.Row.IsExpanded || changes.IsOpened(row);

        // The rows that the move inserts in a row: the moved rows in a row of the list, or the group row of a hidden list.
        private Insert? InsertOf(ListTreeCountedRow row)
        {
            if (changes.Holders.Contains(row))
                return new Insert(Math.Clamp(plan.Index, 0, KeptCountOf(row)), plan.Members.Sum(member => InsertCountOf(row, member)));

            return changes.HiddenListOwners.Contains(row) ? new Insert(GroupIndexIn(row), IsGroupOpen(row) ? 1 + plan.Members.Count : 1) : null;
        }

        // The group row goes after the kept rows of the lists before it.
        private int GroupIndexIn(ListTreeCountedRow owner)
        {
            var lists = tree.ListsOf(owner);
            var groupIndex = ListTreeRowLists.IndexOf(lists, plan.To);
            return owner.Children.Count(child => IsKeptIn(owner, child) && ListTreeRowLists.IndexOf(lists, child.Model) < groupIndex);
        }

        private bool IsGroupOpen(ListTreeCountedRow owner) =>
            (plan.GroupNodeOf(owner.Row)?.IsExpanded ?? false) || changes.IsOpened(changes.GroupKeyOf(owner));

        private sealed record Insert(int Index, int Count);
    }
}
