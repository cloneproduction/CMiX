// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Data;
using CMiX.Core.Compositing;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository : ObservableObject
    {
        public ControlRepository()
        {
            Controls = new ObservableCollection<IControl>();
            Textures = new ObservableCollection<ITextureSource>();
            Entities = new ObservableCollection<IPrefab>();
            Cameras = new ObservableCollection<IPrefab>();
            Lights = new ObservableCollection<IPrefab>();
            BindingOperations.EnableCollectionSynchronization(Controls, this);
        }

        private int nameCount = 1;

        private ObservableCollection<IControl> _controls;
        public ObservableCollection<IControl> Controls
        {
            get => _controls;
            set => SetProperty(ref _controls, value);
        }


        public void AddControl(IControl control)
        {
            if (control == null)
                return;

            if(control is EmptyPrefab) 
                return;

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

            if (control is ITextureSource)
                Textures.Add(prefab as ITextureSource);

            if(control is Camera)
                Cameras.Add(prefab);

            if (control is LightEntity)
                Lights.Add(prefab);

            if (control is Entity)
                Entities.Add(prefab);
        }



        private ObservableCollection<ITextureSource> _textures;
        public ObservableCollection<ITextureSource> Textures
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

        private ObservableCollection<IPrefab> _cameras;
        public ObservableCollection<IPrefab> Cameras
        {
            get => _cameras;
            set => SetProperty(ref _cameras, value);
        }

        private ObservableCollection<IPrefab> _lights;
        public ObservableCollection<IPrefab> Lights
        {
            get => _lights;
            set => SetProperty(ref _lights, value);
        }

        public IControl GetPrefab(Guid id)
        {
            return Controls.FirstOrDefault(x => x.ID == id);
        }
    }
}
