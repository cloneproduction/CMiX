// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class HSCB : TextureFilterBase
    {
        public HSCB(PrefabService prefabService,
                    ModulatableValue<float> hue,
                    ModulatableValue<float> saturation,
                    ModulatableValue<float> contrast,
                    ModulatableValue<float> brightness,
                    Blend blend,
                    PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { hue, saturation, contrast, brightness };

            hue.Label = "Hue";
            saturation.Label = "Saturation";
            contrast.Label = "Contrast";
            brightness.Label = "Brightness";
            hue.SetDefault(0.0f);
            saturation.SetDefault(1.0f);
            contrast.SetDefault(0.0f);
            brightness.SetDefault(0.0f);
        }

        public ModulatableValue<float> Hue => Bindables[0];
        public ModulatableValue<float> Saturation => Bindables[1];
        public ModulatableValue<float> Contrast => Bindables[2];
        public ModulatableValue<float> Brightness => Bindables[3];

        public override IControlModel ToModel()
        {
            var model = new HSCBModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (HSCBModel)model;
            LoadBaseModel(m);
        }
    }
}
