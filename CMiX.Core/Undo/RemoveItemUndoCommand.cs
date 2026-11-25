// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    // Owns the removed item while the command is on a stack, and disposes it when the stack drops the command.
    public class RemoveItemUndoCommand : ItemListUndoCommand, IDisposable
    {
        private readonly IControl _item;
        private int _index;

        public RemoveItemUndoCommand(IItemList list, IControl item) : base(list)
        {
            _item = item;
        }

        protected override void Change() => _index = List.Remove(_item);

        protected override void ChangeBack() => List.Insert(_item, _index);

        public void Dispose()
        {
            List.Release(_item);
        }
    }
}
