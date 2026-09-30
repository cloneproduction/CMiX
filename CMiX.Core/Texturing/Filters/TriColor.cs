// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TriColor : TextureFilterBase
    {
        public TriColor(PrefabService prefabService,
                        GenericValue<float> control,
                        GenericValue<string> colorA,
                        GenericValue<string> colorB,
                        GenericValue<string> colorC,
                        GenericValue<float> smooth,
                        GenericValue<float> center,
                        GenericValue<bool> singleChannel,
                        GenericValue<bool> clampColor,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            ColorA = colorA;
            ColorB = colorB;
            ColorC = colorC;
            Smooth = smooth;
            Center = center;
            SingleChannel = singleChannel;
            ClampColor = clampColor;
        }

        public GenericValue<float> Smooth { get; set; }
        public GenericValue<float> Center { get; set; }
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
                Smooth = (GenericValueModel<float>)Smooth.ToModel(),
                Center = (GenericValueModel<float>)Center.ToModel(),
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
            Smooth.FromModel(m.Smooth);
            Center.FromModel(m.Center);
            SingleChannel.FromModel(m.SingleChannel);
            ClampColor.FromModel(m.ClampColor);
        }
    }
}
