// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMiX.Studio.Avalonia.Notices;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed record DropMove(DropOutcome Outcome, TreeNode? MovedNode);

    // The row that the drop did not move, the place that it did not take, and why.
    public sealed record DropRefusal(TreeNode Source, TreeNode Target, DropOutcome Outcome);

    // The refusal is null when every row of the block moved or was in its place already.
    public sealed record DropBlockMove(IReadOnlyList<TreeNode> MovedNodes, DropRefusal? Refusal);

    public sealed class TreeDrop
    {
        private readonly TreeDropService dropService;
        private readonly IMoveBlockedNotice notice;

        public TreeDrop(TreeDropService dropService, IMoveBlockedNotice notice)
        {
            this.dropService = dropService;
            this.notice = notice;
        }

        public DropMove Move(TreeNode root, TreeNode source, TreeNode target, DropPosition position)
        {
            var inside = TreeDropService.InsideOf(source, target);
            var movedItem = source.Item;
            var outcome = dropService.Drop(source, target, position);
            if (!outcome.Moved)
                return new DropMove(outcome, null);

            // A layer row passes the item to its group, so the group opens too.
            var owner = position == DropPosition.Inside ? inside : target.Parent;
            OpenHolders(owner);

            // Search under the holder only. A shared item is also under other layers.
            var moved = TreeWalk.FindNode((owner ?? root).Children, node => ReferenceEquals(node.Item, movedItem));
            return new DropMove(outcome, moved);
        }

        // Each row lands after the row before it, so the rows keep their tree order. A row that is in its place already does not move.
        public DropBlockMove MoveBlock(TreeNode root, IReadOnlyList<TreeNode> nodes, TreeNode target, DropPosition position)
        {
            if (RefusalOf(nodes, target, position) is { } refusal)
                return new DropBlockMove(Array.Empty<TreeNode>(), refusal);

            var moved = new List<TreeNode>();
            var anchor = target;
            var at = position;
            foreach (var source in nodes)
            {
                var move = Move(root, source, anchor, at);
                if ((move.MovedNode ?? StayingNodeOf(source, anchor, at, move.Outcome)) is not { } landed)
                    return new DropBlockMove(moved, new DropRefusal(source, anchor, move.Outcome));

                moved.Add(landed);
                anchor = landed;
                at = DropPosition.After;
            }

            return new DropBlockMove(moved, null);
        }

        // Every row of the block lands in the list of the target, so each row is read against the target before any row moves.
        private DropRefusal? RefusalOf(IReadOnlyList<TreeNode> nodes, TreeNode target, DropPosition position) =>
            nodes
                .Select(source => dropService.RefusalOf(source, target, position) is { } outcome ? new DropRefusal(source, target, outcome) : null)
                .FirstOrDefault(refusal => refusal != null);

        // A drop in the list of the row that moves nothing and has no reason leaves the row in its place.
        private static TreeNode? StayingNodeOf(TreeNode source, TreeNode anchor, DropPosition position, DropOutcome outcome)
        {
            var list = position == DropPosition.Inside ? TreeDropService.InsideOf(source, anchor).ChildManager : anchor.Owner;
            return !outcome.Moved && outcome.Blocking.Count == 0 && ReferenceEquals(source.Owner, list) ? source : null;
        }

        public Task NoticeBlockAsync(TreeNode source, TreeNode target, DropOutcome outcome)
        {
            if (outcome.Moved || outcome.Blocking.Count == 0)
                return Task.CompletedTask;

            return notice.ShowAsync(dropService.DescribeBlock(source, target, outcome.Blocking));
        }

        private static void OpenHolders(TreeNode? owner)
        {
            for (var holder = owner; holder != null; holder = holder.Parent)
                holder.IsExpanded = true;
        }
    }
}
