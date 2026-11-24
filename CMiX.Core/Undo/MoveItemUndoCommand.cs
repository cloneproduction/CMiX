// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class MoveItemUndoCommand : IUndoCommand
    {
        private readonly IItemList _list;
        private readonly int _oldIndex;
        private readonly int _newIndex;

        public MoveItemUndoCommand(IItemList list, int oldIndex, int newIndex)
        {
            _list = list;
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }

        public void Execute()
        {
            _list.Move(_oldIndex, _newIndex);
        }

        public void Undo()
        {
            _list.Move(_newIndex, _oldIndex);
        }
    }
}
