// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The rules of the drag look are the rules of the drop itself.
    public sealed class TreeDragState
    {
        private readonly TreeDropService dropService;

        private readonly List<TreeNode> marked = new();

        public TreeDragState(TreeDropService dropService)
        {
            this.dropService = dropService;
        }

        // A group has a fixed place in its layer.
        public bool CanDrag(TreeNode node) => node.Item != null && node.Owner != null;

        // The marker shows nothing over a blocked row.
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
