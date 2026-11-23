// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // During a drag, the rows where the source can land and the rows next to it stay bright.
    public static class ListTreeBrightRows
    {
        public static ListTreeLight Of(ListTreeLiveRows live, ListTreeRow source, IEnumerable<ListTreeCandidate> candidates)
        {
            var rows = live.ByKey;
            var targets = PossibleTargetsOf(rows, source, candidates);
            return new ListTreeLight(targets, BrightRowsOf(rows, targets, source));
        }

        private static IReadOnlySet<ListTreeRowKey> PossibleTargetsOf(
            IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> rows, ListTreeRow source, IEnumerable<ListTreeCandidate> candidates)
        {
            var targets = candidates.Select(candidate => candidate.LiveHolderIn(rows)).OfType<ListTreeRowKey>().ToHashSet();
            if (source.Key.Parent is { } parent)
                targets.Add(parent);

            return targets;
        }

        private static IReadOnlySet<ListTreeRowKey> BrightRowsOf(
            IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> rows, IReadOnlySet<ListTreeRowKey> targets, ListTreeRow source)
        {
            var bright = new HashSet<ListTreeRowKey>(targets) { source.Key };
            bright.UnionWith(CardsOfLists(rows, targets));
            foreach (var target in targets)
                if (rows.TryGetValue(target, out var row))
                    bright.UnionWith(row.Children.Where(child => !child.IsGroup).Select(child => child.Key));

            return bright;
        }

        // The card that owns a target list stays bright, but its other lists do not.
        private static IEnumerable<ListTreeRowKey> CardsOfLists(IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> rows, IEnumerable<ListTreeRowKey> targets) =>
            targets.Where(target => rows.TryGetValue(target, out var row) && row.IsGroup).Select(target => target.Parent).OfType<ListTreeRowKey>();
    }
}
