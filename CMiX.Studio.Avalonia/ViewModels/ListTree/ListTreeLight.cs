// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The keys are rows of the live snapshot. Every row that is not bright is dimmed.
    public sealed class ListTreeLight
    {
        public ListTreeLight(IReadOnlySet<ListTreeRowKey> possibleTargets, IReadOnlySet<ListTreeRowKey> brightRows)
        {
            PossibleTargets = possibleTargets;
            BrightRows = brightRows;
        }

        public IReadOnlySet<ListTreeRowKey> PossibleTargets { get; }

        public IReadOnlySet<ListTreeRowKey> BrightRows { get; }

        public bool IsBright(ListTreeRowKey key) => BrightRows.Contains(key);
    }
}
