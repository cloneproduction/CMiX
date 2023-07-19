// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Filters
{
    public class LFOUVModel : IModifierModel
    {
        public LFOUVModel()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValueModel(true);
            BeatModifier = new BeatModifierModel();
            PingPong = new BooleanValueModel();
            XAxis = new BooleanValueModel();
            YAxis = new BooleanValueModel();
            ZAxis = new BooleanValueModel();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.PerInstance);
            TransformType = new GenericValueModel<TransformType>(Transformation.TransformType.Translate);
            Easing = new EasingModel();
            From = new FloatValueModel(0.0f);
            To = new FloatValueModel(1.0f);
            SamplerState = new SamplerStateModel();
        }

        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public BooleanValueModel PingPong { get; set; }
        public BooleanValueModel XAxis { get; set; }
        public BooleanValueModel YAxis { get; set; }
        public BooleanValueModel ZAxis { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
        public GenericValueModel<TransformType> TransformType { get; set; }
        public EasingModel Easing { get; set; }
        public FloatValueModel From { get; set; }
        public FloatValueModel To { get; set; }
        public SamplerStateModel SamplerState { get; set; }
    }
}
