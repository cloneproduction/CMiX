// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Undo
{
    // Owns the removed item while this command sits on a stack, so the item keeps its nested
    // contents and can be reinserted whole by Undo. UndoManager disposes the command when it
    // drops it, which is the only point where the removal becomes permanent.
    public class RemoveItemUndoCommand : IUndoCommand, IDisposable
    {
        private readonly CollectionSync _sync;
        private readonly IControl _item;
        private int _index;
        private IControl _selectedBefore;

        public RemoveItemUndoCommand(CollectionSync sync, IControl item)
        {
            _sync = sync;
            _item = item;
        }

        public void Execute()
        {
            _selectedBefore = _sync.Collection.ManagerData.SelectedItem;
            _index = _sync.Collection.DeleteItem(_item);
            _sync.SendRemove(_item, _sync.Collection.ManagerData.SelectedIndex);
        }

        public void Undo()
        {
            var data = _sync.Collection.ManagerData;
            _sync.Collection.InsertItem(_item, _index);
            _sync.SendAdd(_item, _index);

            if (ReferenceEquals(_selectedBefore, _item)) return;
            data.Select(_selectedBefore);
            _sync.SendSelect(_selectedBefore, data.SelectedIndex);
        }

        public void Dispose()
        {
            OwnedControl.DisposeIfOrphaned(_sync.Collection.ControlRepository, _item);
        }
    }
}
