// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Invert : TextureFilterBase
    {
        public Invert(PrefabService prefabService,
                      ModulatableValue<float> factor,
                      GenericValue<bool> invertAlpha,
                      GenericValue<InvertChannel> invertChannel,
                      GenericValue<float> control,
                      Blend blend,
                      PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            InvertAlpha = invertAlpha;
            InvertChannelSelector = invertChannel;

            Bindables = new List<ModulatableValue<float>> { factor };

            factor.Label = "Factor";
            factor.SetDefault(1.0f);
        }

        public ModulatableValue<float> Factor => Bindables[0];
        public GenericValue<bool> InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new InvertModel
            {
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
            InvertChannelSelector.FromModel(m.InvertChannelSelector);
            InvertAlpha.FromModel(m.InvertAlpha);
        }
    }
}
