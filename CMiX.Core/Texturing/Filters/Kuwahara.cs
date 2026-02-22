// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Kuwahara : ObservableObject, IPrefab, ITextureFilter
    {
        public Kuwahara(PrefabService prefabService,
                        GenericValue<float> radius,
                        GenericValue<KuwaharaType> type,
                        GenericValue<float> control)
        {
            PrefabService = prefabService;
            Radius = radius;
            Control = control;
            Type = type;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Radius { get; set; }
        public GenericValue<KuwaharaType> Type { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
