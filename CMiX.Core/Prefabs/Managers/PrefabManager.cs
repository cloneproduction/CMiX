using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class PrefabManager : PrefabManagerBase
    {
        public PrefabManager(CollectionManager collection,
                             ControlMessenger controlMessenger,
                             MessageFactory messageFactory,
                             ControlActivationService activationService,
                             UndoManager undoManager,
                             ManagerReorderServiceFactory? reorderServiceFactory = null)
            : base(collection.ControlRepository, controlMessenger, messageFactory, activationService, undoManager)
        {
            ID = collection.ManagerData.ID;

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
            ManagerReorderService = reorderServiceFactory?.Invoke(Collection, OnMove);
        }

        private MessageCollectionManagerHandler MessageCollectionManagerHandler => Collection.MessageCollectionManagerHandler;

        public CollectionManager Collection { get; set; }
        public IManagerReorderService? ManagerReorderService { get; }

        [ObservableProperty]
        private bool isExpanded = false;

        public ManagerData ManagerData
        {
            get => Collection.ManagerData;
            set => Collection.ManagerData.ID = value.ID;
        }

        public override ICommand AddItemCommand => Collection.AddItemCommand;
        public ICommand DeleteItemCommand => Collection.DeleteItemCommand;
        public override ICommand RemoveSelectedItemCommand => Collection.RemoveSelectedItemCommand;
        public ICommand ReplaceSelectedItemCommand => Collection.ReplaceSelectedItemCommand;
        public ICommand ResetItemCommand => Collection.ResetItemCommand;

        public override IControl SelectedItem
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
            var (prefab, index) = Collection.AddItem(type);

            if (prefab is EmptyPrefab)
                return;

            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index));
            UndoManager?.Push(new AddItemCommand(Collection, ControlMessenger, MessageFactory, prefab, index));
        }

        public void ReplaceItem(IControl control)
        {
            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.SelectedIndex;
            var (prefab, index, wasReplace) = Collection.ReplaceItem(control);

            bool existsInRepo = ControlRepository.Controls.Any(x => x.ID == prefab.ID);

            var message = (wasReplace || existsInRepo)
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
            OnMove(oldIndex, newIndex);
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

        public void SelectedItemChanged(Guid controlID, int index)
            => Collection.SelectedItemChanged(controlID, index);

        public void SelectedItemChanged(int index)
        {
            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.Items.IndexOf(previousItem);
            Collection.SelectedItemChanged(index);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageSelectedItemChanged>(ManagerData.ID, Collection.SelectedItem, index));
            if (UndoManager?.IsApplying ?? false) return;
            if (previousItem == null || previousItem == Collection.SelectedItem) return;
            UndoManager?.Push(new SelectItemCommand(Collection, ControlMessenger, MessageFactory, previousItem, previousIndex, index));
        }

        public void LoadItem(IControlModel controlModel) => Collection.LoadItem(controlModel);

        public void ResetItem(IControl control) => Collection.ResetItem(control);

        public override void Receive(IMessage message)
        {
            if (message is not IMessageManager || ManagerData.ID != message.ID)
                return;
            ReceiveWithoutEcho(() => MessageCollectionManagerHandler.Handle(Collection, message));
        }

        public override IControlModel ToModel() => new PrefabManagerModel
        {
            ID = ManagerData.ID,
            ManagerData = new ManagerDataModel
            {
                ID = ManagerData.ID,
                SelectedIndex = ManagerData.SelectedIndex,
                Items = new Collection<IControlModel>(
                    ManagerData.Items
                        .Where(c => Collection.ControlRepository.HasUsers(c))
                        .Select(c => c.ToModel())
                        .ToList()
                )
            }
        };

        public override void FromModel(IControlModel model)
        {
            var m = (PrefabManagerModel)model;
            ManagerData.ID = m.ManagerData.ID;
            ManagerData.SelectedIndex = m.ManagerData.SelectedIndex;
        }
    }
}
