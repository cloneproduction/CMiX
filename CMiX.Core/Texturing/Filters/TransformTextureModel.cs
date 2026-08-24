// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Filters
{
    public record TransformTextureModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public SamplerStateModel SamplerState { get; set; } = new();
        public Transform2DModel Transform2D { get; set; } = new();
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public BlendModel Blend {  get; set; } = new();
    }
}
