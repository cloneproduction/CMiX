// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Invert : ObservableObject, IPrefab, ITextureFilter
    {
        public Invert(PrefabService prefabService,
                      GenericValue<float> factor, 
                      GenericValue<bool> invertAlpha, 
                      GenericValue<InvertChannel> invertChannel, 
                      GenericValue<float> control,
                      Blend blend)
        {
            PrefabService = prefabService;
            Factor = factor;
            InvertAlpha = invertAlpha;
            InvertChannelSelector = invertChannel;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Factor { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }    

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new InvertModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Factor = (GenericValueModel<float>)Factor.ToModel(),
            InvertChannelSelector = (GenericValueModel<InvertChannel>)InvertChannelSelector.ToModel(),
            InvertAlpha = (GenericValueModel<bool>)InvertAlpha.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel(),
        };

        public void FromModel(IControlModel model)
        {
            var m = (InvertModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Factor.FromModel(m.Factor);
            InvertChannelSelector.FromModel(m.InvertChannelSelector);
            InvertAlpha.FromModel(m.InvertAlpha);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);
        }
    }
}
