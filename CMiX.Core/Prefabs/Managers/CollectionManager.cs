// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
            ReplaceSelectedItemCommand = new RelayCommand<IControl>(c => ReplaceItem(c));
            RemoveSelectedItemCommand = new RelayCommand(RemoveSelectedItem);
            ResetItemCommand = new RelayCommand<IControl>(ResetItem);
        }

        public Guid ID { get; set; }
        public ManagerData ManagerData { get; }
        public ControlRepository ControlRepository { get; }
        public ControlFactory ControlFactory { get; }
        public MessageCollectionManagerHandler MessageCollectionManagerHandler { get; }

        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RemoveSelectedItemCommand { get; set; }
        public ICommand ReplaceSelectedItemCommand { get; set; }
        public ICommand ResetItemCommand { get; set; }

        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public (IControl prefab, int index) AddItem(Type type)
        {
            var prefab = ControlFactory.Create(type);
            ControlRepository.AddControl(prefab);
            var items = ManagerData.Items;

            if (SelectedItem is EmptyPrefab emptyPrefab && prefab is IPrefab && prefab is not EmptyPrefab)
                items[items.IndexOf(emptyPrefab)] = prefab;
            else
                items.Add(prefab);

            SelectedItem = prefab;
            ManagerData.SelectedIndex = items.IndexOf(prefab);
            return (prefab, ManagerData.SelectedIndex);
        }

        public void AddItem(IControlModel controlModel)
        {
            var items = ManagerData.Items;
            var prefab = ControlFactory.Create(controlModel);

            ControlRepository.AddControl(prefab);

            if (SelectedItem is EmptyPrefab empty)
            {
                var index = items.IndexOf(empty);
                if (index >= 0)
                    items[index] = prefab;
                else
                    items.Add(prefab);
            }
            else
                items.Add(prefab);
            SelectedItem = prefab;
        }

        public (IControl prefab, int index, bool wasReplace) ReplaceItem(IControl control)
        {
            if (control is not IPrefab prefab)
                throw new ArgumentException("Control must implement IPrefab", nameof(control));

            var items = ManagerData.Items;
            bool wasReplace = items.Count > 0;

            if (items.Count == 0)
                items.Add(prefab);
            else
                items[ManagerData.SelectedIndex] = prefab;

            SelectedItem = prefab;
            return (prefab, ManagerData.SelectedIndex, wasReplace);
        }

        public void ReplaceItem(IControlModel controlModel, int index)
        {
            var prefab = ControlRepository.GetControl(controlModel.ID);
            var items = ManagerData.Items;

            if (prefab == null)
            {
                prefab = ControlFactory.Create(controlModel);
                ControlRepository.AddControl(prefab);
            }

            if (items.Count == 0)
                items.Add(prefab);
            else
                items[index] = prefab;

            SelectedItem = prefab;
            ManagerData.SelectedIndex = index;
        }

        public (IControl removed, int newIndex) DeleteItem(IControl control)
        {
            if (control == null) return (null, -1);
            var items = ManagerData.Items;
            var index = items.IndexOf(control);
            if (index < 0) return (null, -1);

            items.RemoveAt(index);
            int newIndex = items.Count == 0 ? -1 : index == 0 ? 0 : index - 1;
            SelectedItem = items.Count == 0 ? null : items[newIndex];
            ManagerData.SelectedIndex = newIndex;
            return (control, newIndex);
        }

        public void DeleteItem(Guid id)
        {
            var items = ManagerData.Items;
            var prefab = items.FirstOrDefault(x => x.ID == id);
            if (prefab == null) return;

            var index = items.IndexOf(prefab);
            items.Remove(prefab);
            if (items.Count == 0)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = -1;
            }
            else
            {
                var newIndex = index == 0 ? 0 : index - 1;
                SelectedItem = items[newIndex];
                ManagerData.SelectedIndex = newIndex;
            }
        }

        public void RemoveSelectedItem()
        {
            SelectedItem = null;
        }

        public void MoveItem(int oldIndex, int newIndex)
        {
            var items = ManagerData.Items;
            if (items.Count == 0) return;
            items.Move(oldIndex, newIndex);
        }

        public void SelectedItemChanged(IControlModel controlModel, int index)
        {
            var control = controlModel != null ? ControlRepository.GetControl(controlModel.ID) : null;
            SelectedItem = control;
            ManagerData.SelectedIndex = control != null ? index : -1;
            if (control != null && ManagerData.Items.All(x => x.ID != control.ID))
                ManagerData.Items.Add(control);
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

        public void ResetItem(IControl control)
        {
            var newControl = ControlFactory.Create(control.GetType());
            var index = ManagerData.Items.IndexOf(control);
            ManagerData.Items[index] = newControl;
        }

        public void Rename()
        {
            if (SelectedItem is IPrefab prefab && prefab.GetType() != typeof(EmptyPrefab))
                prefab.PrefabService.IsRenaming.Value = true;
        }

        public void SelectedItemIsRenaming()
        {
            if (SelectedItem is IPrefab prefab)
                prefab.PrefabService.IsRenaming.Value = true;
        }
    }
}
