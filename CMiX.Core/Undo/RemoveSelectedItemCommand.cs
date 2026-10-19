// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Undo
{
    public class RemoveSelectedItemCommand : IUndoCommand
    {
        private readonly CollectionSync _sync;
        private readonly IControl _previousItem;
        private readonly int _previousIndex;

        public RemoveSelectedItemCommand(CollectionSync sync,
                                         IControl previousItem,
                                         int previousIndex)
        {
            _sync = sync;
            _previousItem = previousItem;
            _previousIndex = previousIndex;
        }

        public void Execute()
        {
            _sync.Collection.RemoveSelectedItem();
            _sync.SendRemoveSelected();
        }

        public void Undo()
        {
            _sync.Collection.SelectedItemChanged(_previousIndex);
            _sync.SendSelect(_previousItem, _previousIndex);
        }
    }
}
