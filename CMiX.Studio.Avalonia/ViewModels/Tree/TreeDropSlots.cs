// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // A drop at the start slot moves nothing.
    public sealed record DropSlot(TreeNode Target, DropPosition Position, bool IsStart = false);

    // The drop places in row order. Two places that give the same index in the same list are one slot (see Rank).
    // The list is a snapshot: make a new one when a row opens or closes, or when the tree changes.
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

            var kept = candidates
                .GroupBy(slot => KeyOf(slot))
                .Select(group => group.MinBy(Rank)!)
                .ToHashSet();
            slots.AddRange(candidates.Where(kept.Contains));
            StartIndex = slots.FindIndex(slot => slot.IsStart);
        }

        public TreeNode Source { get; }

        public IReadOnlyList<DropSlot> Slots => slots;

        public int StartIndex { get; } = -1;

        // A place that the list keeps in another form gives the slot of that form.
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

        public int Next(int index) => slots.Count == 0 ? -1 : Math.Clamp(index + 1, 0, slots.Count - 1);

        public int Previous(int index) => slots.Count == 0 ? -1 : index < 0 ? slots.Count - 1 : Math.Max(index - 1, 0);

        // The inner block ends first. The source and the rows under it are no target.
        private void AddCandidates(ProjectTree tree, TreeNode source, TreeNode row, List<DropSlot> candidates)
        {
            if (ReferenceEquals(row, source))
                candidates.Add(new DropSlot(source, DropPosition.Before, IsStart: true));
            else if (!row.IsOrIsUnder(source))
            {
                Add(tree, source, row, DropPosition.Before, candidates);
                Add(tree, source, row, DropPosition.Inside, candidates);
            }

            for (var block = row; block != null && ReferenceEquals(TreeRowOrder.LastVisible(block), row); block = block.Parent)
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

        // The index is counted before the move, as TreeDropService.Resolve finds it.
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

        private bool IsStartKey(SlotKey key)
        {
            if (!ReferenceEquals(key.List, Source.Owner) || Source.Item == null)
                return false;

            var index = Source.Owner!.ManagerData.Items.IndexOf(Source.Item);
            return key.Index == index || key.Index == index + 1;
        }
    }
}
