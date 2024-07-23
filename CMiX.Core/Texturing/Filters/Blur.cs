// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Blur : ObservableObject, IPrefab
    {
        public Blur(PrefabService prefabService, 
                    GenericValue<bool> visible, 
                    GenericValue<float> strength)
        {
            PrefabService = prefabService;
            Strength = strength;
            Visible = visible;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Strength { get; set; }
        public GenericValue<bool> Visible { get; set; }
        public PrefabService PrefabService { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
