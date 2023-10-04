// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Prefab.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefab
{
    public partial class PrefabManagerSlot : ObservableRecipient, IPrefabCollectionManager, IRecipient<MessageRequestControl>
    {
        public PrefabManagerSlot(PrefabRepository prefabRepository, PrefabFactory prefabFactory)
        {
            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            AddItemCommand = new RelayCommand<Type>(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyPrefab);
            DeleteItemCommand = new RelayCommand<IPrefab>(DeleteItem);
            IsActive = true;

            PrefabRepository = prefabRepository;
            PrefabFactory = prefabFactory;

            Prefabs = new ObservableCollection<IPrefab>();
        }

        public PrefabManagerSlot(Guid id, PrefabRepository prefabRepository, PrefabFactory prefabFactory) : this(prefabRepository, prefabFactory)
        {
            ID = id;
        }


        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabFactory PrefabFactory { get; set; }
        public PrefabRepository PrefabRepository { get; set; }

        public ICommand ItemUpCommand { get; set; }
        public ICommand ItemDownCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand AddEmptyItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }


        [ObservableProperty]
        private ObservableCollection<IPrefab> prefabs;

        [ObservableProperty]
        private IPrefab selectedItem;

        [ObservableProperty]
        private int selectedIndex;

        [ObservableProperty]
        private bool isExpanded;


        public void ItemUp()
        {
            var index = SelectedIndex;

            if(index <= 0) 
                return;

            Prefabs.Move(index, index - 1);
            Send(new MessageMoveItem(ID, index, index - 1));
        }

        public void ItemDown()
        {
            var index = SelectedIndex;

            if (index == Prefabs.Count - 1)
                return;

            Prefabs.Move(index, index + 1);
            Send(new MessageMoveItem(ID, index, index + 1));
        }


        public virtual void AddItem(Type type)
        {
            IPrefab prefab = PrefabFactory.CreatePrefab(type);
            PrefabRepository.AddPrefab(prefab);

            var prefabModel = ControlMessenger.Mapper.Map<IPrefabModel>(prefab);

            if (SelectedItem is EmptyPrefab emptyPrefab)
            {
                Prefabs[Prefabs.IndexOf(SelectedItem)] = prefab;
                Send(new MessageReplaceEmptyPrefab(ID, prefabModel, emptyPrefab));
            }
            else
            {
                Prefabs.Add(prefab);
                Send(new MessageAddItem(ID, prefabModel));
            }

            SelectedItem = prefab;
        }

        public void AddItem(IControlModel controlModel)
        {
            if (controlModel is not IPrefabModel prefabModel)
                return;

            IPrefab prefab = PrefabFactory.CreatePrefab((IPrefabModel)controlModel);
            PrefabRepository.AddPrefab(prefab);

            if (SelectedItem is EmptyPrefab)
                Prefabs[Prefabs.IndexOf(SelectedItem)] = prefab;
            else
                Prefabs.Add(prefab);
        }

        public void AddEmptyPrefab()
        {
            IPrefab prefab = new EmptyPrefab();
            Prefabs.Add(prefab);
            SelectedIndex = Prefabs.Count - 1;
            Send(new MessageAddItem(ID, ControlMessenger.Mapper.Map<IPrefabModel>(prefab)));
        }

        public void ReplaceEmptyPrefab(Guid emptyPrefabID, IControlModel controlModel)
        {
            var emptyPrefab = Prefabs.FirstOrDefault(x => x.ID == emptyPrefabID);
            int index = Prefabs.IndexOf(emptyPrefab);
            IPrefab prefab = Prefabs.ToList().Find(x => x.ID == emptyPrefabID);

            if (prefab is not EmptyPrefab)
                return;

            prefab = PrefabFactory.CreatePrefab((IPrefabModel)controlModel);
            Prefabs[index] = prefab;
            PrefabRepository.AddPrefab(prefab);
        }


        partial void OnSelectedItemChanged(IPrefab oldValue, IPrefab newValue)
        {
            if (newValue == null)
                return;

            if (SelectedItem == null)
                return;

            if (Prefabs.Count == 0)
                return;

            if (SelectedIndex < 0)
                return;

            if (SelectedIndex >= Prefabs.Count)
                return;

            var index = SelectedIndex;

            Prefabs[index] = newValue;

            SelectedIndex = index;
            SelectedItem = newValue;

            Send(new MessageSelectedItemChanged(ID, newValue.ID, index));
        }

        public void SelectedItemChanged(Guid selectedItemID, int index)
        {
            var prefab = PrefabRepository.GetPrefab(selectedItemID);

            if (prefab == null)
                return;

            if (index < 0)
                return;

            if (Prefabs.Count == 0)
                return;

            if (index >= Prefabs.Count)
                return;

            Prefabs[index] = prefab;

            SelectedIndex = index;
            SelectedItem = prefab;
        }


        public virtual void DeleteItem(IPrefab prefab)
        {
            var index = SelectedIndex;

            if (prefab == null)
                return;

            Prefabs.RemoveAt(index);
            Send(new MessageRemoveItem(ID, prefab));

            if (Prefabs.Count == 0)
            {
                SelectedItem = null;
                SelectedIndex = -1;
                return;
            }

            if (index == 0)
            {
                SelectedItem = Prefabs[0];
                return;
            }

            if (index > 0)
            {
                SelectedIndex = index - 1;
                return;
            }
        }

        public virtual void DeleteItem(Guid id)
        {
            var prefab = Prefabs.FirstOrDefault(x => x.ID == id);

            if (prefab == null)
                return;

            Prefabs.Remove(prefab);
        }

        public void MoveItem(int oldIndex, int newIndex)
        {
            if (Prefabs.Count == 0)
                return;

            Prefabs.Move(oldIndex, newIndex);
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
    }
}
