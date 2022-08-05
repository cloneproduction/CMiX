// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class PrefabManager<T> : ObservableRecipient, IRecipient<IMessage>, IControl, IBeatable
    {
        public PrefabManager(IPrefabManagerModel prefabManagerModel)
        {
            ID = prefabManagerModel.ID;
            PrefabFactory = new PrefabFactory();

            Prefabs = new ObservableCollection<IPrefab>();

            AddItemCommand = new RelayCommand(AddItem);
            DeleteItemCommand = new RelayCommand(DeleteItem);
            RenameCommand = new RelayCommand(Rename);

            WeakReferenceMessenger.Default.Register(this, MessageType.In);
        }


        public PrefabFactory PrefabFactory { get; set; }

        public Guid ID { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RenameCommand { get; }

        public ObservableCollection<IPrefab> Prefabs { get; set; }


        private IPrefab _selectedItem;
        public IPrefab SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }


        public MasterBeat MasterBeat { get; set; }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
        }

        public IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            IPrefab prefab = PrefabFactory.CreatePrefab(prefabModel);

            prefab.Name = prefab.GetType().Name;
            SelectedItem = prefab;
            prefab.IsSelected = true;
            Prefabs.Add(prefab);
            prefab.SetMasterBeat(MasterBeat);

            return prefab;
        }
         

        public IPrefab CreatePrefab()
        {
            IPrefab prefab = PrefabFactory.CreatePrefab(typeof(T));

            prefab.Name = prefab.GetType().Name;
            SelectedItem = prefab;
            prefab.IsSelected = true;
            Prefabs.Add(prefab);
            prefab.SetMasterBeat(MasterBeat);

            return prefab;
        }

        public void AddItem()
        {
            IPrefab item = CreatePrefab();
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddPrefab(this.ID, item), MessageType.Out);
        }

        public void AddItem(IPrefabModel prefabModel)
        {
            if (prefabModel == null)
                return;

            CreatePrefab(prefabModel);
        }


        public void DeleteItem()
        {
            var index = Prefabs.IndexOf(SelectedItem);

            if (SelectedItem == null)
                return;

            Prefabs.Remove(SelectedItem);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemovePrefab(this.ID, SelectedItem), MessageType.Out);

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
            var entity = Prefabs.FirstOrDefault(x => x.ID == id);

            if (entity == null)
                return;

            Prefabs.Remove(entity);
        }


        public void Rename() => SelectedItem.IsRenaming = true;

        public void Receive(IMessage message)
        {
            if (message.ID != ID)
                return;

            if (message is MessageAddPrefab messageAddPrefab)
            {
                this.AddItem(messageAddPrefab.PrefabModel as IPrefabModel);
            }

            if (message is MessageRemoveEntity messageRemoveEntity)
            {
                this.DeleteItem(messageRemoveEntity.EntityID);
            }
        }


        public void SetViewModel(IModel model)
        {
            IPrefabManagerModel materialManagerModel = model as IPrefabManagerModel;
            ID = materialManagerModel.ID;
        }

        public IModel GetModel()
        {
            IPrefabManagerModel modifierManagerModel = new PrefabManagerModel();
            modifierManagerModel.ID = this.ID;
            return modifierManagerModel;
        }
    }
}
