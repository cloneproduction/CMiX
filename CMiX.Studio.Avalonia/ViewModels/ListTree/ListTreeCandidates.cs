// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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

        public IReadOnlyList<ListTreeCandidate> Of(ListTreeSnapshot snapshot, ListTreeRow source) =>
            slots.Of(snapshot, source.Node!)
                .Where(slot => IsTakenByTheDrop(source.Node!, slot))
                .Select(slot => CandidateOf(snapshot, source, slot))
                .OfType<ListTreeCandidate>()
                .ToArray();

        // The real drop refuses a move that shared items block, and a list can refuse an item.
        private bool IsTakenByTheDrop(TreeNode source, ListTreeSlot slot) =>
            dropService.BlockReason(source, slot.Target) == null;

        private ListTreeCandidate? CandidateOf(ListTreeSnapshot snapshot, ListTreeRow source, ListTreeSlot slot)
        {
            var moved = ListTreeMove.Of(snapshot, source, slot);
            var layout = ListTreeLayout.Of(moved, sizes);
            return SpotOf(moved, layout, source.Node!) is { } spot ? new ListTreeCandidate(slot, moved, layout, spot) : null;
        }

        private static ListTreeSpot? SpotOf(ListTreeSnapshot moved, ListTreeLayout layout, TreeNode source) =>
            moved.VisibleRows().FirstOrDefault(row => ReferenceEquals(row.Node, source)) is { } row && layout.PlaceOf(row.Key) is { } place
                ? new ListTreeSpot(place.Y, place.Depth)
                : null;
    }
}
