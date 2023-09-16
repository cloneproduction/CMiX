// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
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
    public partial class PrefabManager<T> : ObservableRecipient, IPrefabManager, IRecipient<MessageRequestControl>
    {
        public PrefabManager(CompositionService compositionService)
        {
            //ReplaceItemCommand = new RelayCommand<IPrefab>(ReplaceItem);
            SelectedItemChangedCommand = new RelayCommand<IPrefab>(SelectedItemChanged);
            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            AddItemCommand = new RelayCommand<Type>(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyPrefab);
            DeleteItemCommand = new RelayCommand<IPrefab>(DeleteItem);
            IsActive = true;

            PrefabRepository = compositionService.PrefabRepository;
            PrefabFactory = new PrefabFactory(compositionService);

            Prefabs = new ObservableCollection<IPrefab>(); //compositionService.PrefabRepository.GetRepository(typeof(T))
        }

        public PrefabManager(Guid id, CompositionService compositionService) : this(compositionService)
        {
            ID = id;
        }


        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabFactory PrefabFactory { get; set; }
        public PrefabRepository PrefabRepository { get; set; }

        public ICommand SelectedItemChangedCommand { get; set; }
        public ICommand ReplaceItemCommand { get; set; }
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
            SelectedItem = prefab;
            Send(new MessageAddItem(ID, ControlMessenger.Mapper.Map<IPrefabModel>(prefab)));
        }

        public void ReplaceEmptyPrefab(Guid emptyPrefabID, IControlModel controlModel)
        {
            var emptyPrefab = Prefabs.FirstOrDefault(x => x.ID == emptyPrefabID);
            int index = Prefabs.IndexOf(emptyPrefab);
            IPrefab prefab = Prefabs.ToList().Find(x => x.ID == emptyPrefabID);

            if (prefab is EmptyPrefab)
            {
                prefab = PrefabFactory.CreatePrefab((IPrefabModel)controlModel);
                Prefabs[index] = prefab;
                PrefabRepository.AddPrefab(prefab);
            }
        }




        public void SelectedItemChanged(IPrefab prefab)
        {
            if (prefab == null)
                return;

            if (SelectedItem == null)
                return;

            var index = SelectedIndex;

            Prefabs[index] = prefab;
            SelectedIndex = index;

            Send(new MessageSelectedItemChanged(ID, prefab.ID, index));
        }

        public void SelectedItemChanged(Guid selectedItemID, int index)
        {
            var prefab = PrefabRepository.GetPrefab(typeof(T), selectedItemID);

            if (prefab == null)
                return;

            Prefabs[index] = prefab;
            //SelectedIndex = index;
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


        //public void ReplaceItem(IPrefab prefab)
        //{
        //    //if (prefab == null)
        //    //    return;

        //    //if (Prefabs.Count <= 0)
        //    //{
        //    //    Prefabs.Add(prefab);
        //    //    SelectedItem = prefab;
        //    //    return;
        //    //}

        //    //var oldPrefab = Prefabs[SelectedIndex];
        //    //Prefabs[SelectedIndex] = prefab;
        //    //SelectedItem = prefab;

        //    //Send(new MessageReplaceItem(ID, oldPrefab.ID, prefab.ID));
        //}

        //public void ReplaceEmptyItem(Guid oldPrefabID, Guid newPrefabID)
        //{
        //    //var prefab = PrefabRepository.GetPrefab(typeof(T), newPrefabID);

        //    //if (prefab == null)
        //    //    return;

        //    //if (Prefabs.Count <= 0)
        //    //{
        //    //    Prefabs.Add(prefab);
        //    //    SelectedItem = prefab;
        //    //    return;
        //    //}

        //    //var selected = Prefabs.FirstOrDefault(x => x.ID == oldPrefabID);
        //    //var index = Prefabs.IndexOf(selected);

        //    //Prefabs[index] = prefab;
        //    //SelectedItem = prefab;
        //}



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
