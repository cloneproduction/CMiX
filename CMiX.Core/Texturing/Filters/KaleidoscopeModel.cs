// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public record KaleidoscopeModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<int> Divisions { get; set; } = new(2);
        public GenericValueModel<int> Iterations { get; set; } = new(2);
        public GenericValueModel<float> IterationZoom { get; set; } = new(0.0f);
        public GenericValueModel<float> Rotation { get; set; } = new(0.0f);
        public GenericValueModel<float> Zoom { get; set; } = new(0.5f);
        public GenericValueModel<float> CellRotation { get; set; } = new(0.0f);
        public Vector2Model Center { get; set; } = new(0.0f, 0.0f);
        public Vector2Model CellOffset { get; set; } = new(0.0f, 0.0f);
        public Vector2Model CellScale { get; set; } = new(1.0f, 1.0f);
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public BlendModel Blend { get; set; } = new();
    }
}
