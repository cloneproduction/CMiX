// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Dither : TextureFilterBase
    {
        public Dither(PrefabService prefabService,
                      GenericValue<float> control,
                      GenericValue<float> threshold,
                      Blend blend,
                      PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Threshold = threshold;
        }

        public GenericValue<float> Threshold { get; set; }

        public override IControlModel ToModel()
        {
            var model = new DitherModel
            {
                Threshold = (GenericValueModel<float>)Threshold.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (DitherModel)model;
            LoadBaseModel(m);
            Threshold.FromModel(m.Threshold);
        }
    }
}
