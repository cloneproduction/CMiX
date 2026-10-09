// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LEDPanel : TextureFilterBase
    {
        public LEDPanel(PrefabService prefabService,
                        ModulatableValue<float> pixelSize,
                        ModulatableValue<float> maskStagger,
                        ModulatableValue<float> maskBorder,
                        ModulatableValue<float> maskIntensity,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { pixelSize, maskStagger, maskBorder, maskIntensity };
        }

        public ModulatableValue<float> PixelSize => Bindables[0];
        public ModulatableValue<float> MaskStagger => Bindables[1];
        public ModulatableValue<float> MaskBorder => Bindables[2];
        public ModulatableValue<float> MaskIntensity => Bindables[3];

        public override IControlModel ToModel()
        {
            var model = new LEDPanelModel
            {
                PixelSize = (ModulatableValueModel<float>)PixelSize.ToModel(),
                MaskStagger = (ModulatableValueModel<float>)MaskStagger.ToModel(),
                MaskBorder = (ModulatableValueModel<float>)MaskBorder.ToModel(),
                MaskIntensity = (ModulatableValueModel<float>)MaskIntensity.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LEDPanelModel)model;
            LoadBaseModel(m);
            PixelSize.FromModel(m.PixelSize);
            MaskStagger.FromModel(m.MaskStagger);
            MaskBorder.FromModel(m.MaskBorder);
            MaskIntensity.FromModel(m.MaskIntensity);
        }
    }
}
