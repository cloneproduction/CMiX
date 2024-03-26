// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerData : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public ManagerData(ManagerMessenger managerMessenger)
        {
            ID = Guid.NewGuid();
            Items = new ObservableCollection<IControl>();
            ManagerMessenger = managerMessenger;
            IsActive = true;
        }


        public ManagerMessenger ManagerMessenger { get; set; }
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
            set
            {
                SetProperty(ref _selectedItem, value);
                if (IsActive)
                    ManagerMessenger.SendSelectedItemChanged(this.ID, SelectedItem, SelectedIndex);
                Console.WriteLine(value);
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public void Receive(IMessage message)
        {
            ManagerMessenger.Receive(this, message);
        }
    }
}
