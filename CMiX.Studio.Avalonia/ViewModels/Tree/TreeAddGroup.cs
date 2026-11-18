// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The id has the prefix group. Category is null when the items have different kinds.
    public sealed record TreeAddGroup(string Id, string Header, TreeCategory? Category, IReadOnlyList<TreeAdd> Items);
}
