// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class PrefabManager : ReceivableControl, IControl, IRecipient<IMessage>
    {
        public PrefabManager(CollectionManager collection,
                             ControlMessenger controlMessenger,
                             MessageFactory messageFactory,
                             ControlActivationService activationService)
        {
            ID = collection.ManagerData.ID;
            ControlMessenger = controlMessenger;
            MessageFactory = messageFactory;

            Collection = collection;
            Collection.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CollectionManager.SelectedItem))
                    OnPropertyChanged(nameof(SelectedItem));
            };

            Collection.AddItemCommand = new RelayCommand<Type>(AddItem);
            Collection.DeleteItemCommand = new RelayCommand<IControl>(DeleteItem);
            Collection.ReplaceSelectedItemCommand = new RelayCommand<IControl>(ReplaceItem);
            Collection.RemoveSelectedItemCommand = new RelayCommand(RemoveSelectedItem);
            ManagerReorderService = new ManagerReorderService(Collection, OnMove);
            _undoSteps = new PrefabManagerUndoSteps(Collection, controlMessenger, messageFactory, v => _suppressSelectionUndo = v);

            IsActive = false;
            activationService.Register(this);
        }

        private PrefabManagerUndoSteps _undoSteps;
        private MessageCollectionManagerHandler MessageCollectionManagerHandler => Collection.MessageCollectionManagerHandler;

        public Guid ID { get; set; }
        public CollectionManager Collection { get; set; }
        public ManagerReorderService ManagerReorderService { get; }
        public ControlRepository ControlRepository => Collection.ControlRepository;
        public ControlMessenger ControlMessenger { get; set; }
        public MessageFactory MessageFactory { get; set; }

        // Setter required for Mapster: maps PrefabManagerModel.ManagerData → Collection.ManagerData.ID
        // to get rid of this is to put everything that is in managerdata directly in CollectionManager and update all xaml binding
        public ManagerData ManagerData
        {
            get => Collection.ManagerData;
            set => Collection.ManagerData.ID = value.ID;
        }

        public ICommand AddItemCommand => Collection.AddItemCommand;
        public ICommand DeleteItemCommand => Collection.DeleteItemCommand;
        public ICommand RemoveSelectedItemCommand => Collection.RemoveSelectedItemCommand;
        public ICommand ReplaceSelectedItemCommand => Collection.ReplaceSelectedItemCommand;
        public ICommand ResetItemCommand => Collection.ResetItemCommand;

        public IControl SelectedItem
        {
            get => Collection.SelectedItem;
            set
            {
                var index = Collection.ManagerData.Items.IndexOf(value);
                SelectedItemChanged(index);
            }
        }

        public void AddItem(Type type)
        {
            _suppressSelectionUndo = true;
            var (prefab, index) = Collection.AddItem(type);
            _suppressSelectionUndo = false;

            if (prefab is EmptyPrefab) return;

            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index));
            UndoManager?.Push(_undoSteps.AddItem(prefab, index));
        }


        public void ReplaceItem(IControl control)
        {

            var (prefab, index, wasReplace) = Collection.ReplaceItem(control);
            var message = wasReplace
                ? MessageFactory.CreateMessage<MessageReplaceItem>(ManagerData.ID, prefab, index)
                : MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index);
            ControlMessenger.SendMessage(message);

            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.SelectedIndex;
            UndoManager?.Push(_undoSteps.ReplaceItem(previousItem, previousIndex, prefab, index));
        }

        public void DeleteItem(IControl control)
        {
            var (removed, newIndex) = Collection.DeleteItem(control);
            if (removed == null) return;
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageRemoveItem>(ManagerData.ID, removed, newIndex));

            var index = Collection.ManagerData.Items.IndexOf(control);
            UndoManager?.Push(_undoSteps.DeleteItem(control, index, newIndex));
        }

        public void DeleteItem(Guid id) => Collection.DeleteItem(id);

        public void MoveItem(int oldIndex, int newIndex)
        {
            Collection.MoveItem(oldIndex, newIndex);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageMoveItem>(ManagerData.ID, oldIndex, newIndex));
            UndoManager?.Push(_undoSteps.MoveItem(oldIndex, newIndex));
        }

        private void OnMove(int oldIndex, int newIndex)
        {
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageMoveItem>(ManagerData.ID, oldIndex, newIndex));
            UndoManager?.Push(_undoSteps.MoveItem(oldIndex, newIndex));
        }

        // Capture selection state before removing — previousItem/previousIndex must be saved
        // before calling RemoveSelectedItem() as it clears the selection.
        public void RemoveSelectedItem()
        {
            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.Items.IndexOf(previousItem);
            Collection.RemoveSelectedItem();
            var message = MessageFactory.CreateMessage<MessageRemoveSelectedItem>(ManagerData.ID);
            ControlMessenger.SendMessage(message);
            UndoManager?.Push(_undoSteps.RemoveSelectedItem(previousItem, previousIndex));
        }

        private bool _suppressSelectionUndo = false;


        private bool ShouldRecordSelectionUndo(IControl previousItem) =>
            !_suppressSelectionUndo &&
            !(UndoManager?.IsApplying ?? false) &&
            previousItem != null &&
            previousItem != Collection.SelectedItem;

        public void SelectedItemChanged(Guid controlID, int index)
        {
            Collection.SelectedItemChanged(controlID, index);
        }

        public void SelectedItemChanged(int index)
        {
            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.Items.IndexOf(previousItem);
            Collection.SelectedItemChanged(index);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageSelectedItemChanged>(ManagerData.ID, Collection.SelectedItem, index));
            if (!ShouldRecordSelectionUndo(previousItem)) return;
            UndoManager?.Push(_undoSteps.SelectedItemChanged(previousItem, previousIndex, index));
        }


        //public void Rename() => Collection.Rename();
        //public void SelectedItemIsRenaming() => Collection.SelectedItemIsRenaming();
        public void ResetItem(IControl control) => Collection.ResetItem(control);

        public void Receive(IMessage message)
        {
            if (message is not IMessageManager || ManagerData.ID != message.ID)
                return;
            ReceiveWithoutEcho(() => MessageCollectionManagerHandler.Handle(Collection, message));
        }
    }
}
