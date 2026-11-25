// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    // Owns the added item after an undo took it out, and disposes it when the stack drops the command.
    public class AddItemUndoCommand : ItemListUndoCommand, IDisposable
    {
        private readonly IControl _item;
        private readonly int _index;

        public AddItemUndoCommand(IItemList list, IControl item, int index) : base(list)
        {
            _item = item;
            _index = index;
        }

        protected override void Change() => List.Insert(_item, Math.Min(_index, List.Count));

        protected override void ChangeBack() => List.Remove(_item);

        public void Dispose()
        {
            List.Release(_item);
        }
    }
}
