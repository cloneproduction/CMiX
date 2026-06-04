// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Undo
{
    public class RemoveItemCommand : IUndoCommand
    {
        private readonly CollectionManager _collection;
        private readonly ControlMessenger _messenger;
        private readonly MessageFactory _messageFactory;
        private readonly IControl _item;
        private readonly int _index;
        private readonly int _newIndex;

        public RemoveItemCommand(CollectionManager collection,
                                 ControlMessenger messenger,
                                 MessageFactory messageFactory,
                                 IControl item,
                                 int index,
                                 int newIndex)
        {
            _collection = collection;
            _messenger = messenger;
            _messageFactory = messageFactory;
            _item = item;
            _index = index;
            _newIndex = newIndex;
        }

        public void Execute()
        {
            _collection.DeleteItem(_item);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageRemoveItem>(
                _collection.ManagerData.ID, _item, _newIndex));
        }

        public void Undo()
        {
            _collection.InsertItem(_item, _index);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageAddItem>(
                _collection.ManagerData.ID, _item, _index));
        }
    }
}
