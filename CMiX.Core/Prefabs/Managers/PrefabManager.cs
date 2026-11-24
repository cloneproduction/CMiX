using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using CMiX.Core.BaseControls;
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
                             ItemListFactory listFor,
                             ControlActivationService activationService,
                             GenericValue<bool> hidden,
                             UndoManager undoManager,
                             ManagerReorderServiceFactory reorderServiceFactory = null)
            : base(collection.ControlRepository, activationService, undoManager)
        {
            ID = collection.ManagerData.ID;

            Collection = collection;
            _controlFactory = controlFactory;
            Hidden = hidden;
            collection.Hidden = hidden;
            UpdateHiddenId();
            collection.ManagerData.IdChanged += UpdateHiddenId;
            _messageHandler = messageHandler;
            _listFor = listFor;
            _sync = listFor(collection);
            Collection.ManagerData.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ManagerData.SelectedItem))
                    OnPropertyChanged(nameof(SelectedItem));
            };
            Collection.ManagerData.Items.CollectionChanged += OnItemsChanged;

            AddItemCommand = new RelayCommand<Type>(AddItem);
            AddExistingItemCommand = new RelayCommand<IControl>(AddExistingItem);
            RemoveItemCommand = new RelayCommand<IControl>(RemoveItem);
            DuplicateItemCommand = new RelayCommand<IControl>(item => DuplicateItem(item));
            ClearSelectionCommand = new RelayCommand(ClearSelection);
            ResetItemCommand = new RelayCommand<IControl>(ResetItem);
            ManagerReorderService = reorderServiceFactory?.Invoke(ManagerData, MoveItem);
            RemoveEverywhereCommand = new RelayCommand<IControl>(RemoveEverywhere);
        }

        private readonly ControlFactory _controlFactory;
        private readonly MessageCollectionManagerHandler _messageHandler;
        private readonly ItemListFactory _listFor;
        private readonly CollectionSync _sync;

        public CollectionManager Collection { get; set; }
        public IManagerReorderService ManagerReorderService { get; }

        [ObservableProperty]
        private bool isExpanded = true;

        public ManagerData ManagerData => Collection.ManagerData;

        // True when the whole list is hidden: nothing of it is drawn, and the items keep their own visibility (see VisibleItemsProcessor).
        public GenericValue<bool> Hidden { get; }

        // The ID of the hide value comes from the ID of the list, so every peer and every fresh project has the same one.
        private void UpdateHiddenId() => Hidden.ID = DerivedId.Of(ManagerData.ID, nameof(PrefabManagerModel.Hidden));

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
        public ICommand DuplicateItemCommand { get; }
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

        // The owner of the manager can refuse an existing control, for example a texture that would loop back to it.
        public Func<IControl, bool> CanAddExistingItem { get; set; }

        private void EnsureItemInCollection(IControl control)
        {
            if (Collection.ManagerData.Items.Contains(control)) return;
            if (CanAddExistingItem?.Invoke(control) == false) return;

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
                using (name.SuppressSending())
                    name.Value = newName;
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
            var index = Collection.ManagerData.Items.IndexOf(control);
            if (index >= 0)
                SelectAt(index);
        }

        public void RemoveItem(IControl control) => RemoveFrom(Collection, control);

        private void RemoveFrom(CollectionManager collectionManager, IControl control)
        {
            if (!collectionManager.ManagerData.Items.Contains(control)) return;
            Run(new RemoveItemUndoCommand(_listFor(collectionManager), control));
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

        // Moves an item from the source list into this list at an index, as one undo step that peers also see.
        // An item that this list refuses, or that is not in the source, stays where it is, and the result is None.
        // An item that other items block stays in the source list, and the result gives the items that block it.
        public MoveResult MoveItemToManager(IControl item, PrefabManager source, int index)
        {
            if (item == null || source == null || ReferenceEquals(source, this) || UndoManager?.IsApplying == true
                || !source.ManagerData.Items.Contains(item)
                || CanAddExistingItem?.Invoke(item) == false)
                return MoveResult.None;

            if (item is IMoveRestricted restricted)
            {
                var blocking = restricted.GetItemsBlockingMove(source.ManagerData.Items);
                if (blocking.Count > 0)
                    return MoveResult.BlockedBy(blocking);
            }

            UndoManager?.BeginCapture();
            try
            {
                // The item enters this list before it leaves the source, so it is never orphaned.
                Run(new AddItemUndoCommand(_sync, item, Math.Clamp(index, 0, ManagerData.Items.Count)));
                source.RemoveItem(item);
            }
            finally
            {
                UndoManager?.EndCapture();
            }

            return MoveResult.Done;
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
            using (SuppressSending())
                _messageHandler.Handle(Collection, message);
        }

        public override IControlModel ToModel() => new PrefabManagerModel
        {
            ID = ManagerData.ID,
            Hidden = (GenericValueModel<bool>)Hidden.ToModel(),
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
            if (m.Hidden != null)
                Hidden.FromModel(m.Hidden);
            UpdateHiddenId();
        }
    }
}
