// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TriColor : TextureFilterBase
    {
        public TriColor(PrefabService prefabService,
                        GenericValue<string> colorA,
                        GenericValue<string> colorB,
                        GenericValue<string> colorC,
                        ModulatableValue<float> smooth,
                        ModulatableValue<float> center,
                        GenericValue<bool> singleChannel,
                        GenericValue<bool> clampColor,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            ColorA = colorA;
            ColorB = colorB;
            ColorC = colorC;
            SingleChannel = singleChannel;
            ClampColor = clampColor;

            Bindables = new List<ModulatableValue<float>> { smooth, center };

            smooth.Label = "Smooth";
            center.Label = "Center";
            smooth.SetDefault(0.5f);
            center.SetDefault(0.5f);
        }

        public ModulatableValue<float> Smooth => Bindables[0];
        public ModulatableValue<float> Center => Bindables[1];
        public GenericValue<string> ColorA { get; set; }
        public GenericValue<string> ColorB { get; set; }
        public GenericValue<string> ColorC { get; set; }
        public GenericValue<bool> SingleChannel { get; set; }
        public GenericValue<bool> ClampColor { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TriColorModel
            {
                ColorA = (GenericValueModel<string>)ColorA.ToModel(),
                ColorB = (GenericValueModel<string>)ColorB.ToModel(),
                ColorC = (GenericValueModel<string>)ColorC.ToModel(),
                SingleChannel = (GenericValueModel<bool>)SingleChannel.ToModel(),
                ClampColor = (GenericValueModel<bool>)ClampColor.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TriColorModel)model;
            LoadBaseModel(m);
            ColorA.FromModel(m.ColorA);
            ColorB.FromModel(m.ColorB);
            ColorC.FromModel(m.ColorC);
            SingleChannel.FromModel(m.SingleChannel);
            ClampColor.FromModel(m.ClampColor);
        }
    }
}
