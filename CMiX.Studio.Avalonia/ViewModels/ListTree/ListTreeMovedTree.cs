// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The tree as if the block were dropped on the slot. It is made only when a drag shows it, then kept.
    public sealed class ListTreeMovedTree
    {
        private readonly ListTreeSnapshot live;
        private readonly ListTreeBlock block;
        private readonly ListTreeSlot slot;
        private readonly ListTreeSizes sizes;
        private ListTreeSnapshot? snapshot;
        private ListTreeLayout? layout;

        public ListTreeMovedTree(ListTreeSnapshot live, ListTreeBlock block, ListTreeSlot slot, ListTreeSizes sizes)
        {
            this.live = live;
            this.block = block;
            this.slot = slot;
            this.sizes = sizes;
        }

        public ListTreeSnapshot Snapshot => snapshot ??= ListTreeMove.Of(live, block, slot);

        public ListTreeLayout Layout => layout ??= ListTreeLayout.Of(Snapshot, sizes);
    }
}
