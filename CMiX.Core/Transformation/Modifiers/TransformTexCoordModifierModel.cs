// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;

namespace CMiX.Core.Transformation.Modifiers
{
    public class TransformTexCoordModifierModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public Vector2Model Location { get; set; } = new(0.0f, 0.0f);
        public Vector2Model Scale { get; set; } = new(1.0f, 1.0f);
        public GenericValueModel<float> Uniform { get; set; } = new(1.0f);
        public GenericValueModel<float> Rotation { get; set; } = new(0.0f);
        public SamplerStateModel SamplerState { get; set; } = new();
    }
}
