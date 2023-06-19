// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
    public partial class PrefabManager<T> : ObservableRecipient, IPrefabManager, IControl, IRecipient<MessageRequestControl> where T : class, IPrefab
    {
        public PrefabManager(PrefabManagerModel prefabManagerModel, CompositionService compositionService)
        {
            ID = prefabManagerModel.ID;
            PrefabFactory = compositionService.PrefabFactory;
            Prefabs = new ObservableCollection<IPrefab>();
            SelectionChangedCommand = new RelayCommand<IPrefab>(SelectionChanged);
            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            AddItemCommand = new RelayCommand(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyPrefab);
            DeleteItemCommand = new RelayCommand<IPrefab>(DeleteItem);
            IsActive = true;
        }

        public PrefabManager(PrefabManagerModel prefabManagerModel, CompositionService compositionService, PrefabRepository<T> prefabRepository) : this(prefabManagerModel, compositionService)
        {
            PrefabRepository = prefabRepository;
        }

        public PrefabManager(Guid id, CompositionService compositionService)
        {
            ID = id;
            PrefabFactory = new PrefabFactory(compositionService);
            Prefabs = new ObservableCollection<IPrefab>();
            SelectionChangedCommand = new RelayCommand<IPrefab>(SelectionChanged);
            ItemUpCommand = new RelayCommand(ItemUp);
            ItemDownCommand = new RelayCommand(ItemDown);
            AddItemCommand = new RelayCommand(AddItem);
            AddEmptyItemCommand = new RelayCommand(AddEmptyPrefab);
            DeleteItemCommand = new RelayCommand<IPrefab>(DeleteItem);
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
            Send(new MessageMovePrefab(ID, index, index - 1));
        }

        public void ItemDown()
        {
            var index = SelectedIndex;

            if (index == Prefabs.Count - 1)
                return;

            Prefabs.Move(index, index + 1);
            Send(new MessageMovePrefab(ID, index, index + 1));
        }

        public virtual void AddItem()
        {
            IPrefab prefab = PrefabFactory.CreatePrefab(typeof(T));
            PrefabRepository?.AddPrefab((T)prefab);

            if (SelectedItem is EmptyPrefab)
                Prefabs[Prefabs.IndexOf(SelectedItem)] = prefab;
            else
                Prefabs.Add(prefab);

            SelectedItem = prefab;
            Send(new MessageAddPrefab(ID, ControlMessenger.Mapper.Map<IPrefabModel>(prefab)));
        }

        public virtual void DeleteItem(IPrefab prefab)
        {
            var index = SelectedIndex;

            if (prefab == null)
                return;

            Prefabs.RemoveAt(index);
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
            Console.WriteLine("Item oftype" + prefab.GetType().ToString() + " Deleted");
        }

        public void SelectionChanged(IPrefab prefab)
        {
            //if (prefab == null)
            //    return;

            //if (Prefabs.Count <= 0)
            //{
            //    Prefabs.Add(prefab);
            //    SelectedItem = prefab;
            //    return;
            //}
            //var selectedPrefab = Prefabs[SelectedIndex];
            //Prefabs[SelectedIndex] = prefab;
            //SelectedItem = prefab;
            //Send(new MessageSelectedPrefabChanged(ID, selectedPrefab.ID, prefab.ID));
        }

        public void SelectionChanged(Guid selectedPrefabID, Guid newPrefabID)
        {
            //var prefab = PrefabRepository.GetPrefab(newPrefabID);

            //var selected = Prefabs.FirstOrDefault(x => x.ID == selectedPrefabID);
            //var index = Prefabs.IndexOf(selected);

            //if (prefab == null)
            //    return;

            //if (Prefabs.Count <= 0)
            //{
            //    Prefabs.Add(prefab);
            //    SelectedItem = prefab;
            //    return;
            //}

            //Prefabs[index] = prefab;
            //SelectedItem = prefab;
        }

        public void AddPrefab(IPrefabModel prefabModel)
        {
            if (prefabModel == null)
                return;

            IPrefab prefab = PrefabFactory.CreatePrefab(prefabModel);

            if(prefab is not EmptyPrefab)
                PrefabRepository?.AddPrefab((T)prefab);

            Prefabs.Add(prefab);
            SelectedItem = prefab;
        }

        public void AddEmptyPrefab()
        {
            IPrefab prefab = new EmptyPrefab();
            Prefabs.Add(prefab);
            SelectedItem = prefab;
            Send(new MessageAddPrefab(ID, ControlMessenger.Mapper.Map<IPrefabModel>(prefab)));
        }

        public void MovePrefab(int oldIndex, int newIndex)
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
