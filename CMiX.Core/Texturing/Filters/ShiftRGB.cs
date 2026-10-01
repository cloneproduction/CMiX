// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class ShiftRGB : TextureFilterBase
    {
        public ShiftRGB(PrefabService prefabService,
                        ModulatableValue<float> direction,
                        ModulatableValue<float> shift,
                        ModulatableValue<float> hue,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { direction, shift, hue };

            direction.Label = "Direction";
            shift.Label = "Shift";
            hue.Label = "Hue";
            direction.SetDefault(0.25f);
            shift.SetDefault(0.2f);
            hue.SetDefault(0.0f);
        }

        public ModulatableValue<float> Direction => Bindables[0];
        public ModulatableValue<float> Shift => Bindables[1];
        public ModulatableValue<float> Hue => Bindables[2];

        public override IControlModel ToModel()
        {
            var model = new ShiftRGBModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ShiftRGBModel)model;
            LoadBaseModel(m);
        }
    }
}
