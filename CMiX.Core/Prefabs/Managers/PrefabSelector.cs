// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefab.Messages;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefab.Managers
{
    public partial class PrefabSelector<T> : ObservableRecipient, IControl, IRecipient<MessageRequestControl> where T : class, IPrefab
    {
        public PrefabSelector(PrefabSelectorModel prefabSelectorModel, CompositionService compositionService, PrefabRepository<T> prefabRepository)
        {
            ID = prefabSelectorModel.ID;
            PrefabRepository = prefabRepository;
            PrefabFactory = compositionService.PrefabFactory;
            AddItemCommand = new RelayCommand(AddItem);
            RemoveItemCommand = new RelayCommand(DeleteItem);
            SelectedItemChangedCommand = new RelayCommand<IPrefab>(SelectedItemChanged);
            IsActive = true;
        }

        private void AddItem()
        {
            IPrefab prefab = PrefabFactory.CreatePrefab(typeof(T));
            PrefabRepository?.AddPrefab((T)prefab);
            Send(new MessageSelectorAddPrefab(ID, ControlMessenger.Mapper.Map<IPrefabModel>(prefab)));
            SelectedItem = (T)prefab;
        }

        public void AddPrefab(IPrefabModel prefabModel)
        {
            if (prefabModel == null)
                return;

            SelectedItem = PrefabFactory.CreatePrefab(prefabModel);
            PrefabRepository?.AddPrefab((T)SelectedItem);
        }


        private void DeleteItem()
        {

        }

        public void SelectedItemChanged(IPrefab prefab)
        {
            if(prefab == null) 
                return;

            SelectedItem = (T)prefab;
            Send(new MessageSelectedPrefabChanged(ID, SelectedItem.ID));
        }

        public void SelectedItemChanged(Guid selectedItemID)
        {
            SelectedItem = (T)PrefabRepository.GetPrefab(selectedItemID);
        }

        public void Send(IMessage message)
        {
            WeakReferenceMessenger.Default.Send(message, MessageType.Out);
        }


        private IPrefab _selectedItem;
        public IPrefab SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                //if (IsActive && value != null)
                    //Send(new MessageSelectedPrefabChanged(ID, SelectedItem.ID));
                //Console.WriteLine(value);
            }
        }


        [ObservableProperty]
        private int selectedIndex;


        public Guid ID { get; set; }
        public PrefabFactory PrefabFactory { get; set; }
        public PrefabRepository<T> PrefabRepository { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }


        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
