// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The nodes that one drag moves, in tree order, with the pressed node among them.
    public sealed record ListTreeDragNodes(IReadOnlyList<TreeNode> Nodes, TreeNode Pressed)
    {
        public bool IsBlock => Nodes.Count > 1;

        public static ListTreeDragNodes Of(TreeNode pressed) => new(new[] { pressed }, pressed);

        // A press in a selection takes the top selected rows, or only the pressed row when they are not one kind of item.
        public static ListTreeDragNodes Of(ListTreeSnapshot snapshot, TreeNode pressed, IReadOnlyCollection<TreeNode> selected)
        {
            if (selected.Count < 2 || !selected.Contains(pressed))
                return Of(pressed);

            var outer = selected.Where(node => !selected.Any(other => !ReferenceEquals(other, node) && node.IsOrIsUnder(other))).ToHashSet();
            var nodes = snapshot.AllRows().Select(row => row.Node).OfType<TreeNode>().Where(outer.Contains).ToArray();
            return nodes.Length == outer.Count && outer.Contains(pressed) && IsOneKind(nodes) ? new ListTreeDragNodes(nodes, pressed) : Of(pressed);
        }

        // The rows of the nodes in the snapshot. Null when a node has no row.
        public ListTreeBlock? BlockIn(ListTreeSnapshot snapshot)
        {
            var rows = snapshot.AllRows().Where(row => row.Node != null).ToDictionary(row => row.Node!);
            var blockRows = Nodes.Select(node => rows.GetValueOrDefault(node)).OfType<ListTreeRow>().ToArray();
            return blockRows.Length == Nodes.Count && rows.TryGetValue(Pressed, out var pressed) ? new ListTreeBlock(blockRows, pressed) : null;
        }

        // The kind of an item is the kind of its group, as in TreeDropService.
        private static bool IsOneKind(IReadOnlyList<TreeNode> nodes) =>
            nodes.All(node => node.Item != null && node.Owner != null && node.Parent?.Group != null)
            && nodes.Select(node => node.Parent!.Group!.GetType()).Distinct().Count() == 1
            && nodes.Select(node => node.Item!).Distinct(ReferenceEqualityComparer.Instance).Count() == nodes.Count;
    }
}
