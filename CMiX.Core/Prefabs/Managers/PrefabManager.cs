// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
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

            IsActive = false;
            activationService.Register(this);
        }

        public CollectionManager Collection { get; set; }
        public ManagerReorderService ManagerReorderService { get; }


        public Guid ID { get; set; }
        public ControlMessenger ControlMessenger { get; set; }
        public MessageFactory MessageFactory { get; set; }
        private MessageCollectionManagerHandler MessageCollectionManagerHandler => Collection.MessageCollectionManagerHandler;
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
            set => Collection.SelectedItem = value;
        }

        private void OnMove(int oldIndex, int newIndex)
        {
            var message = MessageFactory.CreateMessage<MessageMoveItem>(ManagerData.ID, oldIndex, newIndex);
            ControlMessenger.SendMessage(message);
        }

        public void AddItem(Type type)
        {
            ControlMessenger.IsSendingBlocked = true;
            var (prefab, index) = Collection.AddItem(type);
            ControlMessenger.IsSendingBlocked = false;

            var message = prefab is EmptyPrefab ? null :
                MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index);
            if (message != null)
                ControlMessenger.SendMessage(message);
        }

        public void ReplaceItem(IControl control)
        {
            ControlMessenger.IsSendingBlocked = true;
            var (prefab, index, wasReplace) = Collection.ReplaceItem(control);
            ControlMessenger.IsSendingBlocked = false;

            var message = wasReplace
                ? MessageFactory.CreateMessage<MessageReplaceItem>(ManagerData.ID, prefab, index)
                : MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index);
            ControlMessenger.SendMessage(message);
        }

        public void DeleteItem(IControl control)
        {
            ControlMessenger.IsSendingBlocked = true;
            var (removed, newIndex) = Collection.DeleteItem(control);
            ControlMessenger.IsSendingBlocked = false;

            if (removed == null) return;
            var message = MessageFactory.CreateMessage<MessageRemoveItem>(ManagerData.ID, removed, newIndex);
            ControlMessenger.SendMessage(message);
        }

        public void DeleteItem(Guid id) => Collection.DeleteItem(id);

        public void MoveItem(int oldIndex, int newIndex)
        {
            Collection.MoveItem(oldIndex, newIndex);
            var message = MessageFactory.CreateMessage<MessageMoveItem>(ManagerData.ID, oldIndex, newIndex);
            ControlMessenger.SendMessage(message);
        }

        public void RemoveSelectedItem()
        {
            ControlMessenger.IsSendingBlocked = true;
            Collection.RemoveSelectedItem();
            ControlMessenger.IsSendingBlocked = false;
            var message = MessageFactory.CreateMessage<MessageRemoveSelectedItem>(ManagerData.ID);
            ControlMessenger.SendMessage(message);
        }

        public void SelectedItemChanged(IControlModel controlModel, int index) => Collection.SelectedItemChanged(controlModel, index);
        public void SelectedItemChanged(int index) => Collection.SelectedItemChanged(index);
        public void Rename() => Collection.Rename();
        public void SelectedItemIsRenaming() => Collection.SelectedItemIsRenaming();
        public void ResetItem(IControl control) => Collection.ResetItem(control);

        public void Receive(IMessage message)
        {
            if (message is not IMessageManager || ManagerData.ID != message.ID)
                return;
            ReceiveWithoutEcho(() => MessageCollectionManagerHandler.Handle(Collection, message));
        }
    }
}
