// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public record AsciiModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> GridSize { get; set; } = new(0.66f);
        public Vector2Model CharacterSize { get; set; } = new(16.0f, 16.0f);
        public GenericValueModel<bool> Grayscale { get; set; } = new(false);
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public BlendModel Blend { get; set; } = new();
    }
}
