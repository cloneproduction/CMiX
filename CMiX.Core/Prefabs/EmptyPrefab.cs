// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class EmptyPrefab : IPrefab
    {
        public EmptyPrefab(PrefabService prefabService)
        {
            PrefabService = prefabService;
            ID = prefabService.ID;
            IsSelected = prefabService.IsSelected;
            IsRenaming = prefabService.IsRenaming;
            Name = prefabService.Name;
            Visibility = prefabService.Visibility;
        }

        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> Visibility { get; set; }
        public GenericValue<bool> IsSelected { get; set; }
        public GenericValue<bool> IsRenaming { get; set; }
        public GenericValue<string> Name { get; set; }
        public Guid ID { get; set; }
    }
}
