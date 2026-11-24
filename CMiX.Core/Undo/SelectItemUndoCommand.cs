// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class SelectItemUndoCommand : IUndoCommand
    {
        private readonly IItemList _list;
        private readonly int _previousIndex;
        private readonly int _newIndex;

        public SelectItemUndoCommand(IItemList list,
                                 IControl previousItem,
                                 int previousIndex,
                                 int newIndex)
        {
            _list = list;
            _previousIndex = previousIndex;
            _newIndex = newIndex;
        }

        public void Execute()
        {
            if (_newIndex < 0 || _newIndex >= _list.Count) return;
            _list.SelectAt(_newIndex);
        }

        public void Undo()
        {
            if (_previousIndex < 0 || _previousIndex >= _list.Count) return;
            _list.SelectAt(_previousIndex);
        }
    }
}
