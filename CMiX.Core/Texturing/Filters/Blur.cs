// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Blur : TextureFilterBase
    {
        public Blur(PrefabService prefabService,
                    GenericValue<bool> visible,
                    ModulatableValue<float> strength,
                    Blend blend,
                    PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { strength };

            strength.Label = "Strength";
            strength.SetDefault(0.5f);

            Visible = visible;
        }

        public ModulatableValue<float> Strength => Bindables[0];
        public GenericValue<bool> Visible { get; set; }

        public override IControlModel ToModel()
        {
            var model = new BlurModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (BlurModel)model;
            LoadBaseModel(m);
        }
    }
}
