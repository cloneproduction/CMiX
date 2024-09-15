// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class ShiftRGB : ObservableObject, IPrefab
    {
        public ShiftRGB(PrefabService prefabService,
                        GenericValue<float> direction,
                        GenericValue<float> shift,
                        GenericValue<float> hue,
                        GenericValue<float> factor)
        {
            PrefabService = prefabService;
            Direction = direction;
            Shift = shift;
            Hue = hue;
            Factor = factor;
        }

        public PrefabService PrefabService { get; set; }
        public Guid ID { get; set; }

        public GenericValue<float> Direction { get; set; }
        public GenericValue<float> Shift { get; set; }
        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Factor { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
