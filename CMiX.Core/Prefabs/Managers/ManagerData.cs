// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerData : ObservableRecipient, IControl//, IRecipient<IMessage>
    {
        public ManagerData()
        {
            ID = Guid.NewGuid();
            Items = new ObservableCollection<IControl>();
            IsActive = true;
        }

        //public IControlModel ToModel() => this.MapToModel();
        public Guid ID { get; set; }

        private ObservableCollection<IControl> _items;
        public ObservableCollection<IControl> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }
    }
}
