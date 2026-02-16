// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs
{
    public class EmptyPrefab : IControl, IPrefab
    {
        public EmptyPrefab(PrefabService prefabService)
        {
            PrefabService = prefabService;
            ID = prefabService.ID;
        }

        public IControlModel ToModel() => this.ToModel();
        public PrefabService PrefabService { get; set; }
        public Guid ID { get; set; }
    }
}
