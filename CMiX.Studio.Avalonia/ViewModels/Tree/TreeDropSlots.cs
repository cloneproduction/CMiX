// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // A place in the list of drop slots. IsStart marks the place where the source is now: a drop there moves nothing.
    public sealed record DropSlot(TreeNode Target, DropPosition Position, bool IsStart = false);

    // The places where a source node can drop, in the order of the rows that show, from top to bottom.
    // A slot is a place that ProjectTree.CanDrop accepts, plus one start slot: Before the source itself.
    // Two slots that put the item at the same index of the same list are one place. The list keeps one of them:
    // Before a row over After a row (After A and Before B draw the same bar), a bar over Inside, and Inside the group
    // over Inside a row that passes the item to that group (a layer or a composition).
    // The list is a snapshot. Make a new one when a row opens or closes, or when the tree changes.
    public sealed class TreeDropSlots
    {
        private readonly List<DropSlot> slots = new();

        public TreeDropSlots(ProjectTree tree, TreeNode source)
        {
            Source = source;
            if (!tree.CanDrag(source))
                return;

            var candidates = new List<DropSlot>();
            foreach (var row in TreeWalk.VisibleRows(tree.Roots))
                AddCandidates(tree, source, row, candidates);

            // For each place, the slot with the lowest rank stays.
            var kept = candidates
                .GroupBy(slot => KeyOf(slot))
                .Select(group => group.MinBy(Rank)!)
                .ToHashSet();
            slots.AddRange(candidates.Where(kept.Contains));
            StartIndex = slots.FindIndex(slot => slot.IsStart);
        }

        public TreeNode Source { get; }

        public IReadOnlyList<DropSlot> Slots => slots;

        // The index of the start slot, or -1 when the source row does not show or cannot be dragged.
        public int StartIndex { get; } = -1;

        // The index of the slot for this place, or -1 when no slot has it. A place that the list keeps in another form
        // (After A for Before B) gives the slot of that form. A place that moves nothing gives the start slot.
        public int IndexOf(TreeNode target, DropPosition position)
        {
            var exact = slots.FindIndex(slot => !slot.IsStart && ReferenceEquals(slot.Target, target) && slot.Position == position);
            if (exact >= 0)
                return exact;

            var key = KeyOf(new DropSlot(target, position));
            if (key == null)
                return -1;

            if (StartIndex >= 0 && IsStartKey(key))
                return StartIndex;

            return slots.FindIndex(slot => !slot.IsStart && KeyOf(slot) == key);
        }

        // The slot under the index. It stops at the last slot. An index of -1 gives the first slot. -1 when the list is empty.
        public int Next(int index) => slots.Count == 0 ? -1 : Math.Clamp(index + 1, 0, slots.Count - 1);

        // The slot over the index. It stops at the first slot. An index of -1 gives the last slot. -1 when the list is empty.
        public int Previous(int index) => slots.Count == 0 ? -1 : index < 0 ? slots.Count - 1 : Math.Max(index - 1, 0);

        // The places at a row, in the order of the rows: Before the row, Inside it, then After each block that ends at this row,
        // the inner block first. The source and the rows under it are no target. The start slot is at the source row.
        private void AddCandidates(ProjectTree tree, TreeNode source, TreeNode row, List<DropSlot> candidates)
        {
            if (ReferenceEquals(row, source))
                candidates.Add(new DropSlot(source, DropPosition.Before, IsStart: true));
            else if (!row.IsOrIsUnder(source))
            {
                Add(tree, source, row, DropPosition.Before, candidates);
                Add(tree, source, row, DropPosition.Inside, candidates);
            }

            for (var block = row; block != null && ReferenceEquals(TreeDropService.LastVisible(block), row); block = block.Parent)
                if (!block.IsOrIsUnder(source))
                    Add(tree, source, block, DropPosition.After, candidates);
        }

        private static void Add(ProjectTree tree, TreeNode source, TreeNode target, DropPosition position, List<DropSlot> candidates)
        {
            if (target.Parent != null && tree.CanDrop(source, target, position))
                candidates.Add(new DropSlot(target, position));
        }

        // Before wins over After, a bar wins over Inside, and Inside the list row wins over a row that passes the item to it.
        private int Rank(DropSlot slot) => slot.Position switch
        {
            DropPosition.Before => 0,
            DropPosition.After => 1,
            _ => ReferenceEquals(TreeDropService.InsideOf(Source, slot.Target), slot.Target) ? 2 : 3
        };

        // The list that takes the item and the index in it, counted before the move, as TreeDropService.Resolve finds them.
        // Null when the place has no list. The start slot has a key of its own.
        private SlotKey? KeyOf(DropSlot slot)
        {
            if (slot.IsStart)
                return new SlotKey(slot, 0);

            if (slot.Position == DropPosition.Inside)
                return TreeDropService.InsideOf(Source, slot.Target).ChildManager is PrefabManager inside
                    ? new SlotKey(inside, inside.ManagerData.Items.Count)
                    : null;

            if (slot.Target.Owner is not { } owner || slot.Target.Item == null)
                return null;

            return new SlotKey(owner, owner.ManagerData.Items.IndexOf(slot.Target.Item) + (slot.Position == DropPosition.After ? 1 : 0));
        }

        // The two places in the own list of the source that leave it where it is: just before it and just after it.
        private bool IsStartKey(SlotKey key)
        {
            if (!ReferenceEquals(key.List, Source.Owner) || Source.Item == null)
                return false;

            var index = Source.Owner!.ManagerData.Items.IndexOf(Source.Item);
            return key.Index == index || key.Index == index + 1;
        }
    }
}
