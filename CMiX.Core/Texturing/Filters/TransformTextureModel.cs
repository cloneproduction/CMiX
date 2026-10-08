// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record TransformTextureModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public SamplerStateModel SamplerState { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Rotation { get; set; } = ModulatableValueModel<float>.Of("Rotation", 0.0f);
        public ModulatableValueModel<float> Uniform { get; set; } = ModulatableValueModel<float>.Of("Uniform", 1.0f);
        public ModulatableVector2Model Location { get; set; } = ModulatableVector2Model.Of(0.0f, 0.0f);
        public ModulatableVector2Model Scale { get; set; } = ModulatableVector2Model.Of(1.0f, 1.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
