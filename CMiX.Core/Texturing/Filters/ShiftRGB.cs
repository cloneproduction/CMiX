// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class ShiftRGB : TextureFilterBase
    {
        public ShiftRGB(PrefabService prefabService,
                        GenericValue<float> direction,
                        GenericValue<float> shift,
                        GenericValue<float> hue,
                        GenericValue<float> control,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Direction = direction;
            Shift = shift;
            Hue = hue;
        }

        public GenericValue<float> Direction { get; set; }
        public GenericValue<float> Shift { get; set; }
        public GenericValue<float> Hue { get; set; }

        public override IControlModel ToModel()
        {
            var model = new ShiftRGBModel
            {
                Direction = (GenericValueModel<float>)Direction.ToModel(),
                Shift = (GenericValueModel<float>)Shift.ToModel(),
                Hue = (GenericValueModel<float>)Hue.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ShiftRGBModel)model;
            LoadBaseModel(m);
            Direction.FromModel(m.Direction);
            Shift.FromModel(m.Shift);
            Hue.FromModel(m.Hue);
        }
    }
}
