// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Core;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The move of the source to a slot. The holder is the key of the row that shows the list of the slot.
    public sealed record ListTreeMovePlan(IControl Item, PrefabManager From, PrefabManager To, int Index, ListTreeRow Source, ListTreeRowKey Holder)
    {
        public bool IsInItsOwnList => ReferenceEquals(From, To);

        public static ListTreeMovePlan Of(ListTreeRow source, ListTreeSlot slot)
        {
            var node = source.Node ?? throw new ArgumentException("The source is not a row of the live tree.", nameof(source));
            var item = node.Item!;
            var from = node.Owner!;
            var to = slot.ListPlace.List;
            return new ListTreeMovePlan(item, from, to, IndexOf(item, from, slot.ListPlace), source, HolderOf(slot));
        }

        // The index is counted after the item leaves its list, as in TreeDropService.
        private static int IndexOf(IControl item, PrefabManager from, ListPlace listPlace)
        {
            var oldIndex = from.ManagerData.Items.IndexOf(item);
            return ReferenceEquals(listPlace.List, from) && listPlace.Index > oldIndex ? listPlace.Index - 1 : listPlace.Index;
        }

        private static ListTreeRowKey HolderOf(ListTreeSlot slot)
        {
            if (slot.Position != DropPosition.Inside)
                return slot.ShownTarget.Parent!;

            return ReferenceEquals(slot.ShownTarget.Model, slot.ListPlace.List) ? slot.ShownTarget : new ListTreeRowKey(slot.ListPlace.List, slot.ShownTarget);
        }
    }
}
