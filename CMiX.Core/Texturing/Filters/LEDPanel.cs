// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
                        GenericValue<float> control,
                        ModulatableValue<float> pixelSize,
                        ModulatableValue<float> maskStagger,
                        ModulatableValue<float> maskBorder,
                        ModulatableValue<float> maskIntensity,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { pixelSize, maskStagger, maskBorder, maskIntensity };

            pixelSize.Label = "Pixel Size";
            maskStagger.Label = "Mask Stagger";
            maskBorder.Label = "Mask Border";
            maskIntensity.Label = "Mask Intensity";
            pixelSize.SetDefault(10.0f);
            maskStagger.SetDefault(0.0f);
            maskBorder.SetDefault(0.0f);
            maskIntensity.SetDefault(0.0f);
        }

        public ModulatableValue<float> PixelSize => Bindables[0];
        public ModulatableValue<float> MaskStagger => Bindables[1];
        public ModulatableValue<float> MaskBorder => Bindables[2];
        public ModulatableValue<float> MaskIntensity => Bindables[3];

        public override IControlModel ToModel()
        {
            var model = new LEDPanelModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LEDPanelModel)model;
            LoadBaseModel(m);
        }
    }
}
