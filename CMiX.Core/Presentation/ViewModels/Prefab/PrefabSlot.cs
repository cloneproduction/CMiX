// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class PrefabSlot : ObservableObject, IControl, IRecipient<IMessage>
    {
        public PrefabSlot()
        {
            WeakReferenceMessenger.Default.Register(this, MessageType.In);
        }

        private IPrefab _prefab;
        public IPrefab Prefab
        {
            get => _prefab;
            set
            {
                SetProperty(ref _prefab, value);
                OnPropertyChanged("Name");
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageSelectedPrefabChanged(this.ID, Prefab), MessageType.Out);
            }
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string Name
        {
            get => Prefab?.Name;
        }

        public Guid ID { get; set; }

        public void SetViewModel(IModel model)
        {
            PrefabSlotModel prefabSlotModel = model as PrefabSlotModel;
            this.ID = prefabSlotModel.ID;
            this.IsSelected = prefabSlotModel.IsSelected;
        }

        public IModel GetModel()
        {
            PrefabSlotModel prefabSlotModel = new PrefabSlotModel();
            prefabSlotModel.ID = this.ID;
            prefabSlotModel.IsSelected = this.IsSelected;
            return prefabSlotModel;
        }

        public void Receive(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
