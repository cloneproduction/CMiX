// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // During a drag, the rows where the source can land and the rows next to it stay bright.
    public static class ListTreeBrightRows
    {
        public static ListTreeLight Of(ListTreeSnapshot live, ListTreeRow source, IEnumerable<ListTreeCandidate> candidates)
        {
            var rows = RowsByKey(live);
            var targets = PossibleTargetsOf(rows, source, candidates);
            return new ListTreeLight(targets, BrightRowsOf(rows, targets, source));
        }

        private static IReadOnlySet<ListTreeRowKey> PossibleTargetsOf(
            IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> rows, ListTreeRow source, IEnumerable<ListTreeCandidate> candidates)
        {
            var targets = candidates.Select(candidate => LiveHolderOf(rows, source, candidate)).OfType<ListTreeRowKey>().ToHashSet();
            if (source.Key.Parent is { } parent)
                targets.Add(parent);

            return targets;
        }

        // Option E hides an empty list, so its card is the target that the live tree shows.
        private static ListTreeRowKey? LiveHolderOf(IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> rows, ListTreeRow source, ListTreeCandidate candidate)
        {
            var holder = MovedRowOf(candidate.Snapshot, source)?.Key.Parent;
            if (holder == null)
                return null;

            return rows.ContainsKey(holder) ? holder : candidate.Slot.ShownTarget;
        }

        private static ListTreeRow? MovedRowOf(ListTreeSnapshot moved, ListTreeRow source) =>
            AllRows(moved.Roots).FirstOrDefault(row => ReferenceEquals(row.Node, source.Node));

        private static IReadOnlySet<ListTreeRowKey> BrightRowsOf(
            IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> rows, IReadOnlySet<ListTreeRowKey> targets, ListTreeRow source)
        {
            var bright = new HashSet<ListTreeRowKey>(targets) { source.Key };
            foreach (var target in targets)
                if (rows.TryGetValue(target, out var row))
                    bright.UnionWith(row.Children.Select(child => child.Key));

            return bright;
        }

        private static IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> RowsByKey(ListTreeSnapshot snapshot) =>
            AllRows(snapshot.Roots).ToDictionary(row => row.Key);

        private static IEnumerable<ListTreeRow> AllRows(IEnumerable<ListTreeRow> rows) =>
            rows.SelectMany(row => AllRows(row.Children).Prepend(row));
    }
}
