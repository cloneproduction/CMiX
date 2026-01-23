// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Transformation.Modifiers
{
    public record LFOModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = new("LFO");
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<bool> PingPong { get; set; } = new(false);
        public GenericValueModel<float> RandomizePhase { get; set; } = new(0.0f);
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new();
        public GenericValueModel<TransformType> TransformType { get; set; } = new(Transformation.TransformType.Translate);
        public GenericValueModel<float> From { get; set; } = new(0.0f);
        public GenericValueModel<float> To { get; set; } = new(1.0f);
        public DirectionXYZModel DirectionXYZ { get; set; } = new();
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
    }
}
