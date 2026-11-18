// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The drag look of the rows while a node is dragged: the rows that take it inside are candidates, the rows next to which it
    // can go are neighbors, a collapsed row with such a row under it gives a hint, a row that refuses the node by a rule is
    // blocked, and the other rows are dimmed (see TreeNode.IsDropCandidate). The rules are the ones of the drop itself.
    public sealed class TreeDragState
    {
        private readonly TreeDropService dropService;

        // The rows that MarkRows marked. ClearRows clears only these.
        private readonly List<TreeNode> marked = new();

        public TreeDragState(TreeDropService dropService)
        {
            this.dropService = dropService;
        }

        // A group (Entities, Cameras, Lights) has a fixed place in its layer. Only an item can be dragged.
        public bool CanDrag(TreeNode node) => node.Item != null && node.Owner != null;

        // A row that the drag marks as blocked takes no drop: the marker shows nothing over it.
        public bool CanDrop(TreeNode source, TreeNode target, DropPosition position) =>
            !target.IsDropBlocked && dropService.CanDrop(source, target, position);

        public void MarkRows(TreeNode root, TreeNode source)
        {
            marked.Clear();
            MarkRow(root, root, source);
        }

        public void ClearRows()
        {
            foreach (var node in marked)
                node.ClearDragLook();

            marked.Clear();
        }

        // Sets the drag look of a row and of the rows under it. It tells whether a row that takes the node is at or under the row.
        private bool MarkRow(TreeNode root, TreeNode node, TreeNode source)
        {
            var below = false;
            foreach (var child in node.Children)
                below |= MarkRow(root, child, source);

            var candidate = node != root && dropService.IsInsideTarget(source, node);
            var neighbor = node != root && dropService.IsNeighbor(source, node);
            var reason = node != root && (candidate || neighbor || dropService.IsTargetByKind(source, node)) ? dropService.BlockReason(source, node) : null;
            if (reason != null)
                candidate = neighbor = false;

            node.ShowDragLook(new DragLook(ReferenceEquals(node, source), candidate, neighbor, below, reason));
            marked.Add(node);
            return below || candidate || neighbor;
        }
    }
}
