// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class Billboard : ObservableObject, IPrefab
    {
        public Billboard(PrefabService prefabService)
        {
            PrefabService = prefabService;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }


        [ObservableProperty]
        private bool isExpanded;
    }
}
