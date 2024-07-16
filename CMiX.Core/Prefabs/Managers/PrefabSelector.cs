// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Managers
{
    public class PrefabSelector : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public PrefabSelector(ManagerData managerData,
                              ControlRepository controlRepository,
                              ManagerMessenger managerMessenger)
        {
            ID = managerData.ID;
            ManagerData = managerData;
            ControlRepository = controlRepository;
            ManagerMessenger = managerMessenger;
            IsActive = true;
        }

        public Guid ID { get; set; }
        public ManagerData ManagerData { get; set; }
        public ControlRepository ControlRepository { get; set; }
        public ManagerMessenger ManagerMessenger { get; set; }



        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if (IsActive)
                    ManagerMessenger.SendSelectedItemChanged(ManagerData.ID, SelectedItem, ManagerData.SelectedIndex);
            }
        }

        public void SelectedItemChanged(Guid selectedItemID, int index)
        {
            var control = ControlRepository.GetControl(selectedItemID);
            SelectedItem = control;

            if (control == null)
            {
                ManagerData.SelectedIndex = -1;
                return;
            }

            ManagerData.SelectedIndex = index;
            if (!ManagerData.Items.Any(x => x.ID == control.ID) && control != null)
                ManagerData.Items.Add(SelectedItem);
        }


        public void Receive(IMessage message)
        {
            ManagerMessenger.Receive(this, message);
        }
    }
}
