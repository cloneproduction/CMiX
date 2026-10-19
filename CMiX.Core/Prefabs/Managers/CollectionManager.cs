// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs.Managers
{
    public class CollectionManager : ObservableObject
    {
        public CollectionManager(ManagerData managerData,
                                 ControlRepository controlRepository,
                                 ControlFactory controlFactory)
        {
            ManagerData = managerData;
            ControlRepository = controlRepository;
            ControlFactory = controlFactory;
        }

        public ManagerData ManagerData { get; }
        public ControlRepository ControlRepository { get; }
        public ControlFactory ControlFactory { get; }

        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public void ClearAll()
        {
            foreach (var item in ManagerData.Items.ToList())
                RemoveAndDispose(item);

            SelectedItem = null;
            ManagerData.SelectedIndex = -1;
        }

        private void AddControlToCollection(IControl prefab)
        {
            var items = ManagerData.Items;
            ControlRepository.AddControl(prefab, this);
            items.Add(prefab);
            SelectedItem = prefab;
            ManagerData.SelectedIndex = items.IndexOf(prefab);
        }

        public void AddItem(IControlModel controlModel)
        {
            var prefab = CreateOrReuse(controlModel);
            AddControlToCollection(prefab);
        }

        // Managers can share one control. A model with the ID of a known control gives back that control.
        private IControl CreateOrReuse(IControlModel controlModel)
        {
            var existing = controlModel.ID != Guid.Empty ? ControlRepository.GetControl(controlModel.ID) : null;
            return existing ?? ControlFactory.Create(controlModel);
        }

        public void InsertItem(IControl control, int index)
        {
            ControlRepository.AddControl(control, this);
            ManagerData.Items.Insert(index, control);
            SelectedItem = control;
            ManagerData.SelectedIndex = index;
        }

        // The index is kept inside the list, so a stale index from a peer cannot throw.
        public void InsertItem(IControlModel controlModel, int index)
        {
            var prefab = CreateOrReuse(controlModel);
            InsertItem(prefab, Math.Clamp(index, 0, ManagerData.Items.Count));
        }

        public void LoadItem(IControlModel controlModel)
        {
            var prefab = CreateOrReuse(controlModel);
            ControlRepository.AddControl(prefab, this);
            ManagerData.Items.Add(prefab);
        }

        private (IControl removed, int newIndex) RemoveFromList(IControl control)
        {
            var items = ManagerData.Items;
            var index = items.IndexOf(control);
            if (index < 0) return (null, -1);

            items.RemoveAt(index);
            ControlRepository.RemoveControl(control, this);

            int newIndex = items.Count == 0 ? -1 : index == 0 ? 0 : index - 1;
            SelectedItem = items.Count == 0 ? null : items[newIndex];
            ManagerData.SelectedIndex = newIndex;
            return (control, newIndex);
        }

        private void RemoveAndDispose(IControl control)
        {
            if (RemoveFromList(control).removed != null)
                OwnedControl.DisposeIfOrphaned(ControlRepository, control);
        }

        // Not disposed on the undoable delete path: the RemoveItemCommand owns the removed control until it is dropped.
        public (IControl removed, int newIndex) DeleteItem(IControl control)
        {
            if (control == null) return (null, -1);
            return RemoveFromList(control);
        }

        public void DeleteItem(Guid id)
        {
            var control = ManagerData.Items.FirstOrDefault(x => x.ID == id);
            if (control != null)
                RemoveAndDispose(control);
        }

        public void RemoveSelectedItem()
        {
            SelectedItem = null;
            ManagerData.SelectedIndex = -1;
        }

        public void MoveItem(int oldIndex, int newIndex)
        {
            var items = ManagerData.Items;
            if (oldIndex < 0 || oldIndex > items.Count - 1) return;
            if (newIndex < 0 || newIndex > items.Count - 1) return;

            var wasSelected = SelectedItem == items[oldIndex];

            items.Move(oldIndex, newIndex);

            // Avalonia ignores NotifyCollectionChangedAction.Move, so the selection is set again (Avalonia issues 2522, 16279).
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
    }
}
