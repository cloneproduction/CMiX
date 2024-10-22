// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation.Modifiers
{
    public class LinearXYZModel : IPrefabModel
    {
        public LinearXYZModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            BeatModifier = new BeatModifierModel();
            Width = new GenericValueModel<float>(0.0f);
            Phase = new GenericValueModel<float>(0.0f);
            DirectionXYZ = new DirectionXYZModel();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);
            TransformTypeSelector = new GenericValueModel<TransformType>(TransformType.Translate);
            ModifierModeSelector = new ModifierModeSelectorModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public GenericValueModel<float> Width { get; set; }
        public DirectionXYZModel DirectionXYZ { get; set; }
        public GenericValueModel<float> Phase { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
        public GenericValueModel<TransformType> TransformTypeSelector { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
    }
}
