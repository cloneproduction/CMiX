// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The keys of a grab step through the candidates of its drag, in tree order. A step stops at the first and at the last.
    public static class ListTreeGrabSteps
    {
        // The candidate of the grabbed row itself. -1 when it has none.
        public static int StartIndexOf(ListTreeDrag drag) => IndexOf(drag.Candidates, candidate => ReferenceEquals(candidate.Slot.Target, drag.Pressed));

        // The shown candidate, else the start.
        public static int ShownIndexOf(ListTreeDrag drag)
        {
            var index = drag.Look is { } look ? IndexOf(drag.Candidates, candidate => candidate.Slot == look.Candidate.Slot) : -1;
            return index >= 0 ? index : StartIndexOf(drag);
        }

        public static int IndexBelow(ListTreeDrag drag) => Math.Min(ShownIndexOf(drag) + 1, drag.Candidates.Count - 1);

        // With no shown candidate and no start, a step up goes to the last.
        public static int IndexAbove(ListTreeDrag drag)
        {
            var index = ShownIndexOf(drag);
            return index < 0 ? drag.Candidates.Count - 1 : Math.Max(index - 1, 0);
        }

        private static int IndexOf(IReadOnlyList<ListTreeCandidate> candidates, Func<ListTreeCandidate, bool> match)
        {
            for (var i = 0; i < candidates.Count; i++)
                if (match(candidates[i]))
                    return i;

            return -1;
        }
    }
}
