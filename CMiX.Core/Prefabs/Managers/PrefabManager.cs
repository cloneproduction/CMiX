// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class PrefabManager : ReceivableControl, IControl, IRecipient<IMessage>
    {
        public PrefabManager(CollectionManager collection,
                             ControlMessenger controlMessenger,
                             MessageFactory messageFactory,
                             ControlActivationService activationService,
                             UndoManager undoManager)
        {
            ID = collection.ManagerData.ID;
            ControlMessenger = controlMessenger;
            MessageFactory = messageFactory;
            UndoManager = undoManager;

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

            IsActive = false;
            activationService.Register(this);

        }

        private MessageCollectionManagerHandler MessageCollectionManagerHandler => Collection.MessageCollectionManagerHandler;

        public Guid ID { get; set; }
        public CollectionManager Collection { get; set; }
        public ManagerReorderService ManagerReorderService { get; }
        public ControlRepository ControlRepository => Collection.ControlRepository;
        public ControlMessenger ControlMessenger { get; set; }
        public MessageFactory MessageFactory { get; set; }

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
                if (value == null)
                {
                    Collection.RemoveSelectedItem();
                    return;
                }

                if (UndoManager?.IsApplying == true) return;

                if (!Collection.ManagerData.Items.Contains(value))
                    EnsureItemInCollection(value);

                SelectedItemChanged(Collection.ManagerData.Items.IndexOf(value));
            }
        }

        private void EnsureItemInCollection(IControl control)
        {
            if (Collection.ManagerData.Items.Contains(control)) return;

            Collection.AddItem(control);
            var index = Collection.ManagerData.Items.IndexOf(control);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, control, index));
            UndoManager?.Push(new AddItemCommand(Collection, ControlMessenger, MessageFactory, control, index));
        }

        public void ClearAll()
        {
            var items = Collection.ManagerData.Items.ToList();
            Collection.ClearAll();
            foreach (var item in items)
                ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageRemoveItem>(ManagerData.ID, item, -1));
            UndoManager?.Clear();
        }

        public void AddItem(Type type)
        {
            UndoManager?.BeginGroup();
            var (prefab, index) = Collection.AddItem(type);

            if (prefab is EmptyPrefab)
            {
                UndoManager?.EndGroup();
                return;
            }

            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index));
            UndoManager?.EndGroup();
            UndoManager?.Push(new AddItemCommand(Collection, ControlMessenger, MessageFactory, prefab, index));
        }

        public void AddItem(IControlModel controlModel)
        {
            Collection.AddItem(controlModel);
            var prefab = Collection.SelectedItem;
            if (prefab is EmptyPrefab) return;

            var index = Collection.ManagerData.Items.IndexOf(prefab);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index));
            UndoManager?.Push(new AddItemCommand(Collection, ControlMessenger, MessageFactory, prefab, index));
        }

        public void ReplaceItem(IControl control)
        {
            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.SelectedIndex;
            var (prefab, index, wasReplace) = Collection.ReplaceItem(control);
            var message = wasReplace
                ? MessageFactory.CreateMessage<MessageReplaceItem>(ManagerData.ID, prefab, index)
                : MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index);
            ControlMessenger.SendMessage(message);
            UndoManager?.Push(new ReplaceItemCommand(Collection, ControlMessenger, MessageFactory, previousItem, previousIndex, prefab, index));
        }

        public void DeleteItem(IControl control)
        {
            var index = Collection.ManagerData.Items.IndexOf(control);
            var (removed, newIndex) = Collection.DeleteItem(control);
            if (removed == null) return;
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageRemoveItem>(ManagerData.ID, removed, newIndex));
            UndoManager?.Push(new RemoveItemCommand(Collection, ControlMessenger, MessageFactory, control, index, newIndex));
        }

        public void DeleteItem(Guid id) => Collection.DeleteItem(id);

        public void MoveItem(int oldIndex, int newIndex)
        {
            Collection.MoveItem(oldIndex, newIndex);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageMoveItem>(ManagerData.ID, oldIndex, newIndex));
            UndoManager?.Push(new MoveItemCommand(Collection, ControlMessenger, MessageFactory, oldIndex, newIndex));
        }

        private void OnMove(int oldIndex, int newIndex)
        {
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageMoveItem>(ManagerData.ID, oldIndex, newIndex));
            UndoManager?.Push(new MoveItemCommand(Collection, ControlMessenger, MessageFactory, oldIndex, newIndex));
        }

        public void RemoveSelectedItem()
        {
            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.Items.IndexOf(previousItem);
            Collection.RemoveSelectedItem();
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageRemoveSelectedItem>(ManagerData.ID));
            UndoManager?.Push(new RemoveSelectedItemCommand(Collection, ControlMessenger, MessageFactory, previousItem, previousIndex));
        }

        private bool ShouldRecordSelectionUndo(IControl previousItem) =>
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
            UndoManager?.Push(new SelectItemCommand(Collection, ControlMessenger, MessageFactory, previousItem, previousIndex, index));
        }

        public void LoadItem(IControlModel controlModel)
        {
            Collection.AddItem(controlModel);
        }

        public void ResetItem(IControl control) => Collection.ResetItem(control);

        public void Receive(IMessage message)
        {
            if (message is not IMessageManager || ManagerData.ID != message.ID)
                return;
            if (ControlMessenger.IsReceivingBlocked) return;
            ReceiveWithoutEcho(() => MessageCollectionManagerHandler.Handle(Collection, message));
        }

        public IControlModel ToModel() => new PrefabManagerModel
        {
            ID = ManagerData.ID,
            ManagerData = new ManagerDataModel
            {
                ID = ManagerData.ID,
                SelectedIndex = ManagerData.SelectedIndex,
                Items = new Collection<IControlModel>(
                    ManagerData.Items
                        .Select(c => c.ToModel())
                        .ToList()
                )
            }
        };

        public void FromModel(IControlModel model)
        {
            var m = (PrefabManagerModel)model;
            ManagerData.ID = m.ManagerData.ID;
            ManagerData.SelectedIndex = m.ManagerData.SelectedIndex;
        }
    }
}
