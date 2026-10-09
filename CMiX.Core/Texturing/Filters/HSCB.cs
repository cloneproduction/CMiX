// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
        }

        public ModulatableValue<float> Hue => Bindables[0];
        public ModulatableValue<float> Saturation => Bindables[1];
        public ModulatableValue<float> Contrast => Bindables[2];
        public ModulatableValue<float> Brightness => Bindables[3];

        public override IControlModel ToModel()
        {
            var model = new HSCBModel
            {
                Hue = (ModulatableValueModel<float>)Hue.ToModel(),
                Saturation = (ModulatableValueModel<float>)Saturation.ToModel(),
                Contrast = (ModulatableValueModel<float>)Contrast.ToModel(),
                Brightness = (ModulatableValueModel<float>)Brightness.ToModel()
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
