// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Undo
{
    // Owns the removed item while this command sits on a stack, so the item keeps its nested
    // contents and can be reinserted whole by Undo. UndoManager disposes the command when it
    // drops it, which is the only point where the removal becomes permanent.
    public class RemoveItemCommand : IUndoCommand, IDisposable
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

        public void Dispose()
        {
            OwnedControl.DisposeIfOrphaned(_collection.ControlRepository, _item);
        }
    }
}
