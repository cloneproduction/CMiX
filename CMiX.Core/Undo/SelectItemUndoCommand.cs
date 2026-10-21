// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Undo
{
    public class SelectItemUndoCommand : IUndoCommand
    {
        private readonly CollectionSync _sync;
        private readonly IControl _previousItem;
        private readonly int _previousIndex;
        private readonly int _newIndex;

        public SelectItemUndoCommand(CollectionSync sync,
                                 IControl previousItem,
                                 int previousIndex,
                                 int newIndex)
        {
            _sync = sync;
            _previousItem = previousItem;
            _previousIndex = previousIndex;
            _newIndex = newIndex;
        }

        public void Execute()
        {
            var collection = _sync.Collection;
            if (_newIndex < 0 || _newIndex >= collection.ManagerData.Items.Count) return;
            collection.ManagerData.SelectAt(_newIndex);
            _sync.SendSelect(collection.ManagerData.SelectedItem, _newIndex);
        }

        public void Undo()
        {
            var collection = _sync.Collection;
            if (_previousIndex < 0 || _previousIndex >= collection.ManagerData.Items.Count) return;
            collection.ManagerData.SelectAt(_previousIndex);
            _sync.SendSelect(_previousItem, _previousIndex);
        }
    }
}
