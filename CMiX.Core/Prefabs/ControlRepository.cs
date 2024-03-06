// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Data;
using CMiX.Core.Compositing;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository : ObservableObject
    {
        public ControlRepository()
        {
            Controls = new ObservableCollection<IControl>();
            Textures = new ObservableCollection<IPrefab>();
            Entities = new ObservableCollection<IPrefab>();
            BindingOperations.EnableCollectionSynchronization(Controls, this);
        }

        private int nameCount = 1;
        //public ObservableCollection<IControl> Controls { get; set; }

        private ObservableCollection<IControl> _controls;
        public ObservableCollection<IControl> Controls
        {
            get => _controls;
            set => SetProperty(ref _controls, value);
        }


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
            if (Controls.Any(x => x.ID == control.ID))
                return;

            Controls.Add(control);
            NamePrefab(control);
            AddToSpecificRepo(control);
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


        void AddToSpecificRepo(IControl control)
        {
            var prefab = control as IPrefab;

            if (control is Texture)
                Textures.Add(prefab);

            if(control is Camera || control is LightEntity || control is Entity)
                Entities.Add(prefab);
        }

        private ObservableCollection<IPrefab> _textures;
        public ObservableCollection<IPrefab> Textures
        {
            get => _textures;
            set => SetProperty(ref _textures, value);
        }

        private ObservableCollection<IPrefab> _entities;
        public ObservableCollection<IPrefab> Entities
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
