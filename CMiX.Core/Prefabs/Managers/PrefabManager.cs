using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
            Collection.ManagerData.Items.CollectionChanged += OnItemsChanged;

            Collection.AddItemCommand = new RelayCommand<Type>(AddItem);
            Collection.AddExistingItemCommand = new RelayCommand<IControl>(AddExistingItem);
            Collection.DeleteItemCommand = new RelayCommand<IControl>(DeleteItem);
            Collection.RemoveSelectedItemCommand = new RelayCommand(RemoveSelectedItem);
            Collection.ResetItemCommand = new RelayCommand<IControl>(ResetItem);
            ManagerReorderService = reorderServiceFactory?.Invoke(Collection, OnMove);
            DeleteEverywhereCommand = new RelayCommand<IControl>(DeleteEverywhere);

            // Registers under the constructor time ManagerData.ID. Two paths reassign that id
            // afterward, so both call RegisterDeleter again once the real id is known: the seven
            // top level managers get it from MainViewModel.SetupManager, and every manager built
            // through ControlFactory.Create gets it from FromModel, which round trips even a
            // brand new control through a default model. A stale ctor registration is otherwise
            // harmless because DeleteEverywhere only looks up ids that appear in _referencers,
            // which are always the ids AddControl was called with, always after the real id is set.
            RegisterDeleter();
        }

        private MessageCollectionManagerHandler MessageCollectionManagerHandler => Collection.MessageCollectionManagerHandler;

        public CollectionManager Collection { get; set; }
        public IManagerReorderService ManagerReorderService { get; }

        [ObservableProperty]
        private bool isExpanded = false;

        // _isAdding suppresses SelectItemCommand when selection changes as a side effect of AddItem.
        // try/finally ensures the flag is always reset even if Collection.AddItem throws.
        private bool _isAdding = false;

        // Defense in depth against a selection feedback loop: a control bound to a ListBox whose
        // ItemsSource differs from ManagerData.Items (for example a shared repository collection)
        // can push a SelectedItem write back in here while an earlier call on this same manager is
        // still applying its own selection change, recursing without ever converging. The guard
        // makes the setter a no op while it is already running so no binding topology can recurse
        // it to a stack overflow; try/finally ensures the flag always resets even if a step throws.
        private bool _isApplyingSelection = false;

        // The id the deleter is currently registered under. ManagerData.ID is reassigned after
        // construction, so the registration key cannot be read back from it at unregister time.
        private Guid _registeredDeleterId;
        private bool _hasRegisteredDeleter;

        public ManagerData ManagerData => Collection.ManagerData;

        private Guid _compositionID;

        // The id of the composition that owns this manager. The manager gives this id to every item.
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                foreach (var item in Collection.ManagerData.Items)
                    if (item is IHasCompositionID owned)
                        owned.CompositionID = value;
            }
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            foreach (var item in e.NewItems)
                if (item is IHasCompositionID owned)
                    owned.CompositionID = _compositionID;
        }

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
            // newIndex is -1 when the deleted item was the collection's last remaining item; that
            // still needs an undo entry (RemoveItemCommand.Undo reinserts at the original index,
            // it does not use newIndex), so the push no longer bails out on a negative newIndex.
            UndoManager?.Push(new RemoveItemCommand(Collection, ControlMessenger, MessageFactory, control, index, newIndex));
        }

        // Replaces an item with a fresh instance of the same type, keeping its position. The raw
        // CollectionManager.ResetItem swap sent no message, recorded no undo entry and dropped the
        // replaced instance with its subscriptions still live, so the reset command routes through
        // the same machinery delete and add use instead: the engine sees a remove and add pair, the
        // swap is one undo entry, and the replaced instance is owned by that entry until it is
        // dropped, which is the only point where its teardown is safe.
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

        // Registers this manager's DeleteItem as the deleter for its current ManagerData.ID. Called
        // from the ctor and again by app side setup code once ManagerData.ID is reassigned, since the
        // registration key must match the id AddControl used when the control was referenced. The
        // entry made under the previous id goes first, so a manager never leaves a stale deleter
        // behind that would keep it and its whole item graph alive.
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

        // Ends this manager's lifetime. Called from the Dispose of every control that owns nested
        // managers, which the delete path only reaches once the undo stack has dropped the delete.
        public void Dispose()
        {
            Collection.ManagerData.Items.CollectionChanged -= OnItemsChanged;
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
            // ControlFactory.Create round trips every freshly built control through FromModel,
            // not only saved project loads, so nested managers get their ManagerData.ID reassigned
            // here just like the top level ones do in MainViewModel.SetupManager; re register so
            // the deleter key matches the id AddControl and LoadItem will use afterward.
            RegisterDeleter();
        }
    }
}
