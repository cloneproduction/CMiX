// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record DisplaceModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManagerModel TextureSelector { get; set; } = new();
        public Vector2Model Offset { get; set; } = new(0.5f, 0.5f);
        public Vector2Model OffsetScale { get; set; } = new(0.1f, 0.1f);
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
    }
}
