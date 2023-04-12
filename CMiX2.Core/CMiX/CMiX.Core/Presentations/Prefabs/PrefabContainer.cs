// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Prefabs
{
    public class PrefabContainer : ObservableObject, IPrefabContainer
    {
        public PrefabContainer(PrefabContainerModel prefabContainerModel, CompositionService compositionService)
        {
            this.ID = prefabContainerModel.ID;
            IsRenaming = new BooleanValue(prefabContainerModel.IsRenaming, compositionService);
            IsSelected = new BooleanValue(prefabContainerModel.IsSelected, compositionService);
            Name = new StringValue(prefabContainerModel.Name, compositionService);
        }


        public event PropertyChangedEventHandler PrefabChanged;
        protected void OnPrefabChanged([CallerMemberName] string? prefab = null)
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

        public BooleanValue IsRenaming
        {
            get => Prefab.IsRenaming;
            set
            {
                if(Prefab != null)
                    Prefab.IsRenaming = value;
            }
        }

        public StringValue Name
        {
            get => Prefab.Name;
            set
            {
                if (Prefab != null)
                    Prefab.Name = value;
            }
        }

        public BooleanValue IsSelected
        {
            get => Prefab.IsSelected;
            set
            {
                if (Prefab != null)
                    Prefab.IsSelected = value;
            }
        }
    }
}
