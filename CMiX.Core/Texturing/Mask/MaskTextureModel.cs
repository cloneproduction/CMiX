// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing
{
    public record class MaskTextureModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManagerModel TextureManager { get; set; } = new();
        public Transform2DModel Transform2D { get; set; } = new();
        public SamplerStateModel SamplerState { get; set; } = new();
        public GenericValueModel<bool> IsEnabled { get; set; } = new(false);
        public GenericValueModel<MaskChannel> MaskChannel { get; set; } = new(Texturing.MaskChannel.Value);
        public GenericValueModel<bool> Invert { get; set; } = new(false);
    }
}
