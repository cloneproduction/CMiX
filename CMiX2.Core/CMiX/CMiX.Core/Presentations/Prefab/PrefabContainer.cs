// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class PrefabContainer : ObservableObject, IPrefabContainer
    {
        public PrefabContainer(PrefabContainerModel prefabContainerModel)
        {
            this.ID = prefabContainerModel.ID;
        }


        public event PropertyChangedEventHandler PrefabChanged;
        protected void OnPrefabChanged([CallerMemberName] string prefab = null)
        {
            PrefabChanged?.Invoke(this, new PropertyChangedEventArgs(prefab));
        }


        public Guid ID { get; set; }


        private IPrefab _prefab;
        public IPrefab Prefab
        {
            get => _prefab;
            set
            {
                SetProperty(ref _prefab, value);
                OnPropertyChanged("Name");
                OnPrefabChanged();
            }
        }

        public bool IsRenaming
        {
            get => Prefab != null ? Prefab.IsRenaming : false;
            set
            {
                if(Prefab != null)
                    Prefab.IsRenaming = value;
            }
        }

        public string Name
        {
            get => Prefab?.Name;
            set
            {
                if (Prefab != null)
                    Prefab.Name = value;
            }
        }

        public bool IsSelected
        {
            get => Prefab != null ? Prefab.IsSelected : false;
            set
            {
                if (Prefab != null)
                    Prefab.IsSelected = value;
            }
        }


        public void SetViewModel(IModel model)
        {
            PrefabContainerModel prefabSlotModel = model as PrefabContainerModel;
            this.ID = prefabSlotModel.ID;

            if(Prefab != null)
                Prefab.SetViewModel(prefabSlotModel);
        }

        public IModel GetModel()
        {
            PrefabContainerModel prefabSlotModel = new PrefabContainerModel();
            prefabSlotModel.ID = ID;

            if (Prefab != null)
                prefabSlotModel.PrefabModel = (IPrefabModel)Prefab.GetModel();

            return prefabSlotModel;
        }
    }
}
