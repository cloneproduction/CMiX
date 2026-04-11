// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
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
            PrefabService = prefabService;
            Hue = hue;
            Saturation = saturation;
            Contrast = contrast; 
            Brightness = brightness;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Saturation { get; set; }
        public GenericValue<float> Contrast { get; set; }
        public GenericValue<float> Brightness { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new HSCBModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Hue = (GenericValueModel<float>)Hue.ToModel(),
            Saturation = (GenericValueModel<float>)Saturation.ToModel(),
            Contrast = (GenericValueModel<float>)Contrast.ToModel(),
            Brightness = (GenericValueModel<float>)Brightness.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (HSCBModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Hue.FromModel(m.Hue);
            Saturation.FromModel(m.Saturation);
            Contrast.FromModel(m.Contrast);
            Brightness.FromModel(m.Brightness);
            Control.FromModel(m.Control);
        }
    }
}
