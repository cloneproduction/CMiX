// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation.Modifiers
{
    public class StepperModel : IPrefabModel
    {
        public StepperModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            StepCount = new GenericValueModel<int>(2);
            To = new GenericValueModel<float>(1.0f);
            From = new GenericValueModel<float>(-1.0f);
            Easing = new EasingModel();
            TransformType = new GenericValueModel<TransformType>(Transformation.TransformType.Translate);
            PingPong = new GenericValueModel<bool>(false);
            BeatModifier = new BeatModifierModel();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);
            ModifierModeSelector = new ModifierModeSelectorModel();
            DirectionXYZ = new DirectionXYZModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<int> StepCount { get; set; }
        public GenericValueModel<float> To { get; set; }
        public GenericValueModel<float> From { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<TransformType> TransformType { get; set; }
        public GenericValueModel<bool> PingPong { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
        public DirectionXYZModel DirectionXYZ { get; set; }
    }
}
