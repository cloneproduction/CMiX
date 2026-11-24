// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;

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

        public void MarkRows(TreeNode root, TreeNode source) => MarkRows(root, new[] { source });

        // A block is lit where every one of its nodes can go. The first blocked node gives the reason of a row.
        public void MarkRows(TreeNode root, IReadOnlyList<TreeNode> sources)
        {
            marked.Clear();
            MarkRow(root, root, sources);
        }

        public void ClearRows()
        {
            foreach (var node in marked)
                node.ClearDragLook();

            marked.Clear();
        }

        private bool MarkRow(TreeNode root, TreeNode node, IReadOnlyList<TreeNode> sources)
        {
            var below = false;
            foreach (var child in node.Children)
                below |= MarkRow(root, child, sources);

            var isSource = sources.Any(source => ReferenceEquals(node, source));
            var candidate = node != root && sources.All(source => dropService.IsInsideTarget(source, node));
            var neighbor = node != root && sources.All(source => dropService.IsNeighbor(source, node));
            var reason = node != root ? ReasonOf(node, sources, candidate || neighbor) : null;
            if (reason != null)
                candidate = neighbor = false;

            node.ShowDragLook(new DragLook(isSource, candidate, neighbor, below, reason));
            marked.Add(node);
            return below || candidate || neighbor;
        }

        private string? ReasonOf(TreeNode node, IReadOnlyList<TreeNode> sources, bool isTarget)
        {
            foreach (var source in sources)
                if ((isTarget || dropService.IsTargetByKind(source, node)) && dropService.BlockReason(source, node) is { } reason)
                    return reason;
            return null;
        }
    }
}
