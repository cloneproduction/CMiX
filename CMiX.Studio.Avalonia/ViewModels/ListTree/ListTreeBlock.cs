// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The rows that one move takes, in tree order, with the pressed row among them.
    public sealed record ListTreeBlock(IReadOnlyList<ListTreeRow> Rows, ListTreeRow Pressed)
    {
        public static ListTreeBlock Of(ListTreeRow row) => new(new[] { row }, row);
    }
}
