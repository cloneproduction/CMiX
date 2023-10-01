// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Compositing;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing;

namespace CMiX.Core.Prefab
{
    public class PrefabRepository 
    {
        public PrefabRepository()
        {
            Compositions = new ObservableCollection<IPrefab>();
            Layers = new ObservableCollection<IPrefab>();
            Entities = new ObservableCollection<IPrefab>();
            Textures = new ObservableCollection<IPrefab>();
            Cameras = new ObservableCollection<IPrefab>();
            Lights = new ObservableCollection<IPrefab>();

            Repositories = new List<ObservableCollection<IPrefab>>();
            Repositories.Add(Compositions);
            Repositories.Add(Layers);
            Repositories.Add(Entities);
            Repositories.Add(Textures);
            Repositories.Add(Cameras);
            Repositories.Add(Lights);
        }

        public List<ObservableCollection<IPrefab>> Repositories { get; set; }

        public ObservableCollection<IPrefab> Compositions { get; set; }
        public ObservableCollection<IPrefab> Layers { get; set; }
        public ObservableCollection<IPrefab> Entities { get; set; }
        public ObservableCollection<IPrefab> Textures { get; set; }
        public ObservableCollection<IPrefab> Cameras { get; set; }
        public ObservableCollection<IPrefab> Lights { get; set; }

        public void AddPrefab(IPrefab prefab)
        {
            Type type = typeof(IPrefab);

            if (prefab is Composition composition)
                Compositions.Add(composition);

            if (prefab is Layer layer)
                Layers.Add(layer);

            if (prefab is Entity entity)
                Entities.Add(entity);

            if (prefab is Texture texture)
                Textures.Add(texture);

            if(prefab is Camera camera)
                Cameras.Add(camera);

            if (prefab is LightEntity lightEntity)
                Lights.Add(lightEntity);
        }

        public void RemovePrefab(IPrefab prefab)
        {
            Type type = typeof(IPrefab);

            if (prefab is Composition composition)
                Compositions.Remove(composition);

            if (prefab is Layer layer)
                Layers.Remove(layer);

            if (prefab is Entity entity)
                Entities.Remove(entity);

            if (prefab is Texture texture)
                Textures.Remove(texture);

            if (prefab is Camera camera)
                Cameras.Remove(camera);

            if (prefab is LightEntity lightEntity)
                Lights.Remove(lightEntity);
        }

        public IPrefab GetPrefab(Type type, Guid id)
        {
            var repo = GetRepository(type);
            return repo.FirstOrDefault(x => x.ID == id);
        }

        public ObservableCollection<IPrefab> GetRepository(Type type)
        {
            if(type == typeof(Composition))
                return Compositions;

            if(type == typeof(Layer))
                return Layers;

            if(type == typeof(Entity))
                return Entities;

            if(type == typeof(Texture))
                return Textures;

            if (type == typeof(Camera))
                return Cameras;

            if (type == typeof(LightEntity))
                return Lights;

            return null;
        }
    }
}
