// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // One candidate per slot that the real drop takes, in tree order.
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

        public IReadOnlyList<ListTreeCandidate> Of(ListTreeSnapshot snapshot, ListTreeRow source)
        {
            if (source.Node is not { } node)
                return Array.Empty<ListTreeCandidate>();

            return slots.Of(snapshot, node)
                .Where(slot => IsTakenByTheDrop(node, slot))
                .Select(slot => CandidateOf(snapshot, source, node, slot))
                .OfType<ListTreeCandidate>()
                .ToArray();
        }

        // The real drop refuses a move that shared items block, and a list can refuse an item.
        private bool IsTakenByTheDrop(TreeNode source, ListTreeSlot slot) =>
            dropService.BlockReason(source, slot.Target) == null;

        private ListTreeCandidate? CandidateOf(ListTreeSnapshot snapshot, ListTreeRow source, TreeNode node, ListTreeSlot slot)
        {
            var moved = ListTreeMove.Of(snapshot, source, slot);
            var layout = ListTreeLayout.Of(moved, sizes);
            return moved.RowOf(node) is { } row && layout.PlaceOf(row.Key) is { } place
                ? new ListTreeCandidate(slot, moved, layout, new ListTreeSpot(place.Y, place.Depth))
                : null;
        }
    }
}
