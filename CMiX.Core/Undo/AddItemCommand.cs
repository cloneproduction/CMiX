using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;

namespace CMiX.Core.Prefabs.Messages
{
    public class AddItemCommand : IUndoCommand
    {
        private readonly CollectionManager _collection;
        private readonly ControlRepository _repository;
        private readonly ControlMessenger _messenger;
        private readonly MessageFactory _messageFactory;
        private readonly IControl _item;
        private readonly int _index;

        public AddItemCommand(CollectionManager collection,
                              ControlRepository repository,
                              ControlMessenger messenger,
                              MessageFactory messageFactory,
                              IControl item,
                              int index)
        {
            _collection = collection;
            _repository = repository;
            _messenger = messenger;
            _messageFactory = messageFactory;
            _item = item;
            _index = index;
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
        }
    }

}
