// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Edge : ObservableObject, IPrefab, ITextureModifier
    {
        public Edge(PrefabService prefabService, 
                    GenericValue<float> radius, 
                    GenericValue<float> brightness, 
                    GenericValue<float> control)
        {
            PrefabService = prefabService;
            Radius = radius;
            Brightness = brightness;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Radius { get; set; }
        public GenericValue<float> Brightness { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
