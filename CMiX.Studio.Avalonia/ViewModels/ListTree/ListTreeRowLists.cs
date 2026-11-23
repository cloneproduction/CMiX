// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The lists of the model of a row, in the order of their group rows.
    public static class ListTreeRowLists
    {
        public static IReadOnlyList<PrefabManager> Of(ListTreeRow row) =>
            row.Key.Model is IControl owner ? TreeGroup.GroupsOf(owner).Select(group => group.Manager).ToArray() : Array.Empty<PrefabManager>();

        // -1 when the model is not one of the lists.
        public static int IndexOf(IReadOnlyList<PrefabManager> lists, object model)
        {
            for (var index = 0; index < lists.Count; index++)
                if (ReferenceEquals(lists[index], model))
                    return index;

            return -1;
        }
    }
}
