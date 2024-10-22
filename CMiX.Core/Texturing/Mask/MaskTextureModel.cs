// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Texturing
{
    public partial class MaskTextureModel : IControlModel
    {
        public MaskTextureModel()
        {
            TextureManager = new PrefabManagerModel();
            SamplerState = new SamplerStateModel();
            Invert = new GenericValueModel<bool>(false);
            TransformTexture = new TransformTextureModel();
            MaskChannel = new GenericValueModel<MaskChannel>(Texturing.MaskChannel.Value);
            IsEnabled = new GenericValueModel<bool>(false);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManagerModel TextureManager { get; set; }
        public TransformTextureModel TransformTexture { get; set; }
        public SamplerStateModel SamplerState { get; set; }
        public GenericValueModel<bool> IsEnabled { get; set; }
        public GenericValueModel<MaskChannel> MaskChannel { get; set; }
        public GenericValueModel<bool> Invert { get; set; }
    }
}
