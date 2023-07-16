// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
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
    public partial class PrefabManager : ObservableRecipient, IPrefabManager, IRecipient<MessageRequestControl>
    {
        public PrefabManager(CompositionService compositionService)// : this()
        {
            Prefabs = new ObservableCollection<IPrefab>();
            SelectionChangedCommand = new RelayCommand<IPrefab>(ReplaceItem);
            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            AddItemCommand = new RelayCommand<Type>(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyPrefab);
            DeleteItemCommand = new RelayCommand<IPrefab>(DeleteItem);
            IsActive = true;

            PrefabRepository = compositionService.PrefabRepository;
            PrefabFactory = new PrefabFactory(compositionService);

            //PrefabCollectionView = CollectionViewSource.GetDefaultView(compositionService.PrefabRepository.Prefabs);
            //PrefabCollectionView.Filter = FilterPrefab;
        }

        //public ICollectionView PrefabCollectionView { get; }

        //public Type FilterType { get; set; } 

        //private bool FilterPrefab(object obj)
        //{
        //    if(obj.GetType() == FilterType)
        //        return true;

        //    return false;
        //}

        public PrefabManager(Guid id, CompositionService compositionService) : this(compositionService)
        {
            ID = id;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabFactory PrefabFactory { get; set; }
        public PrefabRepository PrefabRepository { get; set; }

        public ICommand SelectionChangedCommand { get; set; }
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
            PrefabRepository?.AddPrefab(prefab);

            if (SelectedItem is EmptyPrefab)
                Prefabs[Prefabs.IndexOf(SelectedItem)] = prefab;
            else
                Prefabs.Add(prefab);

            SelectedItem = prefab;
            Send(new MessageAddItem(ID, ControlMessenger.Mapper.Map<IPrefabModel>(prefab)));
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


        public void ReplaceItem(IPrefab prefab)
        {
            if (prefab == null)
                return;

            if (Prefabs.Count <= 0)
            {
                Prefabs.Add(prefab);
                SelectedItem = prefab;
                return;
            }

            var oldPrefab = Prefabs[SelectedIndex];
            Prefabs[SelectedIndex] = prefab;
            SelectedItem = prefab;

            Send(new MessageReplaceItem(ID, oldPrefab.ID, prefab.ID));
        }

        public void ReplaceItem(Guid oldPrefabID, Guid newPrefabID)
        {
            var prefab = PrefabRepository.GetPrefab(newPrefabID);

            if (prefab == null)
                return;

            if (Prefabs.Count <= 0)
            {
                Prefabs.Add(prefab);
                SelectedItem = prefab;
                return;
            }

            var selected = Prefabs.FirstOrDefault(x => x.ID == oldPrefabID);
            var index = Prefabs.IndexOf(selected);

            Prefabs[index] = prefab;
            SelectedItem = prefab;
        }

        public void AddItem(IControlModel controlModel)
        {
            if(controlModel is IPrefabModel prefabModel)
            {
                IPrefab prefab = PrefabFactory.CreatePrefab(prefabModel);

                if (prefab is not EmptyPrefab)
                    PrefabRepository?.AddPrefab(prefab);

                Prefabs.Add(prefab);
                SelectedItem = prefab;
            }
        }

        public void AddEmptyPrefab()
        {
            IPrefab prefab = new EmptyPrefab();
            Prefabs.Add(prefab);
            SelectedItem = prefab;
            Send(new MessageAddItem(ID, ControlMessenger.Mapper.Map<IPrefabModel>(prefab)));
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

        public IPrefab GetPrefab(Guid guid)
        {
            return Prefabs.FirstOrDefault(x => x.ID == guid);
        }
    }
}
