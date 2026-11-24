// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    // Owns the added item after an undo took it out, and disposes it when the stack drops the command.
    public class AddItemUndoCommand : IUndoCommand, IDisposable
    {
        private readonly IItemList _list;
        private readonly IControl _item;
        private readonly int _index;
        private IControl _previousItem;

        public AddItemUndoCommand(IItemList list, IControl item, int index)
        {
            _list = list;
            _item = item;
            _index = index;
        }

        public void Execute()
        {
            _previousItem = _list.SelectedItem;
            _list.Insert(_item, Math.Min(_index, _list.Count));
        }

        public void Undo()
        {
            _list.Remove(_item);

            var index = _list.IndexOf(_previousItem);
            if (index >= 0) _list.SelectAt(index);
        }

        public void Dispose()
        {
            _list.Release(_item);
        }
    }
}
