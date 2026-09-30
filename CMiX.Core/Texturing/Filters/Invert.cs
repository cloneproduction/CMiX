// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Invert : TextureFilterBase
    {
        public Invert(PrefabService prefabService,
                      GenericValue<float> factor,
                      GenericValue<bool> invertAlpha,
                      GenericValue<InvertChannel> invertChannel,
                      GenericValue<float> control,
                      Blend blend,
                      PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Factor = factor;
            InvertAlpha = invertAlpha;
            InvertChannelSelector = invertChannel;
        }

        public GenericValue<float> Factor { get; set; }
        public GenericValue<bool> InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new InvertModel
            {
                Factor = (GenericValueModel<float>)Factor.ToModel(),
                InvertChannelSelector = (GenericValueModel<InvertChannel>)InvertChannelSelector.ToModel(),
                InvertAlpha = (GenericValueModel<bool>)InvertAlpha.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (InvertModel)model;
            LoadBaseModel(m);
            Factor.FromModel(m.Factor);
            InvertChannelSelector.FromModel(m.InvertChannelSelector);
            InvertAlpha.FromModel(m.InvertAlpha);
        }
    }
}
