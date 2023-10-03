// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Prefab.Messages;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefab
{
    public partial class PrefabManagerBase : ObservableRecipient, IPrefabManager, IRecipient<MessageRequestControl>
    {
        public PrefabManagerBase(PrefabRepository prefabRepository, PrefabFactory prefabFactory)
        {
            AddItemCommand = new RelayCommand<Type>(AddItem);

            IsActive = true;

            PrefabRepository = prefabRepository;
            PrefabFactory = prefabFactory;
        }

        public PrefabManagerBase(Guid id, PrefabRepository prefabRepository, PrefabFactory prefabFactory) : this(prefabRepository, prefabFactory)
        {
            ID = id;
        }


        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabFactory PrefabFactory { get; set; }
        public PrefabRepository PrefabRepository { get; set; }

        public ICommand AddItemCommand { get; set; }


        [ObservableProperty]
        private IPrefab selectedItem;

        [ObservableProperty]
        private bool isExpanded;

 

        public virtual void AddItem(Type type)
        {
            IPrefab prefab = PrefabFactory.CreatePrefab(type);
            PrefabRepository.AddPrefab(prefab);
            SelectedItem = prefab;

            var prefabModel = ControlMessenger.Mapper.Map<IPrefabModel>(prefab);

            Send(new MessageAddItem(ID, prefabModel));


        }

        public void AddItem(IControlModel controlModel)
        {
            if (controlModel is not IPrefabModel prefabModel)
                return;

            IPrefab prefab = PrefabFactory.CreatePrefab((IPrefabModel)controlModel);
            PrefabRepository.AddPrefab(prefab);

            SelectedItem = prefab;
        }


        partial void OnSelectedItemChanged(IPrefab oldValue, IPrefab newValue)
        {
            if (newValue == null)
                return;

            if (SelectedItem == null)
                return;

            Send(new MessageSelectedItemChanged(ID, newValue.ID, 0));

            SelectedItem = newValue;
        }

        public void SelectedItemChanged(Guid selectedItemID, int index)
        {
            var prefab = PrefabRepository.GetPrefab(selectedItemID);

            if (prefab == null)
                return;

            if (index < 0)
                return;

            SelectedItem = prefab;
        }


        public void Rename()
        {
            if(SelectedItem.GetType() != typeof(EmptyPrefab))
                SelectedItem.IsRenaming.Value = true;
        }

        public void Send(IMessage message)
        {
            WeakReferenceMessenger.Default.Send(message, MessageType.Out);
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }


        public void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public void ReplaceEmptyPrefab(Guid emptyPrefabID, IControlModel controlModel)
        {
            throw new NotImplementedException();
        }
    }
}
