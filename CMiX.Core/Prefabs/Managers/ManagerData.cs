// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerData : ObservableObject
    {
        public ManagerData()
        {
            Items.CollectionChanged += (s, e) => OnPropertyChanged(nameof(SelectedIndex));
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ObservableCollection<IControl> Items { get; } = new();

        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (!SetProperty(ref _selectedItem, value)) return;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public int SelectedIndex => SelectedItem == null ? -1 : Items.IndexOf(SelectedItem);

        // The new item is selected.
        public void Insert(int index, IControl control)
        {
            Items.Insert(index, control);
            SelectedItem = control;
        }

        // Returns the index the control had, or -1 when it is not in the list.
        // The selection moves to the item before it only when the removed item was selected.
        public int Remove(IControl control)
        {
            var index = Items.IndexOf(control);
            if (index < 0) return -1;

            if (ReferenceEquals(control, SelectedItem))
                SelectedItem = Items.Count == 1 ? null : Items[index == 0 ? 1 : index - 1];
            Items.RemoveAt(index);
            return index;
        }

        // The selection stays on the moved item, also when a listener clears it during the move.
        public bool Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= Items.Count) return false;
            if (newIndex < 0 || newIndex >= Items.Count) return false;

            var selected = SelectedItem;
            Items.Move(oldIndex, newIndex);
            Select(selected);
            return true;
        }

        // A control that is not in the list leaves nothing selected.
        public void Select(IControl control) => SelectedItem = control != null && Items.Contains(control) ? control : null;

        public void SelectAt(int index) => Select(index >= 0 && index < Items.Count ? Items[index] : null);

        public void ClearSelection() => SelectedItem = null;
    }
}
