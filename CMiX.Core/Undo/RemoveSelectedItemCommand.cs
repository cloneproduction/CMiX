// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Undo
{
    public class RemoveSelectedItemCommand : IUndoCommand
    {
        private readonly CollectionManager _collection;
        private readonly ControlMessenger _messenger;
        private readonly MessageFactory _messageFactory;
        private readonly IControl _previousItem;
        private readonly int _previousIndex;

        public RemoveSelectedItemCommand(CollectionManager collection,
                                         ControlMessenger messenger,
                                         MessageFactory messageFactory,
                                         IControl previousItem,
                                         int previousIndex)
        {
            _collection = collection;
            _messenger = messenger;
            _messageFactory = messageFactory;
            _previousItem = previousItem;
            _previousIndex = previousIndex;
        }

        public void Execute()
        {
            _collection.RemoveSelectedItem();
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageRemoveSelectedItem>(
                _collection.ManagerData.ID));
        }

        public void Undo()
        {
            _collection.SelectedItemChanged(_previousIndex);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageSelectedItemChanged>(
                _collection.ManagerData.ID, _previousItem, _previousIndex));
        }
    }
}
