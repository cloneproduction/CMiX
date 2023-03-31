// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;

namespace CMiX.Core.Presentation.Prefabs
{
    public class PrefabDataBase : IPrefabDataBase
    {
        public PrefabDataBase()
        {
            Prefabs = new ObservableCollection<IPrefab>();
        }

        public ObservableCollection<IPrefab> Prefabs { get; set; }
    }
}
