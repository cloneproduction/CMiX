// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class MoveItemUndoCommand : ItemListUndoCommand
    {
        private readonly int _oldIndex;
        private readonly int _newIndex;

        public MoveItemUndoCommand(IItemList list, int oldIndex, int newIndex) : base(list)
        {
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }

        protected override void Change() => List.Move(_oldIndex, _newIndex);

        protected override void ChangeBack() => List.Move(_newIndex, _oldIndex);
    }
}
