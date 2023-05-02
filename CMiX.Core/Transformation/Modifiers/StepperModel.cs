// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Transformation.Modifiers
{
    public class StepperModel : IModifierModel
    {
        public StepperModel()
        {

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
