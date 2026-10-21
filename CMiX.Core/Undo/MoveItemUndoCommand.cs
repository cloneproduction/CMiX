// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Undo
{
    public class MoveItemUndoCommand : IUndoCommand
    {
        private readonly CollectionSync _sync;
        private readonly int _oldIndex;
        private readonly int _newIndex;

        public MoveItemUndoCommand(CollectionSync sync, int oldIndex, int newIndex)
        {
            _sync = sync;
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }

        public void Execute()
        {
            _sync.Collection.ManagerData.Move(_oldIndex, _newIndex);
            _sync.SendMove(_oldIndex, _newIndex);
        }

        public void Undo()
        {
            _sync.Collection.ManagerData.Move(_newIndex, _oldIndex);
            _sync.SendMove(_newIndex, _oldIndex);
        }
    }
}
