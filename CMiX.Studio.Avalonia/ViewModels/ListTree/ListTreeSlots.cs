// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The slots of the visible tree where the source can land, in tree order, one per list and index.
    public sealed class ListTreeSlots
    {
        private readonly TreeDropService dropService;

        public ListTreeSlots(TreeDropService dropService)
        {
            this.dropService = dropService;
        }

        public IReadOnlyList<ListTreeSlot> Of(ListTreeSnapshot snapshot, TreeNode source)
        {
            if (source.Item == null || source.Owner == null)
                return Array.Empty<ListTreeSlot>();

            var slots = new List<SlotAtDepth>();
            AddRows(snapshot.Roots, source, slots);
            return OnePerListPlace(slots);
        }

        private void AddRows(IEnumerable<ListTreeRow> rows, TreeNode source, List<SlotAtDepth> slots)
        {
            foreach (var row in rows)
            {
                if (row.Node is not { } node)
                    continue;

                if (ReferenceEquals(node, source))
                    slots.Add(CurrentSlotOf(row, node));
                else
                    AddRow(row, node, source, slots);
            }
        }

        private void AddRow(ListTreeRow row, TreeNode node, TreeNode source, List<SlotAtDepth> slots)
        {
            Add(row, node, DropPosition.Before, source, slots);
            if (OffersInside(row))
                AddInside(row, node, source, slots);

            if (row.IsExpanded)
                AddRows(row.Children, source, slots);

            Add(row, node, DropPosition.After, source, slots);
        }

        private void Add(ListTreeRow row, TreeNode node, DropPosition position, TreeNode source, List<SlotAtDepth> slots)
        {
            if (dropService.PlaceOf(source, node, position) is { } listPlace)
                slots.Add(new SlotAtDepth(new ListTreeSlot(node, position, listPlace, row.Key), row.Depth));
        }

        private void AddInside(ListTreeRow row, TreeNode node, TreeNode source, List<SlotAtDepth> slots)
        {
            if (dropService.PlaceOf(source, node, DropPosition.Inside) is { } listPlace)
                slots.Add(new SlotAtDepth(new ListTreeSlot(node, DropPosition.Inside, listPlace, row.Key), row.Depth));
            else
                AddInsideHiddenGroups(row, node, source, slots);
        }

        // A drop Inside the card lands in its empty list of the kind of the source.
        private void AddInsideHiddenGroups(ListTreeRow row, TreeNode node, TreeNode source, List<SlotAtDepth> slots)
        {
            foreach (var group in node.Children.Where(IsHiddenGroup))
                if (dropService.PlaceOf(source, group, DropPosition.Inside) is { } listPlace)
                    slots.Add(new SlotAtDepth(new ListTreeSlot(group, DropPosition.Inside, listPlace, row.Key), row.Depth + 1));
        }

        private static bool IsHiddenGroup(TreeNode node) => node.Group != null && node.Children.Count == 0;

        // The drag opens a closed card with rows first, then the slots are made again.
        private static bool OffersInside(ListTreeRow row) =>
            row.IsCard && (row.IsExpanded || row.Children.Count == 0);

        private static SlotAtDepth CurrentSlotOf(ListTreeRow row, TreeNode source)
        {
            var owner = source.Owner!;
            var listPlace = new ListPlace(owner, owner.ManagerData.Items.IndexOf(source.Item!));
            return new SlotAtDepth(new ListTreeSlot(source, DropPosition.Before, listPlace, row.Key), row.Depth);
        }

        private static IReadOnlyList<ListTreeSlot> OnePerListPlace(List<SlotAtDepth> slots)
        {
            var kept = slots.GroupBy(atDepth => atDepth.Slot.ListPlace).Select(Deepest).ToHashSet();
            return slots.Where(kept.Contains).Select(atDepth => atDepth.Slot).ToArray();
        }

        // At one depth, Before wins over After and After wins over Inside, as in TreeDropSlots.
        private static SlotAtDepth Deepest(IEnumerable<SlotAtDepth> sameListPlace) =>
            sameListPlace.OrderByDescending(atDepth => atDepth.Depth).ThenBy(atDepth => atDepth.Slot.Position).First();

        private sealed record SlotAtDepth(ListTreeSlot Slot, int Depth);
    }
}
