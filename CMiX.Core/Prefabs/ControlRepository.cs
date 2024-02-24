// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CMiX.Core.Compositing;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository :ObservableObject
    {
        public ControlRepository()
        {
            Controls = new ObservableCollection<IControl>();
            Textures = CollectionViewSource.GetDefaultView(Controls);
            Textures.Filter = new Predicate<object>(this.FilterTexture);

            Entities = CollectionViewSource.GetDefaultView(Controls);
            Entities.Filter = new Predicate<object>(this.FilterEntities);

            BindingOperations.EnableCollectionSynchronization(Controls, this);
        }


        private int nameCount = 1;
        public ObservableCollection<IControl> Controls { get; set; }


        public bool FilterTexture(object item)
        {
            if(item is Texture)
                return true;

            return false;
        }

        public bool FilterEntities(object item)
        {
            if (item is Camera || item is LightEntity || item is Entity)
                return true;

            return false;
        }


        public void AddControl(IControl control)
        {
            Controls.Add(control);
            NamePrefab(control);
            Console.WriteLine("PrefabRepository Count is " + Controls.Count().ToString());
        }

        void NamePrefab(IControl control)
        {
            if(control is IPrefab prefab)
            {
                var prefabs = Controls.OfType<IPrefab>().ToList();

                if(prefabs.FirstOrDefault(x => x.PrefabService.Name.Value == prefab.PrefabService.Name.Value) != null)
                {
                    prefab.PrefabService.Name.Value = control.GetType().Name + "." + string.Format("{0:000}", nameCount);
                    nameCount++;
                }
            }
        }

        private ICollectionView _textures;
        public ICollectionView Textures
        {
            get => _textures;
            set => SetProperty(ref _textures, value);
        }

        private ICollectionView _entities;
        public ICollectionView Entities
        {
            get => _entities;
            set => SetProperty(ref _entities, value);
        }

        public IControl GetPrefab(Guid id)
        {
            return Controls.FirstOrDefault(x => x.ID == id);
        }
    }
}
