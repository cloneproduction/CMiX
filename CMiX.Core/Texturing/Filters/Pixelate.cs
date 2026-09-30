// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pixelate : TextureFilterBase
    {
        public Pixelate(PrefabService prefabService,
                        ModulatableValue<float> factorX,
                        ModulatableValue<float> factorY,
                        GenericValue<float> control,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { factorX, factorY };

            factorX.Label = "X";
            factorY.Label = "Y";
            factorX.SetDefault(0.5f);
            factorY.SetDefault(0.5f);
        }

        public ModulatableValue<float> X => Bindables[0];
        public ModulatableValue<float> Y => Bindables[1];

        public override IControlModel ToModel()
        {
            var model = new PixelateModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (PixelateModel)model;
            LoadBaseModel(m);
        }
    }
}
