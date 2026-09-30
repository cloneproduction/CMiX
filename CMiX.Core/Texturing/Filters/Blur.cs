// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Blur : TextureFilterBase
    {
        public Blur(PrefabService prefabService,
                    GenericValue<bool> visible,
                    GenericValue<float> strength,
                    GenericValue<float> control,
                    Blend blend,
                    PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Strength = strength;
            Visible = visible;
        }

        public GenericValue<float> Strength { get; set; }
        public GenericValue<bool> Visible { get; set; }

        public override IControlModel ToModel()
        {
            var model = new BlurModel { Strength = (GenericValueModel<float>)Strength.ToModel() };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (BlurModel)model;
            LoadBaseModel(m);
            Strength.FromModel(m.Strength);
        }
    }
}
