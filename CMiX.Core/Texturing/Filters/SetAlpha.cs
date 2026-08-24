// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class SetAlpha : TextureFilterBase
    {
        public SetAlpha(PrefabService prefabService,
                        GenericValue<bool> invert,
                        GenericValue<bool> keepOriginalAlpha,
                        GenericValue<AlphaChannel> alphaChannel,
                        GenericValue<float> control,
                        Blend blend)
            : base(prefabService, control, blend)
        {
            Invert = invert;
            KeepOriginalAlpha = keepOriginalAlpha;
            AlphaChannel = alphaChannel;
        }

        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> KeepOriginalAlpha { get; set; }
        public GenericValue<AlphaChannel> AlphaChannel { get; set; }

        public override IControlModel ToModel()
        {
            var model = new SetAlphaModel
            {
                Invert = (GenericValueModel<bool>)Invert.ToModel(),
                KeepOriginalAlpha = (GenericValueModel<bool>)KeepOriginalAlpha.ToModel(),
                AlphaChannel = (GenericValueModel<AlphaChannel>)AlphaChannel.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (SetAlphaModel)model;
            LoadBaseModel(m);
            Invert.FromModel(m.Invert);
            KeepOriginalAlpha.FromModel(m.KeepOriginalAlpha);
            AlphaChannel.FromModel(m.AlphaChannel);
        }
    }
}
