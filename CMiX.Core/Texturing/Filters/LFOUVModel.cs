// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Filters
{
    public record LFOUVModel : IPrefabModel, ITextureFilterModel, IBeatModifiableModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<TransformType> TransformType { get; set; } = new(Transformation.TransformType.Translate);
        public GenericValueModel<bool> PingPong { get; set; } = new(false);
        public DirectionXYModel DirectionXY { get; set; } = new();
        public GenericValueModel<float> From { get; set; } = new(-1.0f);
        public GenericValueModel<float> To { get; set; } = new(1.0f);
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public SamplerStateModel SamplerState { get; set; } = new();
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
    }
}
