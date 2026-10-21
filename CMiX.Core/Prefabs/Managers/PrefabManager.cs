using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class PrefabManager : PrefabManagerBase, IDisposable
    {
        public PrefabManager(CollectionManager collection,
                             ControlFactory controlFactory,
                             MessageCollectionManagerHandler messageHandler,
                             ControlMessenger controlMessenger,
                             MessageFactory messageFactory,
                             ControlActivationService activationService,
                             UndoManager undoManager,
                             ManagerReorderServiceFactory reorderServiceFactory = null)
            : base(collection.ControlRepository, controlMessenger, messageFactory, activationService, undoManager)
        {
            ID = collection.ManagerData.ID;

            Collection = collection;
            _controlFactory = controlFactory;
            _messageHandler = messageHandler;
            _sync = new CollectionSync(collection, controlMessenger, messageFactory);
            Collection.ManagerData.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ManagerData.SelectedItem))
                    OnPropertyChanged(nameof(SelectedItem));
            };
            Collection.ManagerData.Items.CollectionChanged += OnItemsChanged;

            AddItemCommand = new RelayCommand<Type>(AddItem);
            AddExistingItemCommand = new RelayCommand<IControl>(AddExistingItem);
            RemoveItemCommand = new RelayCommand<IControl>(RemoveItem);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
            ResetItemCommand = new RelayCommand<IControl>(ResetItem);
            ManagerReorderService = reorderServiceFactory?.Invoke(ManagerData, MoveItem);
            RemoveEverywhereCommand = new RelayCommand<IControl>(RemoveEverywhere);
        }

        private readonly ControlFactory _controlFactory;
        private readonly MessageCollectionManagerHandler _messageHandler;
        private readonly CollectionSync _sync;

        public CollectionManager Collection { get; set; }
        public IManagerReorderService ManagerReorderService { get; }

        [ObservableProperty]
        private bool isExpanded = false;

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

        public override ICommand AddItemCommand { get; }
        public ICommand AddExistingItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public override ICommand ClearSelectionCommand { get; }
        public ICommand ResetItemCommand { get; }
        public ICommand RemoveEverywhereCommand { get; }

        public override IControl SelectedItem
        {
            get => ManagerData.SelectedItem;
            set
            {
                if (ReferenceEquals(value, ManagerData.SelectedItem)) return;
                if (value == null) { ManagerData.ClearSelection(); return; }
                EnsureItemInCollection(value);
                if (Collection.ManagerData.Items.Contains(value))
                    SelectAt(Collection.ManagerData.Items.IndexOf(value));
            }
        }

        private void EnsureItemInCollection(IControl control)
        {
            if (Collection.ManagerData.Items.Contains(control)) return;

            Run(new AddItemUndoCommand(_sync, control, ManagerData.Items.Count));
        }

        public void ClearAll()
        {
            var items = Collection.ManagerData.Items.ToList();
            Collection.ClearAll();
            foreach (var item in items)
                _sync.SendRemove(item, -1);
        }

        public void AddItem(Type type)
        {
            var item = _controlFactory.Create(type);
            if (item == null) return;
            Run(new AddItemUndoCommand(_sync, item, ManagerData.Items.Count));
        }

        public void AddItemFromModel(IControlModel model)
        {
            Collection.AddItemFromModel(model);
            var item = ManagerData.SelectedItem;
            var index = Collection.ManagerData.Items.IndexOf(item);
            _sync.SendAdd(item, index);
        }

        // Adds a deep copy right after the source, with new ids and the next free name, as one undo step.
        // A source that is not in this list gives a copy at the end of the list.
        public IControl DuplicateItem(IControl source)
        {
            if (source == null || UndoManager?.IsApplying == true) return null;

            var index = ManagerData.Items.IndexOf(source);
            var insertAt = index < 0 ? ManagerData.Items.Count : index + 1;

            var factory = _controlFactory;
            var copy = factory.Create(ControlCloner.CloneWithNewGuids(source.ToModel()));
            if (copy is IPrefab prefab)
            {
                // The rename is part of the copy: it sends no message and leaves no undo step of its own.
                var name = prefab.PrefabService.Name;
                var newName = factory.UniqueName(name.Value);
                name.IsActive = false;
                try { name.Value = newName; }
                finally { name.IsActive = true; }
            }

            Run(new AddItemUndoCommand(_sync, copy, insertAt));
            return copy;
        }

        // The add and the action are one undo step.
        public void AddItem(Type type, Action<IControl> afterAdd)
        {
            UndoManager?.BeginCapture();
            try
            {
                var item = _controlFactory.Create(type);
                if (item == null) return;
                Run(new AddItemUndoCommand(_sync, item, ManagerData.Items.Count));
                afterAdd(item);
            }
            finally { UndoManager?.EndCapture(); }
        }

        public void AddExistingItem(IControl control)
        {
            EnsureItemInCollection(control);
            SelectAt(Collection.ManagerData.Items.IndexOf(control));
        }

        public void RemoveItem(IControl control) => RemoveFrom(Collection, control);

        private void RemoveFrom(CollectionManager collectionManager, IControl control)
        {
            if (!collectionManager.ManagerData.Items.Contains(control)) return;
            Run(new RemoveItemUndoCommand(new CollectionSync(collectionManager, ControlMessenger, MessageFactory), control));
        }

        // Swaps in a fresh instance at the same position as one undoable remove and add.
        public void ResetItem(IControl control)
        {
            if (control == null) return;
            if (UndoManager?.IsApplying == true) return;

            var index = Collection.ManagerData.Items.IndexOf(control);
            if (index < 0) return;

            var replacement = _controlFactory.Create(control.GetType());
            if (replacement == null) return;

            var command = new ReplaceItemUndoCommand(_sync, control, replacement, index);
            command.Execute();
            UndoManager?.Push(command);
        }

        // Ends the manager's lifetime; called from the Dispose of the owning control.
        public void Dispose()
        {
            Collection.ManagerData.Items.CollectionChanged -= OnItemsChanged;
            ClearAll();
        }

        public void RemoveEverywhere(IControl control)
        {
            control ??= SelectedItem;
            UndoManager?.BeginCapture();
            try
            {
                foreach (var collectionManager in Collection.ControlRepository.GetCollectionManagers(control))
                    RemoveFrom(collectionManager, control);
            }
            finally
            {
                UndoManager?.EndCapture();
            }
        }

        // Moves an item to a new index as one undoable step that peers also see. Does nothing for a bad index.
        public void MoveItemTo(IControl item, int newIndex) =>
            MoveItem(ManagerData.Items.IndexOf(item), newIndex);

        public void MoveItem(int oldIndex, int newIndex)
        {
            var count = ManagerData.Items.Count;
            if (oldIndex < 0 || oldIndex == newIndex || newIndex < 0 || newIndex >= count) return;

            Run(new MoveItemUndoCommand(_sync, oldIndex, newIndex));
        }

        private void Run(IUndoCommand command)
        {
            command.Execute();
            UndoManager?.Push(command);
        }

        public void ClearSelection()
        {
            var previousItem = ManagerData.SelectedItem;
            var previousIndex = ManagerData.Items.IndexOf(previousItem);
            ManagerData.ClearSelection();
            _sync.SendClearSelection();
            if (previousIndex < 0) return;
            UndoManager?.Push(new ClearSelectionUndoCommand(_sync, previousItem, previousIndex));
        }

        public void SelectAt(int index)
        {
            var previousItem = ManagerData.SelectedItem;
            var previousIndex = ManagerData.Items.IndexOf(previousItem);
            ManagerData.SelectAt(index);
            if (ReferenceEquals(previousItem, ManagerData.SelectedItem)) return;
            _sync.SendSelect(ManagerData.SelectedItem, index);
            if (previousItem == null) return;
            UndoManager?.Push(new SelectItemUndoCommand(_sync, previousItem, previousIndex, index));
        }

        public void LoadItem(IControlModel controlModel) => Collection.LoadItem(controlModel);

        public override void Receive(IMessage message)
        {
            if (message is not IMessageManager || ManagerData.ID != message.ID)
                return;
            ReceiveWithoutEcho(() => _messageHandler.Handle(Collection, message));
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
                        .Where(c => Collection.ControlRepository.HasCollectionManagers(c))
                        .Select(c => c.ToModel())
                        .ToList()
                )
            }
        };

        public override void FromModel(IControlModel model)
        {
            var m = (PrefabManagerModel)model;
            ManagerData.ID = m.ManagerData.ID;
        }
    }
}
