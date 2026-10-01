// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Dither : TextureFilterBase
    {
        public Dither(PrefabService prefabService,
                      ModulatableValue<float> threshold,
                      Blend blend,
                      PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { threshold };

            threshold.Label = "Threshold";
            threshold.SetDefault(1.0f);
        }

        public ModulatableValue<float> Threshold => Bindables[0];

        public override IControlModel ToModel()
        {
            var model = new DitherModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (DitherModel)model;
            LoadBaseModel(m);
        }
    }
}
