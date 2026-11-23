// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The live row that gets the dropped row. The key is null when the drop lands at the root.
    public sealed record ListTreeDropHolder(ListTreeRowKey? Key, bool IsSameParent)
    {
        // Option E hides an empty list, so its card is the holder that the live tree shows, as in ListTreeBrightRows.
        public static ListTreeDropHolder Of(IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> liveRows, ListTreeRow source, ListTreeCandidate candidate)
        {
            var parent = candidate.Place.Key.Parent;
            var key = parent == null || liveRows.ContainsKey(parent) ? parent : candidate.Slot.ShownTarget;
            return new ListTreeDropHolder(key, Equals(parent, source.Key.Parent));
        }
    }
}
