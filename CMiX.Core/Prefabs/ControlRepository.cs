// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Data;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Networking.Messenger;
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
            Compositions = new ObservableCollection<Composition>();
            Materials = new ObservableCollection<Material>();
            Textures = new ObservableCollection<ITextureSource>();
            Entities = new ObservableCollection<Entity>();
            Cameras = new ObservableCollection<Camera>();
            Lights = new ObservableCollection<LightEntity>();
            Servers = new ObservableCollection<Server>();
            BeatModifiers = new ObservableCollection<BeatModifier>();
            //BindingOperations.EnableCollectionSynchronization(Controls, this);
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

        public IControl GetControl(Guid id)
        {
            return Controls.FirstOrDefault(x => x.ID == id);
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

            if (control is Composition composition)
                Compositions.Add(composition);

            if (control is ITextureSource texture)
                Textures.Add(texture);

            if(control is Camera camera)
                Cameras.Add(camera);

            if (control is LightEntity lightEntity)
                Lights.Add(lightEntity);

            if (control is Entity entity)
                Entities.Add(entity);

            if (control is Material material)
                Materials.Add(material);

            if (control is Server server)
                Servers.Add(server);

            if (control is BeatModifier beatModifier)
                BeatModifiers.Add(beatModifier);
        }


        private ObservableCollection<Server> _servers;
        public ObservableCollection<Server> Servers
        {
            get => _servers;
            set => SetProperty(ref _servers, value);
        }


        private ObservableCollection<Composition> _compositions;
        public ObservableCollection<Composition> Compositions
        {
            get => _compositions;
            set => SetProperty(ref _compositions, value);
        }

        private ObservableCollection<ITextureSource> _textures;
        public ObservableCollection<ITextureSource> Textures
        {
            get => _textures;
            set => SetProperty(ref _textures, value);
        }

        private ObservableCollection<Material> _materials;
        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set => SetProperty(ref _materials, value);
        }

        private ObservableCollection<Entity> _entities;
        public ObservableCollection<Entity> Entities
        {
            get => _entities;
            set => SetProperty(ref _entities, value);
        }

        private ObservableCollection<Camera> _cameras;
        public ObservableCollection<Camera> Cameras
        {
            get => _cameras;
            set => SetProperty(ref _cameras, value);
        }

        private ObservableCollection<LightEntity> _lights;
        public ObservableCollection<LightEntity> Lights
        {
            get => _lights;
            set => SetProperty(ref _lights, value);
        }

        private ObservableCollection<BeatModifier> _beatModifiers;
        public ObservableCollection<BeatModifier> BeatModifiers
        {
            get => _beatModifiers;
            set => SetProperty(ref _beatModifiers, value);
        }
    }
}
