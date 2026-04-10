// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Prefabs.Managers
{
    public class PrefabManagerUndoSteps
    {
        private readonly CollectionManager _collection;
        private readonly ControlMessenger _controlMessenger;
        private readonly MessageFactory _messageFactory;
        private readonly Action<bool> _setSuppressSelectionUndo;

        public PrefabManagerUndoSteps(
            CollectionManager collection,
            ControlMessenger controlMessenger,
            MessageFactory messageFactory,
            Action<bool> setSuppressSelectionUndo)
        {
            _collection = collection;
            _controlMessenger = controlMessenger;
            _messageFactory = messageFactory;
            _setSuppressSelectionUndo = setSuppressSelectionUndo;
        }

        private void Execute(Action collectionAction, Func<IMessage> createMessage)
        {
            _setSuppressSelectionUndo(true);
            _controlMessenger.IsSendingBlocked = true;
            collectionAction();
            _controlMessenger.IsSendingBlocked = false;
            _setSuppressSelectionUndo(false);
            _controlMessenger.SendMessage(createMessage());
        }

        private Guid ManagerID => _collection.ManagerData.ID;

        private IMessage AddMsg(IControl control, int index) =>
            _messageFactory.CreateMessage<MessageAddItem>(ManagerID, control, index);

        private IMessage RemoveMsg(IControl control, int index) =>
            _messageFactory.CreateMessage<MessageRemoveItem>(ManagerID, control, index);

        private IMessage MoveMsg(int oldIndex, int newIndex) =>
            _messageFactory.CreateMessage<MessageMoveItem>(ManagerID, oldIndex, newIndex);

        private IMessage ReplaceMsg(IControl control, int index) =>
            _messageFactory.CreateMessage<MessageReplaceItem>(ManagerID, control, index);

        private IMessage SelectMsg(IControl control, int index) =>
            _messageFactory.CreateMessage<MessageSelectedItemChanged>(ManagerID, control, index);

        private IMessage RemoveSelectedMsg() =>
            _messageFactory.CreateMessage<MessageRemoveSelectedItem>(ManagerID);



        public UndoStep AddItem(IControl prefab, int index) => new UndoStep(
            undoAction: () => Execute(
                () => _collection.DeleteItem(prefab),
                () => RemoveMsg(prefab, _collection.ManagerData.SelectedIndex)),
            redoAction: () => Execute(
                () =>
                {
                    _collection.ManagerData.Items.Add(prefab); 
                    _collection.SelectedItem = prefab;
                    _collection.ManagerData.SelectedIndex = index; 
                    _collection.ControlRepository.AddControl(prefab);
                },
                () => AddMsg(prefab, index))
        );

        public UndoStep DeleteItem(IControl control, int index, int newIndex) => new UndoStep(
            undoAction: () => Execute(
                () =>
                {
                    _collection.ManagerData.Items.Insert(index, control); 
                    _collection.SelectedItem = control;
                    _collection.ManagerData.SelectedIndex = index; 
                    _collection.ControlRepository.AddControl(control);
                },
                () => AddMsg(control, index)),
            redoAction: () => Execute(
                () => _collection.DeleteItem(control),
                () => RemoveMsg(control, newIndex))
        );

        public UndoStep MoveItem(int oldIndex, int newIndex) => new UndoStep(
            undoAction: () => Execute(
                () => _collection.MoveItem(newIndex, oldIndex),
                () => MoveMsg(newIndex, oldIndex)),
            redoAction: () => Execute(
                () => _collection.MoveItem(oldIndex, newIndex),
                () => MoveMsg(oldIndex, newIndex))
        );

        public UndoStep ReplaceItem(IControl previousItem, int previousIndex, IControl prefab, int index) => new UndoStep(
            undoAction: () => Execute(
                () => _collection.ReplaceItem(previousItem),
                () => ReplaceMsg(previousItem, previousIndex)),
            redoAction: () => Execute(
                () => _collection.ReplaceItem(prefab),
                () => ReplaceMsg(prefab, index))
        );

        public UndoStep SelectedItemChanged(IControl previousItem, int previousIndex, int index) => new UndoStep(
            undoAction: () => Execute(
                () => _collection.SelectedItemChanged(previousIndex),
                () => SelectMsg(previousItem, previousIndex)),
            redoAction: () => Execute(
                () => _collection.SelectedItemChanged(index),
                () => SelectMsg(_collection.SelectedItem, index))
        );

        public UndoStep RemoveSelectedItem(IControl previousItem, int previousIndex) => new UndoStep(
            undoAction: () => Execute(
                () => _collection.SelectedItemChanged(previousIndex),
                () => SelectMsg(previousItem, previousIndex)),
            redoAction: () => Execute(
                () => _collection.RemoveSelectedItem(),
                () => RemoveSelectedMsg())
        );
    }
}
