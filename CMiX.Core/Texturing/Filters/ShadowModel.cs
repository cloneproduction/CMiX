// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public record ShadowModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public Vector3Model LightDirection { get; set; } = new(2.51f, -0.91f, 1.15f);
        public GenericValueModel<float> Height { get; set; } = new(0.85f);
        public GenericValueModel<float> DotTolerance { get; set; } = new(0.36f);
        public GenericValueModel<float> RayJitter { get; set; } = new(0.0f);
        public GenericValueModel<float> RayLength { get; set; } = new(-0.07f);
        public GenericValueModel<float> ShadowFade { get; set; } = new(0.04f);
        public GenericValueModel<float> ShadowFallOffPow { get; set; } = new(0.6f);
        public GenericValueModel<float> ShadowBlur { get; set; } = new(0.001f);
        public GenericValueModel<float> ShadowBlurPow { get; set; } = new(-0.49f);
        public GenericValueModel<float> SharpOffset { get; set; } = new(-0.05f);
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
    }
}
