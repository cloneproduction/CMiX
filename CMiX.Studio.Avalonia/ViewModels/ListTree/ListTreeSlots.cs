// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The places of the visible tree where the source can land, in tree order, one per list and index.
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

            var candidates = new List<Candidate>();
            AddRows(snapshot.Roots, source, candidates);
            return OnePerPlace(candidates);
        }

        private void AddRows(IEnumerable<ListTreeRow> rows, TreeNode source, List<Candidate> candidates)
        {
            foreach (var row in rows)
            {
                if (ReferenceEquals(row.Node, source))
                    candidates.Add(CurrentPlaceOf(row));
                else
                    AddRow(row, source, candidates);
            }
        }

        private void AddRow(ListTreeRow row, TreeNode source, List<Candidate> candidates)
        {
            Add(row, DropPosition.Before, source, candidates);
            if (OffersInside(row))
                AddInside(row, source, candidates);

            if (row.IsExpanded)
                AddRows(row.Children, source, candidates);

            Add(row, DropPosition.After, source, candidates);
        }

        private void Add(ListTreeRow row, DropPosition position, TreeNode source, List<Candidate> candidates)
        {
            if (dropService.PlaceOf(source, row.Node!, position) is { } place)
                candidates.Add(new Candidate(new ListTreeSlot(row.Node!, position, place, row.Key), row.Depth));
        }

        private void AddInside(ListTreeRow row, TreeNode source, List<Candidate> candidates)
        {
            if (dropService.PlaceOf(source, row.Node!, DropPosition.Inside) is { } place)
                candidates.Add(new Candidate(new ListTreeSlot(row.Node!, DropPosition.Inside, place, row.Key), row.Depth));
            else
                AddInsideHiddenGroups(row, source, candidates);
        }

        // A drop Inside the card lands in its empty list of the kind of the source.
        private void AddInsideHiddenGroups(ListTreeRow row, TreeNode source, List<Candidate> candidates)
        {
            foreach (var group in row.Node!.Children.Where(IsHiddenGroup))
                if (dropService.PlaceOf(source, group, DropPosition.Inside) is { } place)
                    candidates.Add(new Candidate(new ListTreeSlot(group, DropPosition.Inside, place, row.Key), row.Depth + 1));
        }

        private static bool IsHiddenGroup(TreeNode node) => node.Group != null && node.Children.Count == 0;

        // The drag opens a closed card with rows first, then the places are made again.
        private static bool OffersInside(ListTreeRow row) =>
            row.IsCard && (row.IsExpanded || row.Children.Count == 0);

        private static Candidate CurrentPlaceOf(ListTreeRow row)
        {
            var owner = row.Node!.Owner!;
            var place = new ListPlace(owner, owner.ManagerData.Items.IndexOf(row.Node.Item!));
            return new Candidate(new ListTreeSlot(row.Node, DropPosition.Before, place, row.Key), row.Depth);
        }

        private static IReadOnlyList<ListTreeSlot> OnePerPlace(List<Candidate> candidates)
        {
            var kept = candidates.GroupBy(candidate => candidate.Slot.ListPlace).Select(Deepest).ToHashSet();
            return candidates.Where(kept.Contains).Select(candidate => candidate.Slot).ToArray();
        }

        // At one depth, Before wins over After and After wins over Inside, as in TreeDropSlots.
        private static Candidate Deepest(IEnumerable<Candidate> samePlace) =>
            samePlace.OrderByDescending(candidate => candidate.Depth).ThenBy(candidate => candidate.Slot.Position).First();

        private sealed record Candidate(ListTreeSlot Slot, int Depth);
    }
}
