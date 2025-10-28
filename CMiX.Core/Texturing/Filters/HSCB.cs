// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class HSCB : ObservableObject, IPrefab, ITextureFilter
    {
        public HSCB(PrefabService prefabService,
                    GenericValue<float> hue, 
                    GenericValue<float> saturation, 
                    GenericValue<float> contrast, 
                    GenericValue<float> brightness, 
                    GenericValue<float> control)
        {
            ID = Guid.NewGuid();
            PrefabService = prefabService;
            Hue = hue;
            Saturation = saturation;
            Contrast = contrast; 
            Brightness = brightness;
            Control = control;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Saturation { get; set; }
        public GenericValue<float> Contrast { get; set; }
        public GenericValue<float> Brightness { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
