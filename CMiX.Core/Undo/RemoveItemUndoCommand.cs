// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    // Owns the removed item while the command is on a stack, and disposes it when the stack drops the command.
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
            var index = _list.IndexOf(_selectedBefore);
            if (index >= 0) _list.SelectAt(index);
        }

        public void Dispose()
        {
            _list.Release(_item);
        }
    }
}
