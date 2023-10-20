// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class EmptyPrefab : IPrefab
    {
        public EmptyPrefab(PrefabService prefabService)
        {
            IsSelected = prefabService.IsSelected;
            IsRenaming = prefabService.IsRenaming;
            Name = prefabService.Name;
            Visibility = prefabService.Visibility;
        }

        public BooleanValue Visibility { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public Guid ID { get; set; } = Guid.NewGuid();
    }
}
