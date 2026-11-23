// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // One row of a move: its item and the list that the item leaves.
    public sealed record ListTreeMoveMember(IControl Item, PrefabManager From, ListTreeRow Source);
}
