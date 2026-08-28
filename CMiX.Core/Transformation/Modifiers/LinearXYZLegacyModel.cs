// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation.Modifiers
{
    public record LinearXYZLegacyModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BeatModifierModel BeatModifier { get; set; } = new();
        public GenericValueModel<float> Width { get; set; } = new(0.0f);
        public DirectionXYZModel DirectionXYZ { get; set; } = new();
        public GenericValueModel<float> Phase { get; set; } = new(0.0f);
        public GenericValueModel<ModifierMode> Mode { get; set; } = new(ModifierMode.ToSpread);
        public GenericValueModel<TransformType> TransformTypeSelector { get; set; } = new(TransformType.Translate);
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new();
    }
}
