// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;
using CMiX.Studio.Avalonia.Services;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed record DropMove(DropOutcome Outcome, TreeNode? MovedNode);

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
