// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The tree during a drag: the rows and the layout of the current candidate, the light of each row and the drop target.
    public sealed class ListTreeDragLook
    {
        private readonly IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> liveRows;
        private readonly ListTreeLight light;
        private readonly ListTreeBlock block;
        private ListTreeDropHolder? holder;

        private ListTreeDragLook(ListTreeCandidate candidate, IReadOnlyList<ListTreeRow> rows, IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> liveRows,
            ListTreeLight light, ListTreeBlock block)
        {
            Candidate = candidate;
            Rows = rows;
            this.liveRows = liveRows;
            this.light = light;
            this.block = block;
        }

        public ListTreeCandidate Candidate { get; }

        // The visible rows of the candidate. A row that the live tree does not have shows the node of its model.
        public IReadOnlyList<ListTreeRow> Rows { get; }

        public ListTreeLayout Layout => Candidate.Layout;

        public ListTreeRowKey Dragged => Candidate.Place.Key;

        public ListTreeDropHolder Holder => holder ??= ListTreeDropHolder.Of(liveRows, block.Pressed, Candidate);

        // Null for a reorder in the same parent: the moving row shows the place.
        public TreeNode? DropTarget => Holder.IsSameParent ? null : HolderNode;

        public TreeNode? HolderNode => Holder.Key is { } key ? liveRows.GetValueOrDefault(key)?.Node : null;

        public static ListTreeDragLook Of(ListTreeSnapshot live, ListTreeRow source, ListTreeCandidate candidate, ListTreeLight light) =>
            Of(live, ListTreeBlock.Of(source), candidate, light);

        public static ListTreeDragLook Of(ListTreeSnapshot live, ListTreeBlock block, ListTreeCandidate candidate, ListTreeLight light)
        {
            var liveRows = live.AllRows().ToDictionary(row => row.Key);
            var liveNodes = LiveNodesOf(liveRows.Values);
            var rows = candidate.Snapshot.VisibleRows().Select(row => row.Node != null ? row : row with { Node = liveNodes.GetValueOrDefault(row.Key.Model) }).ToArray();
            return new ListTreeDragLook(candidate, rows, liveRows, light, block);
        }

        // A row that the live tree does not have is new in the candidate: the moved row or its list. It stays bright.
        public ListTreeRowLight LightOf(ListTreeRowKey key)
        {
            if (key.Equals(Dragged) || IsOtherRowOfTheBlock(key))
                return ListTreeRowLight.Dragged;

            return liveRows.ContainsKey(key) && !light.IsBright(key) ? ListTreeRowLight.Dimmed : ListTreeRowLight.Rest;
        }

        // The other rows of the block land next to the pressed row, under the same parent.
        private bool IsOtherRowOfTheBlock(ListTreeRowKey key) =>
            block.Rows.Count > 1 && Equals(key.Parent, Dragged.Parent) && block.Rows.Any(row => ReferenceEquals(row.Key.Model, key.Model));

        private static IReadOnlyDictionary<object, TreeNode> LiveNodesOf(IEnumerable<ListTreeRow> rows)
        {
            var nodes = new Dictionary<object, TreeNode>(ReferenceEqualityComparer.Instance);
            foreach (var row in rows)
                if (row.Node is { } node)
                    nodes.TryAdd(row.Key.Model, node);

            return nodes;
        }
    }
}
