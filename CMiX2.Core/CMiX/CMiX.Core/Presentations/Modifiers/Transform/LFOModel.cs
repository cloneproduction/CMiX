// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentations.Animation;
using CMiX.Core.Presentations.Beat;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public class LFOModel : IModifierModel
    {
        public LFOModel()
        {
            Name = "LFO";
            ID = Guid.NewGuid();

            Mode = new GenericValueModel<ModifierMode>(ModifierMode.PerInstance);

            Visible = new BooleanValueModel(true);
            BeatModifier = new BeatModifierModel();

            XAxis = new BooleanValueModel();
            YAxis = new BooleanValueModel();
            ZAxis = new BooleanValueModel();

            PingPong = new BooleanValueModel();

            TransformType = new GenericValueModel<TransformType>(Presentation.ViewModels.TransformType.Translate);
            Easing = new EasingModel();

            From = new FloatValueModel(0.0f);
            To = new FloatValueModel(1.0f);
        }

        public string Name { get; set; }
        public BooleanValueModel PingPong { get; set; }
        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public BeatModifierModel BeatModifier { get; set; }

        public BooleanValueModel XAxis { get; set; }
        public BooleanValueModel YAxis { get; set; }
        public BooleanValueModel ZAxis { get; set; }
        public GenericValueModel<TransformType> TransformType { get; set; }
        public FloatValueModel From { get; set; }
        public FloatValueModel To { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; internal set; }
        public EasingModel Easing { get; internal set; }
    }
}
