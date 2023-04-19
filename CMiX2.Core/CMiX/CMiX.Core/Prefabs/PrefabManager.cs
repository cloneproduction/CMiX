// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml.Linq;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Message;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs
{
    public class PrefabManager<T> : ObservableRecipient, IPrefabManager, IControl, IRecipient<MessageRequestControl> where T : class, IPrefab
    {
        public PrefabManager(PrefabManagerModel prefabManagerModel, CompositionService compositionService)
        {
            ID = prefabManagerModel.ID;
            PrefabFactory = compositionService.PrefabFactory;
            Prefabs = new ObservableCollection<IPrefab>();
            //SelectionChangedCommand = new RelayCommand<IPrefab>(ChangeSelectedItem);
            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            //AddItemToContainerCommand = new RelayCommand<IPrefab>(AddItemToContainer);
            AddItemCommand = new RelayCommand(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyItem);
            DeleteItemCommand = new RelayCommand<IPrefab>(DeleteItem);
            RenameCommand = new RelayCommand(Rename);
            IsActive = true;
        }

        public PrefabManager(Guid id, CompositionService compositionService)
        {
            ID = id;
            PrefabFactory = new PrefabFactory(compositionService);
            Prefabs = new ObservableCollection<IPrefab>();
            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            //AddItemToContainerCommand = new RelayCommand<IPrefab>(AddItemToContainer);
            AddItemCommand = new RelayCommand(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyItem);
            DeleteItemCommand = new RelayCommand<IPrefab>(DeleteItem);
            RenameCommand = new RelayCommand(Rename);
            IsActive = true;
        }

        public PrefabManager(Guid id, CompositionService compositionService, PrefabRepository<T> prefabRepository) : this(id, compositionService)
        {
            PrefabRepository = prefabRepository;
        }


        public Guid ID { get; set; }
        public PrefabFactory PrefabFactory { get; set; }
        public PrefabRepository<T> PrefabRepository { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand ItemUpCommand { get; set; }
        public ICommand ItemDownCommand { get; set; }
        public ICommand AddItemToContainerCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand AddEmptyItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RenameCommand { get; }

        int nameCount = 0;

        private ObservableCollection<IPrefab> _prefabs;
        public ObservableCollection<IPrefab> Prefabs
        {
            get => _prefabs;
            set => SetProperty(ref _prefabs, value);
        }

        private IPrefab _selectedItem;
        public IPrefab SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                Send(new MessageSelectPrefab(ID, SelectedItem));
            }
        }

        public void SelectPrefab(Guid prefabID)
        {
            //SelectedItem = GetPrefab(prefabID);
        }

        public void ItemUp()
        {
            var currentIndex = Prefabs.IndexOf(SelectedItem);

            if (currentIndex <= 0)
                return;

            Prefabs.Move(currentIndex, currentIndex - 1);
            Send(new MessageMovePrefab(ID, currentIndex, currentIndex - 1));
        }

        public void ItemDown()
        {
            var currentIndex = Prefabs.IndexOf(SelectedItem);

            if (currentIndex == Prefabs.Count - 1)
                return;

            Prefabs.Move(currentIndex, currentIndex + 1);
            Send(new MessageMovePrefab(ID, currentIndex, currentIndex + 1));
        }

        public IPrefab CreateEmptyPrefab()
        {
            IPrefab emptyPrefab = new EmptyPrefab();
            Prefabs.Add(emptyPrefab);
            SelectedItem = emptyPrefab;
            return emptyPrefab;
        }

        public IPrefab CreatePrefab()
        {
            var prefab = PrefabFactory.CreatePrefab(typeof(T));
            prefab.Name.Value = prefab.GetType().Name + "." + nameCount.ToString("000");
            nameCount++;

            prefab.IsSelected.Value = true;
            PrefabRepository?.AddPrefab((T)prefab);

            Prefabs.Add(prefab);
            SelectedItem = prefab;

            return prefab;
        }

        public void AddPrefab(IPrefabModel prefabModel)
        {
            if (prefabModel == null)
                return;

            var prefab = PrefabFactory.CreatePrefab(prefabModel);
            prefab.Name.Value = prefab.GetType().Name + "." + nameCount.ToString("000");
            nameCount++;

            prefab.IsSelected.Value = true;
            PrefabRepository?.AddPrefab((T)prefab);
            Prefabs.Add(prefab);
            SelectedItem = prefab;
        }

        //public void AddItemToContainer(IPrefab prefabContainer)
        //{
        //    var prefab = PrefabFactory.CreatePrefab(typeof(T));
        //    prefab.Name.Value = prefab.GetType().Name + "." + nameCount.ToString("000");
        //    nameCount++;
        //    prefab.IsSelected.Value = true;
        //    PrefabRepository?.AddPrefab((T)prefab);

        //    SelectedItem = prefab;

        //    var containerModel = ControlMessenger.Mapper.Map<IPrefabModel>(prefabContainer);
        //    Send(new MessageAddPrefab(ID, containerModel));
        //}


        public virtual void AddItem()
        {
            var prefab = CreatePrefab();
            var prefabModel = ControlMessenger.Mapper.Map<IPrefabModel>(prefab);
            Send(new MessageAddPrefab(ID, prefabModel));
        }



        public void AddEmptyItem()
        {
            var prefab = new EmptyPrefab(new EmptyPrefabModel());
            var containerModel = ControlMessenger.Mapper.Map<IPrefabModel>(prefab);
            Send(new MessageAddPrefab(ID, containerModel));
        }


        public void Send(IMessage message)
        {
            WeakReferenceMessenger.Default.Send(message, MessageType.Out);
        }


        public virtual void DeleteItem(IPrefab prefab)
        {
            var index = Prefabs.IndexOf(prefab);

            if (prefab == null)
                return;

            Prefabs.Remove(prefab);
            Send(new MessageRemovePrefab(ID, prefab));

            if (Prefabs.Count == 0)
            {
                SelectedItem = null;
                return;
            }

            if (index == 0)
            {
                SelectedItem = Prefabs[0];
                return;
            }

            if (index > 0)
            {
                SelectedItem = Prefabs[index - 1];
                return;
            }
        }

        public void DeleteItem(Guid id)
        {
            var prefab = Prefabs.FirstOrDefault(x => x.ID == id);

            if (prefab == null)
                return;

            Prefabs.Remove(prefab);
            PrefabRepository?.RemovePrefab((T)prefab);
        }

        public void ChangePrefab(Guid containerID, Guid prefabID)
        {
            var prefabContainer = Prefabs.FirstOrDefault(x => x.ID == containerID);
            //prefabContainer.Prefab = WeakReferenceMessenger.Default.Send(new MessageRequestPrefab(prefabID), MessageType.Internal).Response;
        }

        public void MovePrefab(int oldIndex, int newIndex)
        {
            if (Prefabs.Count <= 0)
                return;

            Prefabs.Move(oldIndex, newIndex);
        }

        public void Rename() => SelectedItem.IsRenaming.Value = true;

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == ID && !message.HasReceivedResponse)
                message.Reply(this);
        }

        public IPrefab GetPrefab(Guid guid)
        {
            return Prefabs.FirstOrDefault(x => x.ID == guid);
        }

        //public void PrefabContainer_PrefabChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    var prefabContainer = sender as IPrefab;
        //    var prefabID = prefabContainer.ID;
        //    Send(new MessagePrefabContainerChanged(ID, prefabContainer.ID, prefabID));
        //}
    }
}
