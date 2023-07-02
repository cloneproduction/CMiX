// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class StepperModel : IModifierModel
    {
        public StepperModel()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValueModel(true);
            StepCount = new IntegerValueModel(2);
            To = new FloatValueModel(1.0f);
            From = new FloatValueModel(-1.0f);
            Easing = new EasingModel();
            TransformType = new GenericValueModel<TransformType>(Transformation.TransformType.Translate);
            PingPong = new BooleanValueModel(false);
            XAxis = new BooleanValueModel(true);
            YAxis = new BooleanValueModel(false);
            ZAxis = new BooleanValueModel(false);
            BeatModifier = new BeatModifierModel();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);
        }

        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public IntegerValueModel StepCount { get; set; }
        public FloatValueModel To { get; set; }
        public FloatValueModel From { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<TransformType> TransformType { get; set; }
        public BooleanValueModel PingPong { get; set; }
        public BooleanValueModel ZAxis { get; set; }
        public BooleanValueModel YAxis { get; set; }
        public BooleanValueModel XAxis { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
    }
}
