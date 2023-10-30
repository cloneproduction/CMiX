// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;

namespace CMiX.Core.Prefabs
{
    public class PrefabRepository 
    {
        public PrefabRepository()
        {
            Prefabs = new ObservableCollection<IPrefab>();
        }

        private int nameCount = 0;
        public ObservableCollection<IPrefab> Prefabs { get; set; }

        public void AddPrefab(IPrefab prefab)
        {
            Prefabs.Add(prefab);
        }

        public void RemovePrefab(IPrefab prefab)
        {
            Prefabs.Remove(prefab);
        }

        public IPrefab GetPrefab(Guid id)
        {
            return Prefabs.FirstOrDefault(x => x.ID == id);
        }
    }
}
