// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record KaleidoscopeModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<int> Divisions { get; set; } = new(2);
        public GenericValueModel<int> Iterations { get; set; } = new(2);
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> IterationZoom { get; set; } = ModulatableValueModel<float>.Of("Iteration Zoom", 0.0f);
        public ModulatableValueModel<float> Rotation { get; set; } = ModulatableValueModel<float>.Of("Rotation", 0.0f);
        public ModulatableValueModel<float> Zoom { get; set; } = ModulatableValueModel<float>.Of("Zoom", 0.5f);
        public ModulatableValueModel<float> CellRotation { get; set; } = ModulatableValueModel<float>.Of("Cell Rotation", 0.0f);
        public ModulatableVector2Model Center { get; set; } = ModulatableVector2Model.Of(0.0f, 0.0f);
        public ModulatableVector2Model CellOffset { get; set; } = ModulatableVector2Model.Of(0.0f, 0.0f);
        public ModulatableVector2Model CellScale { get; set; } = ModulatableVector2Model.Of(1.0f, 1.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
