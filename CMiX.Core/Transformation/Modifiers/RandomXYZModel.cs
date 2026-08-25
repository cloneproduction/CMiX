// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Transformation.Modifiers
{
    public record RandomXYZModel : IPrefabModel, IBeatModifiableModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<bool> Gaussian { get; set; } = new(false);
        public GenericValueModel<bool> RandomizeLocation { get; set; } = new(true);
        public Vector3Model Location { get; set; } = new();
        public GenericValueModel<bool> RandomizeScale { get; set; } = new(true);
        public Vector3Model Scale { get; set; } = new();
        public GenericValueModel<bool> RandomizeRotation { get; set; } = new(true);
        public Vector3Model Rotation { get; set; } = new();
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
        public GenericValueModel<float> Uniform { get; set; } = new(0.0f);
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new();
    }
}
