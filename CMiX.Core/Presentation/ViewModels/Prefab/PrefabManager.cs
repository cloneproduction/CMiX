// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class PrefabManager<T> : ObservableRecipient, IPrefabManager, IControl, IRecipient<MessageRequestControl> where T : class, IPrefab
    {
        public PrefabManager(PrefabManagerModel prefabManagerModel, PrefabFactory prefabFactory)
        {
            ID = prefabManagerModel.ID;
            PrefabFactory = prefabFactory;
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

        public PrefabManager(Guid id, PrefabFactory prefabFactory)
        {
            ID = id;
            PrefabFactory = prefabFactory;
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

        public PrefabManager(Guid id, PrefabFactory prefabFactory, PrefabRepository<T> prefabRepository) : this(id, prefabFactory)
        {
            PrefabRepository = prefabRepository;
        }


        public Guid ID { get; set; }

        public PrefabFactory PrefabFactory { get; set; }
        public PrefabRepository<T> PrefabRepository { get; set; }

        public ICommand ItemUpCommand { get; set; }
        public ICommand ItemDownCommand { get; set; }
        public ICommand AddItemToContainerCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand AddEmptyItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RenameCommand { get; }


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
            var container = GetPrefab(containerID);//.Prefabs.FirstOrDefault(x => x.ID == messageAddPrefab.ContainerID);

            if (container == null)
                container = new PrefabContainer(new PrefabContainerModel(containerID));

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

            SelectedItem.Prefab = (T)prefab;
            SelectedItem.PrefabChanged += PrefabContainer_PrefabChanged;
        }

        public virtual void AddItem()
        {
            PrefabContainer prefab = CreatePrefabContainer();
            Send(new MessageAddPrefab(this.ID, prefab));
        }


        int nameCount = 0;
        public void AddItem(Guid containerID, IPrefabModel prefabModel)
        {
            var container = GetPrefab(containerID);

            IPrefab prefab = PrefabFactory.CreatePrefab(prefabModel);
            prefab.Name = prefab.GetType().Name + "." + nameCount.ToString("000");
            nameCount++;

            prefab.IsSelected = true;
            PrefabRepository?.AddPrefab((T)prefab);
            ((PrefabContainer)container).Prefab = (T)prefab;
        }

        public void AddEmptyItem()
        {
            PrefabContainer prefab = CreateEmptyPrefabContainer();
            Send(new MessageAddPrefab(this.ID, prefab));
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

            Prefabs.Remove(prefab);
            PrefabRepository?.RemovePrefab((T)prefab.Prefab);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemovePrefab(this.ID, prefab), MessageType.Out);

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

        public void SetViewModel(IModel model)
        {
            PrefabManagerModel prefabManagerModel = model as PrefabManagerModel;
            this.ID = prefabManagerModel.ID;
        }

        public IModel GetModel()
        {
            PrefabManagerModel prefabManagerModel = new PrefabManagerModel();
            prefabManagerModel.ID = this.ID;
            return prefabManagerModel;
        }

        public IPrefab GetPrefab(Guid guid)
        {
            return Prefabs.FirstOrDefault(x => x.ID == guid);
        }
    }
}
