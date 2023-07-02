// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class LFOModel : IModifierModel
    {
        public LFOModel()
        {
            Name = "LFO";
            ID = Guid.NewGuid();
            Visible = new BooleanValueModel(true);
            BeatModifier = new BeatModifierModel();
            DirectionXYZ = new DirectionXYZModel();
            PingPong = new BooleanValueModel();
            TransformType = new GenericValueModel<TransformType>(Transformation.TransformType.Translate);
            Easing = new EasingModel();
            From = new FloatValueModel(0.0f);
            To = new FloatValueModel(1.0f);
        }

        public string Name { get; set; }
        public BooleanValueModel PingPong { get; set; }
        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public BeatModifierModel BeatModifier { get; set; }

        public GenericValueModel<TransformType> TransformType { get; set; }
        public FloatValueModel From { get; set; }
        public FloatValueModel To { get; set; }
        public EasingModel Easing { get; internal set; }
        public DirectionXYZModel DirectionXYZ { get; internal set; }
    }
}
