// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Undo
{
    public class SelectItemCommand : IUndoCommand
    {
        private readonly CollectionManager _collection;
        private readonly ControlMessenger _messenger;
        private readonly MessageFactory _messageFactory;
        private readonly IControl _previousItem;
        private readonly int _previousIndex;
        private readonly int _newIndex;

        public SelectItemCommand(CollectionManager collection,
                                 ControlMessenger messenger,
                                 MessageFactory messageFactory,
                                 IControl previousItem,
                                 int previousIndex,
                                 int newIndex)
        {
            _collection = collection;
            _messenger = messenger;
            _messageFactory = messageFactory;
            _previousItem = previousItem;
            _previousIndex = previousIndex;
            _newIndex = newIndex;
        }

        public void Execute()
        {
            if (_newIndex < 0 || _newIndex >= _collection.ManagerData.Items.Count) return;
            _collection.SelectedItemChanged(_newIndex);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageSelectedItemChanged>(
                _collection.ManagerData.ID, _collection.SelectedItem, _newIndex));
        }

        public void Undo()
        {
            if (_previousIndex < 0 || _previousIndex >= _collection.ManagerData.Items.Count) return;
            _collection.SelectedItemChanged(_previousIndex);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageSelectedItemChanged>(
                _collection.ManagerData.ID, _previousItem, _previousIndex));
        }
    }
}
