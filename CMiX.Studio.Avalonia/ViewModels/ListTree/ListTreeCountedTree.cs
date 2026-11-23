// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The live rows of one drag, counted once. A slot then gets the place of the moved row without a moved tree.
    public sealed class ListTreeCountedTree
    {
        private readonly Builder built;

        private ListTreeCountedTree(ListTreeCountedRow root, ListTreeRow source, ListTreeCountedRow? countedSource, ListTreeSizes sizes, Builder built)
        {
            Root = root;
            Source = source;
            CountedSource = countedSource;
            Sizes = sizes;
            this.built = built;
        }

        // The root is not a row: its children are the roots of the snapshot.
        public ListTreeCountedRow Root { get; }

        public ListTreeRow Source { get; }

        public ListTreeCountedRow? CountedSource { get; }

        public ListTreeSizes Sizes { get; }

        // Null when the move gives the source no visible row, as ListTreeMove and ListTreeLayout.
        public ListTreePlace? PlaceOf(ListTreeSlot slot) => ListTreeMoveCount.Of(this, ListTreeMovePlan.Of(Source, slot)).Place();

        public ListTreeCountedRow? ParentOf(ListTreeCountedRow row) => built.Parents.GetValueOrDefault(row);

        public ListTreeCountedRow? RowOf(ListTreeRowKey? key) => key != null ? built.RowsByKey.GetValueOrDefault(key) : null;

        // Only the rows that the move keeps: the row of the source in the list that it leaves goes out, with its rows.
        public IEnumerable<ListTreeCountedRow> RowsOf(object model) => built.RowsByModel.GetValueOrDefault(model) ?? Enumerable.Empty<ListTreeCountedRow>();

        public IEnumerable<ListTreeCountedRow> OwnersOf(PrefabManager list) => built.OwnersByList.GetValueOrDefault(list) ?? Enumerable.Empty<ListTreeCountedRow>();

        // The first row of the item of the source in the row, also when it leaves.
        public ListTreeCountedRow? RowOfTheItemIn(ListTreeCountedRow row) => built.ItemRows.GetValueOrDefault(row);

        public IReadOnlyList<PrefabManager> ListsOf(ListTreeCountedRow owner) => built.ListsByOwner.GetValueOrDefault(owner) ?? Array.Empty<PrefabManager>();

        // The rows of the source in the list that it leaves, one per row that shows the list.
        public IReadOnlyList<ListTreeCountedRow> LeavingRows => built.LeavingRows;

        public static ListTreeCountedTree Of(ListTreeSnapshot snapshot, ListTreeRow source, ListTreeSizes sizes)
        {
            var root = ListTreeCountedRow.Of(RootRowOf(snapshot), 0);
            var built = new Builder(source.Node?.Owner, source.Node?.Item);
            built.AddKeptChildren(root);
            return new ListTreeCountedTree(root, source, built.RowsByKey.GetValueOrDefault(source.Key), sizes, built);
        }

        private static ListTreeRow RootRowOf(ListTreeSnapshot snapshot) =>
            new(new ListTreeRowKey(snapshot, null), null, -1, IsCard: false, IsGroup: false, IsExpanded: true, snapshot.Roots);

        private sealed class Builder
        {
            private readonly object? from;
            private readonly object? item;

            public Builder(object? from, object? item)
            {
                this.from = from;
                this.item = item;
            }

            public Dictionary<ListTreeCountedRow, ListTreeCountedRow> Parents { get; } = new();

            public Dictionary<ListTreeRowKey, ListTreeCountedRow> RowsByKey { get; } = new();

            public Dictionary<object, List<ListTreeCountedRow>> RowsByModel { get; } = new(ReferenceEqualityComparer.Instance);

            public Dictionary<object, List<ListTreeCountedRow>> OwnersByList { get; } = new(ReferenceEqualityComparer.Instance);

            public Dictionary<ListTreeCountedRow, IReadOnlyList<PrefabManager>> ListsByOwner { get; } = new();

            public List<ListTreeCountedRow> LeavingRows { get; } = new();

            public Dictionary<ListTreeCountedRow, ListTreeCountedRow> ItemRows { get; } = new();

            public void AddKeptChildren(ListTreeCountedRow row)
            {
                foreach (var child in row.Children)
                {
                    AddChild(row, child);
                    if (IsSourceIn(row, child))
                    {
                        LeavingRows.Add(child);
                        AddLeavingChildren(child);
                        continue;
                    }

                    AddKept(child);
                    AddKeptChildren(child);
                }
            }

            private void AddLeavingChildren(ListTreeCountedRow row)
            {
                foreach (var child in row.Children)
                {
                    AddChild(row, child);
                    AddLeavingChildren(child);
                }
            }

            private void AddChild(ListTreeCountedRow row, ListTreeCountedRow child)
            {
                Parents.Add(child, row);
                RowsByKey.Add(child.Row.Key, child);
                if (item != null && ReferenceEquals(child.Model, item))
                    ItemRows.TryAdd(row, child);
            }

            private void AddKept(ListTreeCountedRow row)
            {
                AddTo(RowsByModel, row.Model, row);
                var lists = ListTreeRowLists.Of(row.Row);
                if (lists.Count == 0)
                    return;

                ListsByOwner.Add(row, lists);
                foreach (var list in lists)
                    AddTo(OwnersByList, list, row);
            }

            private bool IsSourceIn(ListTreeCountedRow row, ListTreeCountedRow child) =>
                from != null && ReferenceEquals(row.Model, from) && ReferenceEquals(child.Model, item);

            private static void AddTo(Dictionary<object, List<ListTreeCountedRow>> rows, object key, ListTreeCountedRow row)
            {
                if (!rows.TryGetValue(key, out var list))
                    rows.Add(key, list = new List<ListTreeCountedRow>());

                list.Add(row);
            }
        }
    }
}
