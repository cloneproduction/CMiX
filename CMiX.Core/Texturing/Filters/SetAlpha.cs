// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class SetAlpha : ObservableObject, IPrefab, ITextureFilter
    {
        public SetAlpha(PrefabService prefabService,
                        GenericValue<bool> invert,
                        GenericValue<bool> keepOriginalAlpha,
                        GenericValue<AlphaChannel> alphaChannel, 
                        GenericValue<float> control,
                        Blend blend)
        {
            PrefabService = prefabService;
            Invert = invert;
            KeepOriginalAlpha = keepOriginalAlpha;
            AlphaChannel = alphaChannel;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> KeepOriginalAlpha { get; set; }
        public GenericValue<AlphaChannel> AlphaChannel { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new SetAlphaModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Invert = (GenericValueModel<bool>)Invert.ToModel(),
            KeepOriginalAlpha = (GenericValueModel<bool>)KeepOriginalAlpha.ToModel(),
            AlphaChannel = (GenericValueModel<AlphaChannel>)AlphaChannel.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (SetAlphaModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Invert.FromModel(m.Invert);
            KeepOriginalAlpha.FromModel(m.KeepOriginalAlpha);
            AlphaChannel.FromModel(m.AlphaChannel);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);
        }
    }
}
