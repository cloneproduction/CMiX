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
        private readonly Opened opened;
        private readonly Changed changed;

        private ListTreeMoveChanges(ListTreeMovePlan plan, IReadOnlySet<ListTreeCountedRow> holders, IReadOnlySet<ListTreeCountedRow> hiddenListOwners,
            Opened opened, Changed changed)
        {
            this.plan = plan;
            Holders = holders;
            HiddenListOwners = hiddenListOwners;
            this.opened = opened;
            this.changed = changed;
        }

        // The kept rows that show the list of the slot. Each gets a row of the source.
        public IReadOnlySet<ListTreeCountedRow> Holders { get; }

        // The kept rows that own the list of the slot when option E hides it. Each gets its group row.
        public IReadOnlySet<ListTreeCountedRow> HiddenListOwners { get; }

        public bool IsChanged(ListTreeCountedRow row) => changed.Rows.Contains(row);

        // The drop opens every row on the path to a row that gets the source.
        public bool IsOpened(ListTreeCountedRow row) => opened.Rows.Contains(row);

        // A key with no live row is the group row of a hidden list.
        public bool IsOpened(ListTreeRowKey key) => opened.Keys.Contains(key);

        // In tree order: the changed children and the rows of the source that leave.
        public IReadOnlyList<ListTreeCountedRow> ChangedChildrenOf(ListTreeCountedRow row) =>
            changed.Children.TryGetValue(row, out var children) ? children : Array.Empty<ListTreeCountedRow>();

        public ListTreeRowKey GroupKeyOf(ListTreeCountedRow owner) => GroupKeyOf(owner, plan);

        public static ListTreeMoveChanges Of(ListTreeCountedTree tree, ListTreeMovePlan plan)
        {
            var holders = new HashSet<ListTreeCountedRow>(tree.RowsOf(plan.To));
            var hiddenListOwners = new HashSet<ListTreeCountedRow>(tree.OwnersOf(plan.To).Where(owner => IsHiddenListOwner(owner, plan, holders)));
            var openedRows = new HashSet<ListTreeCountedRow>();
            var openedKeys = new HashSet<ListTreeRowKey>();
            if (plan.IsInItsOwnList)
                OpenKey(tree, plan.Holder, openedRows, openedKeys);
            else
                OpenHolders(tree, plan, holders, hiddenListOwners, openedRows, openedKeys);

            var changed = new HashSet<ListTreeCountedRow> { tree.Root };
            var changedChildren = new Dictionary<ListTreeCountedRow, List<ListTreeCountedRow>>();
            foreach (var row in openedRows.Concat(holders).Concat(hiddenListOwners))
                AddChanged(tree, row, changed, changedChildren);

            AddLeavingRows(tree, changed, changedChildren);
            foreach (var children in changedChildren.Values)
                children.Sort((first, second) => first.Index.CompareTo(second.Index));

            return new ListTreeMoveChanges(plan, holders, hiddenListOwners, new Opened(openedRows, openedKeys), new Changed(changed, changedChildren));
        }

        private static bool IsHiddenListOwner(ListTreeCountedRow owner, ListTreeMovePlan plan, IReadOnlySet<ListTreeCountedRow> holders) =>
            !holders.Contains(owner) && !owner.Children.Any(child => ReferenceEquals(child.Model, plan.To));

        private static ListTreeRowKey GroupKeyOf(ListTreeCountedRow owner, ListTreeMovePlan plan) => new(plan.To, owner.Row.Key);

        // A move to another list opens every row that shows that list. A move in its own list opens only the holder of the slot.
        private static void OpenHolders(ListTreeCountedTree tree, ListTreeMovePlan plan, IEnumerable<ListTreeCountedRow> holders, IEnumerable<ListTreeCountedRow> hiddenListOwners,
            HashSet<ListTreeCountedRow> openedRows, HashSet<ListTreeRowKey> openedKeys)
        {
            foreach (var holder in holders)
                AddWithParents(tree, holder, openedRows);

            foreach (var owner in hiddenListOwners)
            {
                openedKeys.Add(GroupKeyOf(owner, plan));
                AddWithParents(tree, owner, openedRows);
            }
        }

        private static void OpenKey(ListTreeCountedTree tree, ListTreeRowKey? key, HashSet<ListTreeCountedRow> openedRows, HashSet<ListTreeRowKey> openedKeys)
        {
            for (; key != null; key = key.Parent)
            {
                if (tree.RowOf(key) is { } row)
                {
                    AddWithParents(tree, row, openedRows);
                    return;
                }

                openedKeys.Add(key);
            }
        }

        private static void AddWithParents(ListTreeCountedTree tree, ListTreeCountedRow row, HashSet<ListTreeCountedRow> rows)
        {
            ListTreeCountedRow? next = row;
            while (next != null && rows.Add(next))
                next = tree.ParentOf(next);
        }

        // A row changes when a row under it changes, so the root always changes. An opened row changes.
        private static void AddChanged(ListTreeCountedTree tree, ListTreeCountedRow row, HashSet<ListTreeCountedRow> changed,
            Dictionary<ListTreeCountedRow, List<ListTreeCountedRow>> changedChildren)
        {
            ListTreeCountedRow? next = row;
            while (next != null && changed.Add(next))
            {
                var parent = tree.ParentOf(next);
                if (parent != null)
                    AddTo(changedChildren, parent, next);

                next = parent;
            }
        }

        // A row of the source that leaves its list counts as a changed child of that list.
        private static void AddLeavingRows(ListTreeCountedTree tree, HashSet<ListTreeCountedRow> changed, Dictionary<ListTreeCountedRow, List<ListTreeCountedRow>> changedChildren)
        {
            foreach (var row in tree.LeavingRows)
            {
                if (changed.Contains(row) || tree.ParentOf(row) is not { } list)
                    continue;

                AddTo(changedChildren, list, row);
                AddChanged(tree, list, changed, changedChildren);
            }
        }

        private static void AddTo(Dictionary<ListTreeCountedRow, List<ListTreeCountedRow>> children, ListTreeCountedRow parent, ListTreeCountedRow row)
        {
            if (!children.TryGetValue(parent, out var list))
                children.Add(parent, list = new List<ListTreeCountedRow>());

            list.Add(row);
        }

        // The rows that the drop opens, and the keys of the group rows of hidden lists that it opens.
        private sealed record Opened(IReadOnlySet<ListTreeCountedRow> Rows, IReadOnlySet<ListTreeRowKey> Keys);

        // The rows that the move changes, and for each the changed children.
        private sealed record Changed(IReadOnlySet<ListTreeCountedRow> Rows, IReadOnlyDictionary<ListTreeCountedRow, List<ListTreeCountedRow>> Children);
    }
}
