// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using CMiX.Core.Compositing;
using CMiX.Core.Texturing;

namespace CMiX.Core.Prefab
{
    public class PrefabRepository 
    {
        public PrefabRepository()
        {
            //Prefabs = new ObservableCollection<IPrefab>();

            Compositions = new ObservableCollection<IPrefab>();
            Layers = new ObservableCollection<IPrefab>();
            Entities = new ObservableCollection<IPrefab>();
            Textures = new ObservableCollection<IPrefab>();
        }

        //public ObservableCollection<IPrefab> Prefabs { get; set; }


        public ObservableCollection<IPrefab> Compositions { get; set; }
        public ObservableCollection<IPrefab> Layers { get; set; }
        public ObservableCollection<IPrefab> Entities { get; set; }
        public ObservableCollection<IPrefab> Textures { get; set; }


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

            //Prefabs.Add(prefab);
        }

        public void RemovePrefab(IPrefab prefab)
        {
            //Prefabs.Remove(prefab);
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

            return null;
        }
    }
}
