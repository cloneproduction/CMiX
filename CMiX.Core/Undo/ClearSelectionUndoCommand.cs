// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Undo
{
    public class ClearSelectionUndoCommand : IUndoCommand
    {
        private readonly CollectionSync _sync;
        private readonly IControl _previousItem;
        private readonly int _previousIndex;

        public ClearSelectionUndoCommand(CollectionSync sync,
                                         IControl previousItem,
                                         int previousIndex)
        {
            _sync = sync;
            _previousItem = previousItem;
            _previousIndex = previousIndex;
        }

        public void Execute()
        {
            _sync.Collection.ManagerData.ClearSelection();
            _sync.SendClearSelection();
        }

        public void Undo()
        {
            _sync.Collection.ManagerData.SelectAt(_previousIndex);
            _sync.SendSelect(_previousItem, _previousIndex);
        }
    }
}
