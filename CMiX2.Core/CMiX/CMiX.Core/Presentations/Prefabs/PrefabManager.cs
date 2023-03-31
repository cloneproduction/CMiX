// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.Prefabs
{
    public class PrefabManager<T> : ObservableRecipient, IPrefabManager, IControl, IRecipient<MessageRequestControl> where T : class, IPrefab
    {
        public PrefabManager(PrefabManagerModel prefabManagerModel, CompositionService compositionService)
        {
            ID = prefabManagerModel.ID;
            CompositionService = compositionService;
            PrefabFactory = compositionService.PrefabFactory;
            Prefabs = new ObservableCollection<PrefabContainer>();

            SelectionChangedCommand = new RelayCommand<IPrefab>(ChangeSelectedItem);

            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            AddItemToContainerCommand = new RelayCommand<PrefabContainer>(AddItemToContainer);
            AddItemCommand = new RelayCommand(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyItem);
            DeleteItemCommand = new RelayCommand<PrefabContainer>(DeleteItem);
            RenameCommand = new RelayCommand(Rename);

            IsActive = true;
        }

        public PrefabManager(Guid id, CompositionService compositionService)
        {
            ID = id;
            CompositionService = compositionService;
            PrefabFactory = compositionService.PrefabFactory;
            Prefabs = new ObservableCollection<PrefabContainer>();

            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            AddItemToContainerCommand = new RelayCommand<PrefabContainer>(AddItemToContainer);
            AddItemCommand = new RelayCommand(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyItem);
            DeleteItemCommand = new RelayCommand<PrefabContainer>(DeleteItem);
            RenameCommand = new RelayCommand(Rename);

            IsActive = true;
        }

        public PrefabManager(Guid id, CompositionService compositionService, PrefabRepository<T> prefabRepository) : this(id, compositionService)
        {
            PrefabRepository = prefabRepository;
        }


        public Guid ID { get; set; }

        public CompositionService CompositionService { get; set; }
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


        public void ChangeSelectedItem(IPrefab prefab)
        {
            SelectedItem.Prefab = ((PrefabContainer)prefab).Prefab;
        }


        private ObservableCollection<PrefabContainer> _prefabs;
        public ObservableCollection<PrefabContainer> Prefabs
        {
            get => _prefabs;
            set => SetProperty(ref _prefabs, value);
        }

        private PrefabContainer _selectedItem;
        public PrefabContainer SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                Send(new MessageSelectPrefab(this.ID, SelectedItem));
            }
        }

        private T _comboBoxSelectedItem;
        public T ComboBoxSelectedItem
        {
            get => _comboBoxSelectedItem;
            set => SetProperty(ref _comboBoxSelectedItem, value);
        }


        public void SelectPrefab(Guid prefabID)
        {
            SelectedItem = GetPrefab(prefabID) as PrefabContainer;
        }

        public void ItemUp()
        {
            var currentIndex = Prefabs.IndexOf(SelectedItem);

            if (currentIndex <= 0)
                return;

            Prefabs.Move(currentIndex, currentIndex - 1);
            Send(new MessageMovePrefab(this.ID, currentIndex, currentIndex - 1));
        }

        public void ItemDown()
        {
            var currentIndex = Prefabs.IndexOf(SelectedItem);

            if (currentIndex == Prefabs.Count - 1)
                return;

            Prefabs.Move(currentIndex, currentIndex + 1);
            Send(new MessageMovePrefab(this.ID, currentIndex, currentIndex + 1));
        }


        public void PrefabContainer_PrefabChanged(object sender, PropertyChangedEventArgs e)
        {
            var prefabContainer = sender as PrefabContainer;
            if (prefabContainer.Prefab == null)
                return;

            var prefabID = prefabContainer.Prefab.ID;
            Send(new MessagePrefabContainerChanged(this.ID, prefabContainer.ID, prefabID));
        }


        public PrefabContainer CreateEmptyPrefabContainer()
        {
            PrefabContainer prefabContainer = new PrefabContainer(new PrefabContainerModel());
            prefabContainer.PrefabChanged += PrefabContainer_PrefabChanged;
            Prefabs.Add(prefabContainer);
            SelectedItem = prefabContainer;

            return prefabContainer;
        }

        public PrefabContainer CreatePrefabContainer()
        {
            IPrefab prefab = PrefabFactory.CreatePrefab(typeof(T));
            prefab.Name = prefab.GetType().Name + "." + nameCount.ToString("000");
            nameCount++;

            prefab.IsSelected = true;
            PrefabRepository?.AddPrefab((T)prefab);

            PrefabContainer prefabContainer = new PrefabContainer(new PrefabContainerModel());
            prefabContainer.Prefab = (T)prefab;
            prefabContainer.PrefabChanged += PrefabContainer_PrefabChanged;
            Prefabs.Add(prefabContainer);
            SelectedItem = prefabContainer;

            return prefabContainer;
        }

        public void AddPrefab(Guid containerID, IPrefabModel prefabModel)
        {
            bool addToContainer = false;

            var container = GetPrefab(containerID);

            if (container == null)
            {
                addToContainer = true;
                container = new PrefabContainer(new PrefabContainerModel(containerID));
            }

            if (prefabModel != null)
            {
                IPrefab prefab = PrefabFactory.CreatePrefab(prefabModel);
                prefab.Name = prefab.GetType().Name + "." + nameCount.ToString("000");
                nameCount++;

                prefab.IsSelected = true;
                PrefabRepository?.AddPrefab((T)prefab);
                ((PrefabContainer)container).Prefab = (T)prefab;
            }

            ((PrefabContainer)container).PrefabChanged += PrefabContainer_PrefabChanged;

            if(addToContainer)
                Prefabs.Add((PrefabContainer)container);

            SelectedItem = (PrefabContainer)container;
        }

        public void AddItemToContainer(PrefabContainer prefabContainer)
        {
            IPrefab prefab = PrefabFactory.CreatePrefab(typeof(T));
            prefab.Name = prefab.GetType().Name + "." + nameCount.ToString("000");
            nameCount++;
            prefab.IsSelected = true;
            PrefabRepository?.AddPrefab((T)prefab);

            prefabContainer.Prefab = (T)prefab;
            prefabContainer.PrefabChanged += PrefabContainer_PrefabChanged;
            SelectedItem = prefabContainer;

            var containerModel = CompositionService.Mapper.Map<IPrefabModel>(prefabContainer);
            Send(new MessageAddPrefab(this.ID, containerModel));
        }


        public virtual void AddItem()
        {
            PrefabContainer prefabContainer = CreatePrefabContainer();

            var containerModel = CompositionService.Mapper.Map<PrefabContainerModel>(prefabContainer);
            Send(new MessageAddPrefab(this.ID, containerModel.Prefab));
        }

        int nameCount = 0;

        public void AddEmptyItem()
        {
            PrefabContainer prefab = CreateEmptyPrefabContainer();
            var containerModel = CompositionService.Mapper.Map<IPrefabModel>(prefab);
            Send(new MessageAddPrefab(this.ID, containerModel));
        }


        public void Send(IMessage message)
        {
            WeakReferenceMessenger.Default.Send<IMessage, int>(message, MessageType.Out);
        }


        public virtual void DeleteItem(PrefabContainer prefab)
        {
            var index = Prefabs.IndexOf(prefab);

            if (prefab == null)
                return;

            prefab.PrefabChanged -= PrefabContainer_PrefabChanged;
            Prefabs.Remove(prefab);
            Send(new MessageRemovePrefab(this.ID, prefab));

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
            PrefabRepository?.RemovePrefab((T)prefab.Prefab);
        }


        public void ChangePrefab(Guid containerID, Guid prefabID)
        {
            var prefabContainer = Prefabs.FirstOrDefault(x => x.ID == containerID);
            prefabContainer.Prefab = WeakReferenceMessenger.Default.Send(new MessageRequestPrefab(prefabID), MessageType.Internal).Response;
        }

        public void MovePrefab(int oldIndex, int newIndex)
        {
            if (Prefabs.Count <= 0)
                return;

            Prefabs.Move(oldIndex, newIndex);
        }

        public void Rename() => SelectedItem.IsRenaming = true;

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }

        public IPrefab GetPrefab(Guid guid)
        {
            return Prefabs.FirstOrDefault(x => x.ID == guid);
        }
    }
}
