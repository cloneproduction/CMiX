// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class CollectionManager : ObservableObject, IControl
    {
        public CollectionManager(ManagerData managerData,
                                 ControlRepository controlRepository,
                                 ControlFactory controlFactory,
                                 MessageCollectionManagerHandler messageCollectionManagerHandler)
        {
            ID = managerData.ID;
            ManagerData = managerData;
            ControlRepository = controlRepository;
            ControlFactory = controlFactory;
            MessageCollectionManagerHandler = messageCollectionManagerHandler;

            AddItemCommand = new RelayCommand<Type>(t => AddItem(t));
            DeleteItemCommand = new RelayCommand<IControl>(c => DeleteItem(c));
            RemoveSelectedItemCommand = new RelayCommand(RemoveSelectedItem);
            ResetItemCommand = new RelayCommand<IControl>(ResetItem);
        }

        public Guid ID { get; set; }
        public ManagerData ManagerData { get; }
        public ControlRepository ControlRepository { get; }
        public ControlFactory ControlFactory { get; }
        public MessageCollectionManagerHandler MessageCollectionManagerHandler { get; }

        public ICommand AddItemCommand { get; set; }
        public ICommand AddExistingItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RemoveSelectedItemCommand { get; set; }
        public ICommand ResetItemCommand { get; set; }

        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public void ClearAll()
        {
            foreach (var item in ManagerData.Items.ToList())
                RemoveControlFromCollection(item, disposeIfOrphaned: true);

            ManagerData.SelectedIndex = -1;
            SelectedItem = null;
        }

        private void AddControlToCollection(IControl prefab)
        {
            var items = ManagerData.Items;
            ControlRepository.AddControl(prefab, ManagerData.ID);
            items.Add(prefab);
            ManagerData.SelectedIndex = items.IndexOf(prefab);
            SelectedItem = prefab;
        }

        public (IControl prefab, int index) AddItem(Type type)
        {
            var prefab = ControlFactory.Create(type);
            AddControlToCollection(prefab);
            return (prefab, ManagerData.SelectedIndex);
        }

        public void AddItem(IControl control)
        {
            AddControlToCollection(control);
            //ManagerData.SelectedIndex = ManagerData.Items.IndexOf(control);
        }

        public void AddItem(IControlModel controlModel)
        {
            var prefab = ControlFactory.Create(controlModel);
            AddControlToCollection(prefab);
        }

        public void InsertItem(IControl control, int index)
        {
            ControlRepository.AddControl(control, ManagerData.ID);
            ManagerData.Items.Insert(index, control);
            ManagerData.SelectedIndex = index;
            SelectedItem = control;
        }

        public void LoadControlIntoCollection(IControl prefab)
        {
            ControlRepository.AddControl(prefab, ManagerData.ID);
            ManagerData.Items.Add(prefab);
        }

        public void LoadItem(IControlModel controlModel)
        {
            var prefab = ControlFactory.Create(controlModel);
            LoadControlIntoCollection(prefab);
        }

        // disposeIfOrphaned stays false on the undoable delete path. There the removed instance is
        // owned by the RemoveItemCommand sitting on the undo stack, which disposes it only once the
        // command is dropped, so undoing a delete puts the control back with its contents intact.
        // Teardown paths that no undo command can reverse still dispose here.
        private (IControl removed, int newIndex) RemoveControlFromCollection(IControl control, bool disposeIfOrphaned)
        {
            var items = ManagerData.Items;
            var index = items.IndexOf(control);
            if (index < 0) return (null, -1);

            items.RemoveAt(index);
            ControlRepository.RemoveControl(control, ManagerData.ID);

            if (disposeIfOrphaned)
                OwnedControl.DisposeIfOrphaned(ControlRepository, control);

            int newIndex = items.Count == 0 ? -1 : index == 0 ? 0 : index - 1;
            SelectedItem = items.Count == 0 ? null : items[newIndex];
            ManagerData.SelectedIndex = newIndex;
            return (control, newIndex);
        }

        public (IControl removed, int newIndex) DeleteItem(IControl control)
        {
            if (control == null) return (null, -1);
            return RemoveControlFromCollection(control, disposeIfOrphaned: false);
        }

        public void DeleteItem(Guid id)
        {
            var control = ManagerData.Items.FirstOrDefault(x => x.ID == id);
            if (control != null)
                RemoveControlFromCollection(control, disposeIfOrphaned: true);
        }

        public void RemoveSelectedItem()
        {
            SelectedItem = null;
            ManagerData.SelectedIndex = -1;
        }

        //public void MoveItem(int oldIndex, int newIndex)
        //{
        //    var items = ManagerData.Items;
        //    if (oldIndex < 0 || oldIndex > items.Count - 1) return;
        //    if (newIndex < 0 || newIndex > items.Count - 1) return;
        //    items.Move(oldIndex, newIndex);
        //}

        public void MoveItem(int oldIndex, int newIndex)
        {
            var items = ManagerData.Items;
            if (oldIndex < 0 || oldIndex > items.Count - 1) return;
            if (newIndex < 0 || newIndex > items.Count - 1) return;

            var wasSelected = SelectedItem == items[oldIndex];

            items.Move(oldIndex, newIndex);

            // Avalonia's SelectingItemsControl does not process
            // NotifyCollectionChangedAction.Move, so the ListBox visually loses
            // selection even though the moved item is still the same instance
            // (github.com/AvaloniaUI/Avalonia/issues/2522, /issues/16279).
            // Re-assert selection explicitly to work around it.
            if (wasSelected)
            {
                SelectedItem = null;
                SelectedItem = items[newIndex];
                ManagerData.SelectedIndex = newIndex;
            }
        }

        public void SelectedItemChanged(Guid controlID, int index)
        {
            var control = controlID != Guid.Empty ? ControlRepository.GetControl(controlID) : null;
            SelectedItem = control;
            ManagerData.SelectedIndex = control != null ? index : -1;
        }

        public void SelectedItemChanged(int index)
        {
            if (index < 0 || ManagerData.Items.Count == 0)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = -1;
                return;
            }
            SelectedItem = ManagerData.Items[index];
            ManagerData.SelectedIndex = index;
        }

        // The bare collection level swap, with no message and no undo entry. PrefabManager replaces
        // the reset command with its own undoable path, so this one only runs for a collection that
        // no manager owns. Nothing can put the replaced control back afterward, so it is disposed
        // here like the other teardown paths do, and the selection follows the replacement instead
        // of pointing at an instance the collection no longer holds.
        public void ResetItem(IControl control)
        {
            if (control == null) return;
            var index = ManagerData.Items.IndexOf(control);
            if (index < 0) return;
            var newControl = ControlFactory.Create(control.GetType());
            ManagerData.Items[index] = newControl;
            ControlRepository.RemoveControl(control, ManagerData.ID);
            ControlRepository.AddControl(newControl, ManagerData.ID);

            if (SelectedItem == control)
                SelectedItem = newControl;

            OwnedControl.DisposeIfOrphaned(ControlRepository, control);
        }

        public IControlModel ToModel() => new CollectionManagerModel
        {
            ID = ID,
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
            var m = (CollectionManagerModel)model;
            ID = m.ID;
            ManagerData.SelectedIndex = m.ManagerData.SelectedIndex;
        }
    }
}
