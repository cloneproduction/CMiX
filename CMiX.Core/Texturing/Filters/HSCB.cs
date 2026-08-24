// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class HSCB : TextureFilterBase
    {
        public HSCB(PrefabService prefabService,
                    GenericValue<float> hue,
                    GenericValue<float> saturation,
                    GenericValue<float> contrast,
                    GenericValue<float> brightness,
                    GenericValue<float> control,
                    Blend blend)
            : base(prefabService, control, blend)
        {
            Hue = hue;
            Saturation = saturation;
            Contrast = contrast;
            Brightness = brightness;
        }

        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Saturation { get; set; }
        public GenericValue<float> Contrast { get; set; }
        public GenericValue<float> Brightness { get; set; }

        public override IControlModel ToModel()
        {
            var model = new HSCBModel
            {
                Hue = (GenericValueModel<float>)Hue.ToModel(),
                Saturation = (GenericValueModel<float>)Saturation.ToModel(),
                Contrast = (GenericValueModel<float>)Contrast.ToModel(),
                Brightness = (GenericValueModel<float>)Brightness.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (HSCBModel)model;
            LoadBaseModel(m);
            Hue.FromModel(m.Hue);
            Saturation.FromModel(m.Saturation);
            Contrast.FromModel(m.Contrast);
            Brightness.FromModel(m.Brightness);
        }
    }
}
