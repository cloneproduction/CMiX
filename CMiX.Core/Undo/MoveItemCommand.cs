// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Undo
{
    public class MoveItemCommand : IUndoCommand
    {
        private readonly CollectionManager _collection;
        private readonly ControlMessenger _messenger;
        private readonly MessageFactory _messageFactory;
        private readonly int _oldIndex;
        private readonly int _newIndex;

        public MoveItemCommand(CollectionManager collection,
                               ControlMessenger messenger,
                               MessageFactory messageFactory,
                               int oldIndex,
                               int newIndex)
        {
            _collection = collection;
            _messenger = messenger;
            _messageFactory = messageFactory;
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }

        public void Execute()
        {
            _collection.MoveItem(_oldIndex, _newIndex);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageMoveItem>(
                _collection.ManagerData.ID, _oldIndex, _newIndex));
        }

        public void Undo()
        {
            _collection.MoveItem(_newIndex, _oldIndex);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageMoveItem>(
                _collection.ManagerData.ID, _newIndex, _oldIndex));
        }
    }
}
