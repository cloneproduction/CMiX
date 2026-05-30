//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using CMiX.Core.Networking;
//using CMiX.Core.Networking.Messages;
//using CMiX.Core.Prefabs.Managers;
//using CMiX.Core.Prefabs.Messages;

//namespace CMiX.Core.Undo
//{
//    public class ReplaceItemCommand : IUndoCommand
//    {
//        private readonly CollectionManager _collection;
//        private readonly ControlMessenger _messenger;
//        private readonly MessageFactory _messageFactory;
//        private readonly IControl _previousItem;
//        private readonly int _previousIndex;
//        private readonly IControl _newItem;
//        private readonly int _newIndex;

//        public ReplaceItemCommand(CollectionManager collection,
//                                  ControlMessenger messenger,
//                                  MessageFactory messageFactory,
//                                  IControl previousItem,
//                                  int previousIndex,
//                                  IControl newItem,
//                                  int newIndex)
//        {
//            _collection = collection;
//            _messenger = messenger;
//            _messageFactory = messageFactory;
//            _previousItem = previousItem;
//            _previousIndex = previousIndex;
//            _newItem = newItem;
//            _newIndex = newIndex;
//        }

//        public void Execute()
//        {
//            _collection.ReplaceItem(_newItem, _newIndex);
//            _messenger.SendMessage(_messageFactory.CreateMessage<MessageReplaceItem>(
//                _collection.ManagerData.ID, _newItem, _newIndex));
//        }

//        public void Undo()
//        {
//            _collection.ReplaceItem(_previousItem, _previousIndex);
//            _messenger.SendMessage(_messageFactory.CreateMessage<MessageReplaceItem>(
//                _collection.ManagerData.ID, _previousItem, _previousIndex));
//        }
//    }
//}
