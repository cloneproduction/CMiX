// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerData : ObservableObject, IControl
    {
        public ManagerData()
        {
            ID = Guid.NewGuid();
            Items = new ObservableCollection<IControl>();
        }
        public ManagerData(Guid id)
        {
            ID = id;
            Items = new ObservableCollection<IControl>();
        }

        public Guid ID { get; set; }

        private ObservableCollection<IControl> _items;
        public ObservableCollection<IControl> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }
    }
}
