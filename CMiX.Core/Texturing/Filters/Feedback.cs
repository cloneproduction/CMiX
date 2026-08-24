// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Feedback : TextureFilterBase
    {
        public Feedback(PrefabService prefabService,
                        GenericValue<float> factor,
                        GenericValue<float> control,
                        Blend blend)
            : base(prefabService, control, blend)
        {
            Factor = factor;
        }

        public GenericValue<float> Factor { get; set; }

        public override IControlModel ToModel()
        {
            var model = new FeedbackModel
            {
                Factor = (GenericValueModel<float>)Factor.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (FeedbackModel)model;
            LoadBaseModel(m);
            Factor.FromModel(m.Factor);
        }
    }
}
