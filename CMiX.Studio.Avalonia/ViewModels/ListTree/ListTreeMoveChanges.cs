// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The live rows that one move changes, as ListTreeMove changes them. Every other row keeps its live count.
    internal sealed class ListTreeMoveChanges
    {
        private readonly ListTreeMovePlan plan;
        private readonly IReadOnlySet<ListTreeRowKey> opened;
        private readonly IReadOnlySet<ListTreeCountedRow> changed;
        private readonly IReadOnlyDictionary<ListTreeCountedRow, ListTreeCountedRow[]> changedChildren;

        private ListTreeMoveChanges(ListTreeMovePlan plan, IReadOnlySet<ListTreeCountedRow> holders, IReadOnlySet<ListTreeCountedRow> hiddenListOwners,
            IReadOnlySet<ListTreeRowKey> opened, IReadOnlySet<ListTreeCountedRow> changed, IReadOnlyDictionary<ListTreeCountedRow, ListTreeCountedRow[]> changedChildren)
        {
            this.plan = plan;
            Holders = holders;
            HiddenListOwners = hiddenListOwners;
            this.opened = opened;
            this.changed = changed;
            this.changedChildren = changedChildren;
        }

        // The kept rows that show the list of the slot. Each gets a row of the source.
        public IReadOnlySet<ListTreeCountedRow> Holders { get; }

        // The kept rows that own the list of the slot when option E hides it. Each gets its group row.
        public IReadOnlySet<ListTreeCountedRow> HiddenListOwners { get; }

        public bool IsChanged(ListTreeCountedRow row) => changed.Contains(row);

        // The drop opens every row on the path to a row that gets the source.
        public bool IsOpened(ListTreeRowKey key) => opened.Contains(key);

        // In tree order: the changed children and the rows of the source that leave.
        public IReadOnlyList<ListTreeCountedRow> ChangedChildrenOf(ListTreeCountedRow row) => changedChildren.GetValueOrDefault(row) ?? Array.Empty<ListTreeCountedRow>();

        public ListTreeRowKey GroupKeyOf(ListTreeCountedRow owner) => GroupKeyOf(owner, plan);

        public static ListTreeMoveChanges Of(ListTreeCountedTree tree, ListTreeMovePlan plan)
        {
            var holders = tree.RowsOf(plan.To).ToHashSet();
            var hiddenListOwners = tree.OwnersOf(plan.To).Where(owner => !holders.Contains(owner) && tree.RowOf(GroupKeyOf(owner, plan)) == null).ToHashSet();
            var opened = OpenedBy(HoldersToOpen(plan, holders, hiddenListOwners));
            var seeds = holders.Concat(hiddenListOwners).Concat(tree.LeavingRows.Select(tree.ParentOf)).Concat(opened.Select(tree.RowOf)).OfType<ListTreeCountedRow>();
            var changed = ChangedRowsOf(tree, seeds);
            return new ListTreeMoveChanges(plan, holders, hiddenListOwners, opened, changed, ChangedChildrenOf(tree, changed));
        }

        private static ListTreeRowKey GroupKeyOf(ListTreeCountedRow owner, ListTreeMovePlan plan) => new(plan.To, owner.Row.Key);

        // A move in its own list opens only the holder of the slot. A move to another list opens every row that shows that list.
        private static IEnumerable<ListTreeRowKey> HoldersToOpen(ListTreeMovePlan plan, IEnumerable<ListTreeCountedRow> holders, IEnumerable<ListTreeCountedRow> hiddenListOwners) =>
            plan.IsInItsOwnList
                ? new[] { plan.Holder }
                : holders.Select(holder => holder.Row.Key).Concat(hiddenListOwners.Select(owner => GroupKeyOf(owner, plan)));

        private static IReadOnlySet<ListTreeRowKey> OpenedBy(IEnumerable<ListTreeRowKey> holders)
        {
            var opened = new HashSet<ListTreeRowKey>();
            foreach (var holder in holders)
                for (ListTreeRowKey? key = holder; key != null; key = key.Parent)
                    opened.Add(key);

            return opened;
        }

        // A row changes when a row under it changes, so the root always changes.
        private static IReadOnlySet<ListTreeCountedRow> ChangedRowsOf(ListTreeCountedTree tree, IEnumerable<ListTreeCountedRow> seeds)
        {
            var changed = new HashSet<ListTreeCountedRow> { tree.Root };
            foreach (var seed in seeds)
            {
                var row = seed;
                while (row != null && changed.Add(row))
                    row = tree.ParentOf(row);
            }

            return changed;
        }

        private static IReadOnlyDictionary<ListTreeCountedRow, ListTreeCountedRow[]> ChangedChildrenOf(ListTreeCountedTree tree, IEnumerable<ListTreeCountedRow> changed) =>
            changed.Concat(tree.LeavingRows).Distinct()
                .Where(row => tree.ParentOf(row) != null)
                .GroupBy(row => tree.ParentOf(row)!)
                .ToDictionary(group => group.Key, group => group.OrderBy(row => row.Index).ToArray());
    }
}
