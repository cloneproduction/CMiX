// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // One candidate per slot that the real drop takes for every row of the block, in tree order.
    public sealed class ListTreeCandidates
    {
        private readonly ListTreeSlots slots;
        private readonly TreeDropService dropService;
        private readonly ListTreeSizes sizes;

        public ListTreeCandidates(ListTreeSlots slots, TreeDropService dropService, ListTreeSizes sizes)
        {
            this.slots = slots;
            this.dropService = dropService;
            this.sizes = sizes;
        }

        public IReadOnlyList<ListTreeCandidate> Of(ListTreeSnapshot snapshot, ListTreeRow source) => Of(snapshot, ListTreeBlock.Of(source));

        public IReadOnlyList<ListTreeCandidate> Of(ListTreeSnapshot snapshot, ListTreeBlock block)
        {
            var counted = ListTreeCountedTree.Of(snapshot, block, sizes);
            return SlotsOf(snapshot, block)
                .Select(slot => CandidateOf(snapshot, counted, slot))
                .OfType<ListTreeCandidate>()
                .ToArray();
        }

        // The slots of the pressed row that the real drop takes for every row of the block.
        public IReadOnlyList<ListTreeSlot> SlotsOf(ListTreeSnapshot snapshot, ListTreeBlock block)
        {
            if (block.Pressed.Node is not { } pressed)
                return Array.Empty<ListTreeSlot>();

            var nodes = block.Rows.Select(row => row.Node).OfType<TreeNode>().ToArray();
            return slots.Of(snapshot, pressed).Where(slot => nodes.All(node => IsTakenByTheDrop(node, slot))).ToArray();
        }

        // The real drop refuses a move that shared items block, and a list can refuse an item.
        private bool IsTakenByTheDrop(TreeNode source, ListTreeSlot slot) =>
            dropService.BlockReason(source, slot.Target) == null && IsFreeFor(source, slot);

        // A list that has the item already does not take it again, and a row does not land under itself.
        private static bool IsFreeFor(TreeNode source, ListTreeSlot slot)
        {
            var list = slot.ListPlace.List;
            var isNewInTheList = ReferenceEquals(source.Owner, list) || !list.ManagerData.Items.Contains(source.Item!);
            var isBesideTheSource = ReferenceEquals(slot.Target, source) && slot.Position != DropPosition.Inside;
            return isNewInTheList && (isBesideTheSource || !slot.Target.IsOrIsUnder(source));
        }

        // The moved tree is made only for the candidate that a drag shows.
        private ListTreeCandidate? CandidateOf(ListTreeSnapshot snapshot, ListTreeCountedTree counted, ListTreeSlot slot) =>
            counted.PlaceOf(slot) is { } place ? new ListTreeCandidate(slot, place, new ListTreeMovedTree(snapshot, counted.Block, slot, sizes)) : null;
    }
}
