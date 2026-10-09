// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
                      Blend blend,
                      PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            InvertAlpha = invertAlpha;
            InvertChannelSelector = invertChannel;

            Bindables = new List<ModulatableValue<float>> { factor };
        }

        public ModulatableValue<float> Factor => Bindables[0];
        public GenericValue<bool> InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new InvertModel
            {
                InvertChannelSelector = (GenericValueModel<InvertChannel>)InvertChannelSelector.ToModel(),
                InvertAlpha = (GenericValueModel<bool>)InvertAlpha.ToModel(),
                Factor = (ModulatableValueModel<float>)Factor.ToModel()
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
