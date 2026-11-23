// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The live rows of one drag, counted once. A slot then gets the place of the pressed row of the block without a moved tree.
    public sealed class ListTreeCountedTree
    {
        private readonly Builder built;

        private ListTreeCountedTree(ListTreeCountedRow root, ListTreeBlock block, ListTreeCountedRow? countedSource, ListTreeSizes sizes, Builder built)
        {
            Root = root;
            Block = block;
            CountedSource = countedSource;
            Sizes = sizes;
            this.built = built;
        }

        // The root is not a row: its children are the roots of the snapshot.
        public ListTreeCountedRow Root { get; }

        public ListTreeBlock Block { get; }

        // The counted row of the pressed row of the block.
        public ListTreeCountedRow? CountedSource { get; }

        public ListTreeSizes Sizes { get; }

        // Null when the move gives the source no visible row, as ListTreeMove and ListTreeLayout.
        public ListTreePlace? PlaceOf(ListTreeSlot slot) => ListTreeMoveCount.Of(this, ListTreeMovePlan.Of(Block, slot)).Place();

        public ListTreeCountedRow? ParentOf(ListTreeCountedRow row) => built.Parents.GetValueOrDefault(row);

        public ListTreeCountedRow? RowOf(ListTreeRowKey? key) => key != null ? built.RowsByKey.GetValueOrDefault(key) : null;

        // Only the rows that the move keeps: the row of the source in the list that it leaves goes out, with its rows.
        public IEnumerable<ListTreeCountedRow> RowsOf(object model) => built.RowsByModel.GetValueOrDefault(model) ?? Enumerable.Empty<ListTreeCountedRow>();

        public IEnumerable<ListTreeCountedRow> OwnersOf(PrefabManager list) => built.OwnersByList.GetValueOrDefault(list) ?? Enumerable.Empty<ListTreeCountedRow>();

        // The first row of the item of a row of the block in the row, also when it leaves.
        public ListTreeCountedRow? RowOfTheItemIn(ListTreeCountedRow row, object item) =>
            built.ItemRows.GetValueOrDefault(row)?.FirstOrDefault(child => ReferenceEquals(child.Model, item));

        public IReadOnlyList<PrefabManager> ListsOf(ListTreeCountedRow owner) => built.ListsByOwner.GetValueOrDefault(owner) ?? Array.Empty<PrefabManager>();

        // The rows of the block in the lists that they leave, one per row that shows a list.
        public IReadOnlyList<ListTreeCountedRow> LeavingRows => built.LeavingRows;

        public static ListTreeCountedTree Of(ListTreeSnapshot snapshot, ListTreeRow source, ListTreeSizes sizes) => Of(snapshot, ListTreeBlock.Of(source), sizes);

        public static ListTreeCountedTree Of(ListTreeSnapshot snapshot, ListTreeBlock block, ListTreeSizes sizes)
        {
            var root = ListTreeCountedRow.Of(RootRowOf(snapshot), 0);
            var built = new Builder(ListsByItemOf(block));
            built.AddKeptChildren(root);
            return new ListTreeCountedTree(root, block, built.RowsByKey.GetValueOrDefault(block.Pressed.Key), sizes, built);
        }

        // A row with no node leaves no list.
        private static IReadOnlyDictionary<object, object> ListsByItemOf(ListTreeBlock block)
        {
            var lists = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);
            foreach (var row in block.Rows)
                if (row.Node is { Item: { } item, Owner: { } list })
                    lists.TryAdd(item, list);

            return lists;
        }

        private static ListTreeRow RootRowOf(ListTreeSnapshot snapshot) =>
            new(new ListTreeRowKey(snapshot, null), null, -1, IsCard: false, IsGroup: false, IsExpanded: true, snapshot.Roots);

        private sealed class Builder
        {
            private readonly IReadOnlyDictionary<object, object> listsByItem;

            public Builder(IReadOnlyDictionary<object, object> listsByItem)
            {
                this.listsByItem = listsByItem;
            }

            public Dictionary<ListTreeCountedRow, ListTreeCountedRow> Parents { get; } = new();

            public Dictionary<ListTreeRowKey, ListTreeCountedRow> RowsByKey { get; } = new();

            public Dictionary<object, List<ListTreeCountedRow>> RowsByModel { get; } = new(ReferenceEqualityComparer.Instance);

            public Dictionary<object, List<ListTreeCountedRow>> OwnersByList { get; } = new(ReferenceEqualityComparer.Instance);

            public Dictionary<ListTreeCountedRow, IReadOnlyList<PrefabManager>> ListsByOwner { get; } = new();

            public List<ListTreeCountedRow> LeavingRows { get; } = new();

            public Dictionary<ListTreeCountedRow, List<ListTreeCountedRow>> ItemRows { get; } = new();

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
                if (listsByItem.ContainsKey(child.Model))
                    AddTo(ItemRows, row, child);
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
                listsByItem.TryGetValue(child.Model, out var list) && ReferenceEquals(row.Model, list);

            private static void AddTo<TKey>(Dictionary<TKey, List<ListTreeCountedRow>> rows, TKey key, ListTreeCountedRow row) where TKey : notnull
            {
                if (!rows.TryGetValue(key, out var list))
                    rows.Add(key, list = new List<ListTreeCountedRow>());

                list.Add(row);
            }
        }
    }
}
