using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Undo
{
    // Undo takes the added item back out of the collection, so from that point on this command
    // owns the instance and disposes it when UndoManager drops the command.
    public class AddItemCommand : IUndoCommand, IDisposable
    {
        private readonly CollectionManager _collection;
        private readonly ControlRepository _repository;
        private readonly ControlMessenger _messenger;
        private readonly MessageFactory _messageFactory;
        private readonly IControl _item;
        private readonly int _index;
        private readonly IControl _previousItem;
        private readonly int _previousIndex;

        public AddItemCommand(CollectionManager collection,
                              ControlRepository repository,
                              ControlMessenger messenger,
                              MessageFactory messageFactory,
                              IControl item,
                              int index,
                              IControl previousItem,
                              int previousIndex)
        {
            _collection = collection;
            _repository = repository;
            _messenger = messenger;
            _messageFactory = messageFactory;
            _item = item;
            _index = index;
            _previousItem = previousItem;
            _previousIndex = previousIndex;
        }

        public void Execute()
        {
            _collection.AddItem(_item);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageAddItem>(
                _collection.ManagerData.ID, _item, _index));
        }

        public void Undo()
        {
            _collection.DeleteItem(_item);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageRemoveItem>(
                _collection.ManagerData.ID, _item, _collection.ManagerData.SelectedIndex));

            if (_previousItem != null && _collection.ManagerData.Items.Contains(_previousItem))
            {
                _collection.SelectedItemChanged(_previousIndex);
                _messenger.SendMessage(_messageFactory.CreateMessage<MessageSelectedItemChanged>(
                    _collection.ManagerData.ID, _previousItem, _previousIndex));
            }
        }

        public void Dispose()
        {
            OwnedControl.DisposeIfOrphaned(_repository, _item);
        }
    }
}
