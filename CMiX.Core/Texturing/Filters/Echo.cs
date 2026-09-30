// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Echo : TextureFilterBase
    {
        public Echo(PrefabService prefabService,
                    GenericValue<float> factor,
                    GenericValue<float> control,
                    Blend blend,
                    PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Factor = factor;
        }

        public GenericValue<float> Factor { get; set; }

        public override IControlModel ToModel()
        {
            var model = new EchoModel
            {
                Factor = (GenericValueModel<float>)Factor.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (EchoModel)model;
            LoadBaseModel(m);
            Factor.FromModel(m.Factor);
        }
    }
}
