// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class PrefabSlotManager : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public PrefabSlotManager(PrefabSlotManagerModel entityManagerModel)
        {
            ID = entityManagerModel.ID;

            PrefabSlots = new ObservableCollection<PrefabSlot>();

            AddItemCommand = new RelayCommand(AddItem);
            DeleteItemCommand = new RelayCommand(DeleteItem);
            RenameCommand = new RelayCommand(Rename);

            WeakReferenceMessenger.Default.Register(this, MessageType.In);
        }

        public Guid ID { get; set; }

        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RenameCommand { get; set; }

        public ObservableCollection<PrefabSlot> PrefabSlots { get; set; }


        private PrefabSlot _selectedItem;
        public PrefabSlot SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private IPrefab _comboboxSelectedItem;
        public IPrefab ComboboxSelectedItem
        {
            get => _comboboxSelectedItem;
            set => SetProperty(ref _comboboxSelectedItem, value);
        }


        public void AddItem()
        {
            PrefabSlot slot = new PrefabSlot();
            PrefabSlots.Add(slot);
            SelectedItem = slot;
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddSlot(this.ID, SelectedItem), MessageType.Out);
        }


        public void DeleteItem()
        {
            var index = PrefabSlots.IndexOf(SelectedItem);

            if (SelectedItem == null)
                return;

            PrefabSlots.Remove(SelectedItem);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemoveSlot(this.ID, SelectedItem), MessageType.Out);

            if (PrefabSlots.Count == 0)
            {
                SelectedItem = null;
                return;
            }

            if (index == 0)
            {
                SelectedItem = PrefabSlots[0];
                return;
            }

            if (index > 0)
            {
                SelectedItem = PrefabSlots[index - 1];
                return;
            }
        }


        public void Rename()
        {

        }


        public IModel GetModel()
        {
            PrefabSlotManagerModel prefabSlotManagerModel = new PrefabSlotManagerModel();

            return prefabSlotManagerModel;
        }

        public void SetViewModel(IModel model)
        {
            //throw new NotImplementedException();
        }

        public void Receive(IMessage message)
        {
            //throw new NotImplementedException();
        }
    }
}
