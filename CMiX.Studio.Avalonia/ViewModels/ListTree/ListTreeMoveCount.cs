// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The rows of one move, counted on the live rows as ListTreeMove and ListTreeLayout give them. Only the rows that the move changes are counted again.
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

        // Null when the move gives the source no visible row.
        public ListTreePlace? Place()
        {
            if (tree.RowOf(plan.Source.Key) is not { } source || tree.ParentOf(source) is not { } list || !IsLeaving(list, source))
                return null;

            return plan.IsInItsOwnList ? PlaceInItsOwnList(list) : PlaceInHolder() ?? PlaceInHiddenList();
        }

        // Every row that shows the list keeps its own row. The row of the source shows closed.
        private ListTreePlace? PlaceInItsOwnList(ListTreeCountedRow list) =>
            IndexAt(list, Math.Clamp(plan.Index, 0, KeptCountOf(list))) is { } index ? PlaceAt(plan.Source.Key, index, plan.Source.Depth) : null;

        // A list that already has the item gives the source no row.
        private ListTreePlace? PlaceInHolder()
        {
            if (tree.RowOf(plan.Holder) is not { } holder || !changes.Holders.Contains(holder) || OwnRowIn(holder) != null)
                return null;

            return IndexAt(holder, InsertOf(holder)!.Index) is { } index ? PlaceAt(new ListTreeRowKey(plan.Source.Key.Model, holder.Row.Key), index, holder.Row.Depth + 1) : null;
        }

        // Option E hides an empty list. The drop shows its group row with the source as its only row.
        private ListTreePlace? PlaceInHiddenList()
        {
            var owner = changes.HiddenListOwners.FirstOrDefault(candidate => changes.GroupKeyOf(candidate).Equals(plan.Holder));
            if (owner == null || !IsGroupOpen(owner) || IndexAt(owner, GroupIndexIn(owner)) is not { } groupIndex)
                return null;

            return PlaceAt(new ListTreeRowKey(plan.Source.Key.Model, changes.GroupKeyOf(owner)), groupIndex + 1, owner.Row.Depth + 2);
        }

        private ListTreePlace PlaceAt(ListTreeRowKey key, int index, int depth)
        {
            var sizes = tree.Sizes;
            return new ListTreePlace(key, index * sizes.Pitch, sizes.RowHeight, 1 * sizes.Pitch - sizes.Gap, depth, depth * sizes.Indent, plan.Source.Children.Count > 0);
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
        private int CountBefore(ListTreeCountedRow parent, int liveIndex) =>
            parent.CountBefore(liveIndex) + changes.ChangedChildrenOf(parent).Where(child => child.Index < liveIndex).Sum(child => ShownCountOf(parent, child) - child.Count);

        private int ShownCountOf(ListTreeCountedRow parent, ListTreeCountedRow child) => IsKeptIn(parent, child) ? CountOf(child) : 0;

        private int CountOf(ListTreeCountedRow row)
        {
            if (!changes.IsChanged(row))
                return row.Count;

            if (!counts.TryGetValue(row, out var count))
                counts.Add(row, count = IsOpen(row) ? 1 + CountBefore(row, row.Children.Count) + (InsertOf(row)?.Count ?? 0) : 1);

            return count;
        }

        private int KeptIndexOf(ListTreeCountedRow parent, int liveIndex) =>
            liveIndex - changes.ChangedChildrenOf(parent).Count(child => child.Index < liveIndex && !IsKeptIn(parent, child));

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

        private bool IsLeaving(ListTreeCountedRow parent, ListTreeCountedRow child) =>
            ReferenceEquals(parent.Model, plan.From) && ReferenceEquals(child.Model, plan.Item);

        private bool IsOpen(ListTreeCountedRow row) => row.Row.IsExpanded || changes.IsOpened(row.Row.Key);

        // The row that the move inserts in a row: the moved row in a row of the list, or the group row of a hidden list.
        private Insert? InsertOf(ListTreeCountedRow row)
        {
            if (changes.Holders.Contains(row))
                return new Insert(Math.Clamp(plan.Index, 0, KeptCountOf(row)), OwnRowIn(row) is { } own && !own.Row.Key.Equals(plan.Source.Key) ? own.Count : 1);

            return changes.HiddenListOwners.Contains(row) ? new Insert(GroupIndexIn(row), IsGroupOpen(row) ? 2 : 1) : null;
        }

        private ListTreeCountedRow? OwnRowIn(ListTreeCountedRow holder) => tree.RowOf(new ListTreeRowKey(plan.Item, holder.Row.Key));

        // The group row goes after the kept rows of the lists before it.
        private int GroupIndexIn(ListTreeCountedRow owner)
        {
            var lists = tree.ListsOf(owner);
            var groupIndex = ListTreeRowLists.IndexOf(lists, plan.To);
            return owner.Children.Count(child => IsKeptIn(owner, child) && ListTreeRowLists.IndexOf(lists, child.Model) < groupIndex);
        }

        private bool IsGroupOpen(ListTreeCountedRow owner) =>
            (GroupNodeOf(owner)?.IsExpanded ?? false) || changes.IsOpened(changes.GroupKeyOf(owner));

        private TreeNode? GroupNodeOf(ListTreeCountedRow owner) =>
            owner.Row.Node?.Children.FirstOrDefault(child => ReferenceEquals(child.Group?.Manager, plan.To));

        private sealed record Insert(int Index, int Count);
    }
}
