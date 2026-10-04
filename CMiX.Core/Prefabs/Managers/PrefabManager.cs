using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class PrefabManager : PrefabManagerBase, IDisposable
    {
        public PrefabManager(CollectionManager collection,
                             ControlMessenger controlMessenger,
                             MessageFactory messageFactory,
                             ControlActivationService activationService,
                             UndoManager undoManager,
                             ManagerReorderServiceFactory reorderServiceFactory = null)
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
            Collection.AddExistingItemCommand = new RelayCommand<IControl>(AddExistingItem);
            Collection.DeleteItemCommand = new RelayCommand<IControl>(DeleteItem);
            Collection.RemoveSelectedItemCommand = new RelayCommand(RemoveSelectedItem);
            Collection.ResetItemCommand = new RelayCommand<IControl>(ResetItem);
            ManagerReorderService = reorderServiceFactory?.Invoke(Collection, OnMove);
            DeleteEverywhereCommand = new RelayCommand<IControl>(DeleteEverywhere);

            // Registered under the constructor ID; RegisterDeleter runs again when the real ID is set.
            RegisterDeleter();
        }

        private MessageCollectionManagerHandler MessageCollectionManagerHandler => Collection.MessageCollectionManagerHandler;

        public CollectionManager Collection { get; set; }
        public IManagerReorderService ManagerReorderService { get; }

        [ObservableProperty]
        private bool isExpanded = false;

        // Suppresses SelectItemCommand while AddItem changes the selection.
        private bool _isAdding = false;

        // Makes the setter a no-op while it runs, so a binding cannot recurse it into a stack overflow.
        private bool _isApplyingSelection = false;

        // The ID the deleter is registered under; ManagerData.ID changes after construction.
        private Guid _registeredDeleterId;
        private bool _hasRegisteredDeleter;

        public ManagerData ManagerData => Collection.ManagerData;

        public override ICommand AddItemCommand => Collection.AddItemCommand;
        public ICommand AddExistingItemCommand => Collection.AddExistingItemCommand;
        public ICommand DeleteItemCommand => Collection.DeleteItemCommand;
        public override ICommand RemoveSelectedItemCommand => Collection.RemoveSelectedItemCommand;
        public ICommand ResetItemCommand => Collection.ResetItemCommand;
        public ICommand DeleteEverywhereCommand { get; }

        public override IControl SelectedItem
        {
            get => Collection.SelectedItem;
            set
            {
                if (_isApplyingSelection) return;
                _isApplyingSelection = true;
                try
                {
                    if (value == null) { Collection.RemoveSelectedItem(); return; }
                    if (UndoManager?.IsApplying == true) return;
                    EnsureItemInCollection(value);
                    if (Collection.ManagerData.Items.Contains(value))
                        SelectedItemChanged(Collection.ManagerData.Items.IndexOf(value));
                }
                finally { _isApplyingSelection = false; }
            }
        }

        private (IControl prefab, int index) CreateItem(Type type)
        {
            _isAdding = true;
            try { return Collection.AddItem(type); }
            finally { _isAdding = false; }
        }

        private void AddItemInternal(IControl prefab, int index, IControl previousItem, int previousIndex)
        {
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index));
            UndoManager?.Push(new AddItemCommand(Collection, ControlRepository, ControlMessenger, MessageFactory, prefab, index, previousItem, previousIndex));
        }

        private void EnsureItemInCollection(IControl control)
        {
            if (Collection.ManagerData.Items.Contains(control)) return;
            if (UndoManager?.IsApplying == true) return;

            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.Items.IndexOf(previousItem);
            Collection.AddItem(control);
            var index = Collection.ManagerData.Items.IndexOf(control);
            AddItemInternal(control, index, previousItem, previousIndex);
        }

        public void ClearAll()
        {
            var items = Collection.ManagerData.Items.ToList();
            Collection.ClearAll();
            foreach (var item in items)
                ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageRemoveItem>(ManagerData.ID, item, -1));
        }

        public void AddItem(Type type)
        {
            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.Items.IndexOf(previousItem);
            var (prefab, index) = CreateItem(type);
            if (prefab == null) return;
            AddItemInternal(prefab, index, previousItem, previousIndex);
        }

        public void AddItem(IControlModel model)
        {
            Collection.AddItem(model);
            var prefab = Collection.SelectedItem;
            var index = Collection.ManagerData.Items.IndexOf(prefab);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab, index));
        }

        public void AddItemFromFilePath(Type type, string filePath)
        {
            UndoManager?.BeginCapture();
            try
            {
                var previousItem = Collection.SelectedItem;
                var previousIndex = Collection.ManagerData.Items.IndexOf(previousItem);
                var (prefab, index) = CreateItem(type);
                if (prefab == null) return;
                if (prefab is IAssetTextureSource source)
                    source.AssetSelector.SetAssetFromPath(filePath);
                AddItemInternal(prefab, index, previousItem, previousIndex);
            }
            finally { UndoManager?.EndCapture(); }
        }

        public void AddExistingItem(IControl control)
        {
            EnsureItemInCollection(control);
            SelectedItemChanged(Collection.ManagerData.Items.IndexOf(control));
        }

        public void DeleteItem(IControl control)
        {
            var index = Collection.ManagerData.Items.IndexOf(control);
            var (removed, newIndex) = Collection.DeleteItem(control);
            if (removed == null) return;
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageRemoveItem>(ManagerData.ID, removed, newIndex));
            // newIndex is -1 after the last item is deleted; the undo entry is still pushed.
            UndoManager?.Push(new RemoveItemCommand(Collection, ControlMessenger, MessageFactory, control, index, newIndex));
        }

        // Swaps in a fresh instance at the same position as one undoable remove and add.
        public void ResetItem(IControl control)
        {
            if (control == null) return;
            if (UndoManager?.IsApplying == true) return;

            var index = Collection.ManagerData.Items.IndexOf(control);
            if (index < 0) return;

            var replacement = Collection.ControlFactory.Create(control.GetType());
            if (replacement == null) return;

            var command = new ReplaceItemCommand(Collection, ControlMessenger, MessageFactory, control, replacement, index);
            command.Execute();
            UndoManager?.Push(command);
        }

        // Registers DeleteItem under the current ManagerData.ID, replacing any entry under the previous ID.
        public void RegisterDeleter()
        {
            UnregisterDeleter();
            _registeredDeleterId = ManagerData.ID;
            _hasRegisteredDeleter = true;
            Collection.ControlRepository.RegisterDeleter(_registeredDeleterId, DeleteItem);
        }

        public void UnregisterDeleter()
        {
            if (!_hasRegisteredDeleter) return;
            _hasRegisteredDeleter = false;
            Collection.ControlRepository.UnregisterDeleter(_registeredDeleterId, DeleteItem);
        }

        // Ends the manager's lifetime; called from the Dispose of the owning control.
        public void Dispose()
        {
            ClearAll();
            UnregisterDeleter();
        }

        private void DeleteEverywhere(IControl control)
        {
            UndoManager?.BeginCapture();
            try
            {
                Collection.ControlRepository.DeleteEverywhere(control ?? SelectedItem);
            }
            finally
            {
                UndoManager?.EndCapture();
            }
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
            if (previousIndex < 0) return;
            UndoManager?.Push(new RemoveSelectedItemCommand(Collection, ControlMessenger, MessageFactory, previousItem, previousIndex));
        }

        public void SelectedItemChanged(int index)
        {
            var previousItem = Collection.SelectedItem;
            var previousIndex = Collection.ManagerData.Items.IndexOf(previousItem);
            Collection.SelectedItemChanged(index);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageSelectedItemChanged>(ManagerData.ID, Collection.SelectedItem, index));
            if (UndoManager?.IsApplying ?? false) return;
            if (_isAdding) return;
            if (previousItem == null || previousItem == Collection.SelectedItem) return;
            UndoManager?.Push(new SelectItemCommand(Collection, ControlMessenger, MessageFactory, previousItem, previousIndex, index));
        }

        public void LoadItem(IControlModel controlModel) => Collection.LoadItem(controlModel);

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
            // ControlFactory.Create also sets the ID through FromModel, so the deleter is registered again.
            RegisterDeleter();
        }
    }
}
