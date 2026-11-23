// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The move of a block to a slot. The holder is the key of the row that shows the list of the slot.
    // The index is the place of the first row of the block in the list after every row of the block leaves it.
    public sealed record ListTreeMovePlan(IReadOnlyList<ListTreeMoveMember> Members, int PressedIndex, PrefabManager To, int Index, ListTreeRowKey Holder)
    {
        public bool IsInItsOwnList => Members.All(member => ReferenceEquals(member.From, To));

        public ListTreeMoveMember Pressed => Members[PressedIndex];

        public bool IsPressedInItsOwnList => ReferenceEquals(Pressed.From, To);

        public bool IsLeaving(object list, object item) =>
            Members.Any(member => ReferenceEquals(member.Item, item) && ReferenceEquals(member.From, list));

        // A second row of a shared card has no node, so its group row has none.
        public TreeNode? GroupNodeOf(ListTreeRow owner) =>
            owner.Node?.Children.FirstOrDefault(child => ReferenceEquals(child.Group?.Manager, To));

        // A holder that shows the list of the item already keeps the row that it has. The row of the member itself shows closed.
        public static bool KeepsItsOwnRowOpen(ListTreeMoveMember member, ListTreeRow own) => !own.Key.Equals(member.Row.Key);

        public static ListTreeMovePlan Of(ListTreeRow source, ListTreeSlot slot) => Of(ListTreeBlock.Of(source), slot);

        public static ListTreeMovePlan Of(ListTreeBlock block, ListTreeSlot slot)
        {
            var members = block.Rows.Select(MemberOf).ToArray();
            return new ListTreeMovePlan(members, IndexOfPressed(block), slot.ListPlace.List, IndexOf(members, slot.ListPlace), HolderOf(slot));
        }

        private static ListTreeMoveMember MemberOf(ListTreeRow row)
        {
            var node = row.Node ?? throw new ArgumentException("A row of the block is not a row of the live tree.", nameof(row));
            return new ListTreeMoveMember(node.Item!, node.Owner!, row);
        }

        private static int IndexOfPressed(ListTreeBlock block)
        {
            for (var index = 0; index < block.Rows.Count; index++)
                if (ReferenceEquals(block.Rows[index], block.Pressed))
                    return index;

            throw new ArgumentException("The pressed row is not a row of the block.", nameof(block));
        }

        // The index is counted after the items leave their list, as in TreeDropService.
        private static int IndexOf(IReadOnlyList<ListTreeMoveMember> members, ListPlace listPlace)
        {
            var items = listPlace.List.ManagerData.Items;
            return listPlace.Index - members.Count(member => ReferenceEquals(member.From, listPlace.List) && items.IndexOf(member.Item) < listPlace.Index);
        }

        private static ListTreeRowKey HolderOf(ListTreeSlot slot)
        {
            if (slot.Position != DropPosition.Inside)
                return slot.ShownTarget.Parent!;

            return ReferenceEquals(slot.ShownTarget.Model, slot.ListPlace.List) ? slot.ShownTarget : new ListTreeRowKey(slot.ListPlace.List, slot.ShownTarget);
        }
    }
}
