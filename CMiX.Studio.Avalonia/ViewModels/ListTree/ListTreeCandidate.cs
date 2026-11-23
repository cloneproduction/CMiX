// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // A legal slot with the tree as if the source were dropped on it. The place is the box of the moved row.
    public sealed record ListTreeCandidate
    {
        private readonly ListTreeSnapshot live;
        private readonly ListTreeBlock block;
        private readonly ListTreeSizes sizes;
        private ListTreeSnapshot? snapshot;
        private ListTreeLayout? layout;

        public ListTreeCandidate(ListTreeSlot slot, ListTreePlace place, ListTreeSnapshot live, ListTreeBlock block, ListTreeSizes sizes)
        {
            Slot = slot;
            Place = place;
            this.live = live;
            this.block = block;
            this.sizes = sizes;
        }

        public ListTreeSlot Slot { get; }

        public ListTreePlace Place { get; init; }

        public ListTreeSnapshot Snapshot => snapshot ??= ListTreeMove.Of(live, block, Slot);

        public ListTreeLayout Layout => layout ??= ListTreeLayout.Of(Snapshot, sizes);
    }
}
