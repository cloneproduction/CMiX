// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class ClearSelectionUndoCommand : IUndoCommand
    {
        private readonly IItemList _list;
        private readonly int _previousIndex;

        public ClearSelectionUndoCommand(IItemList list,
                                         int previousIndex)
        {
            _list = list;
            _previousIndex = previousIndex;
        }

        public void Execute()
        {
            _list.ClearSelection();
        }

        public void Undo()
        {
            _list.SelectAt(_previousIndex);
        }
    }
}
