// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    // Owns the removed item while this command sits on a stack, so the item keeps its nested
    // contents and can be reinserted whole by Undo. UndoManager disposes the command when it
    // drops it, which is the only point where the removal becomes permanent.
    public class RemoveItemUndoCommand : IUndoCommand, IDisposable
    {
        private readonly IItemList _list;
        private readonly IControl _item;
        private int _index;
        private IControl _selectedBefore;

        public RemoveItemUndoCommand(IItemList list, IControl item)
        {
            _list = list;
            _item = item;
        }

        public void Execute()
        {
            _selectedBefore = _list.SelectedItem;
            _index = _list.Remove(_item);
        }

        public void Undo()
        {
            _list.Insert(_item, _index);

            if (ReferenceEquals(_selectedBefore, _item)) return;
            _list.SelectAt(_list.IndexOf(_selectedBefore));
        }

        public void Dispose()
        {
            _list.Release(_item);
        }
    }
}
