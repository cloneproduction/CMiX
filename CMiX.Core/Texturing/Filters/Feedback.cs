// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Feedback : TextureFilterBase
    {
        public Feedback(PrefabService prefabService,
                        ModulatableValue<float> factor,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { factor };
        }

        public ModulatableValue<float> Factor => Bindables[0];

        public override IControlModel ToModel()
        {
            var model = new FeedbackModel { Factor = (ModulatableValueModel<float>)Factor.ToModel() };
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
