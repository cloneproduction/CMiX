// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class RandomHSVModel : IModifierModel
    {
        public RandomHSVModel()
        {
            ID = Guid.NewGuid();

            Hue = new FloatValueModel();
            Saturation = new FloatValueModel();
            Value = new FloatValueModel();
            Alpha = new FloatValueModel();

            Visible = new BooleanValueModel(true);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.PerInstance);
        }

        public Guid ID { get; set; }

        public Vector3Model HSV { get; set; }
        public BooleanValueModel Visible { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public FloatValueModel Hue { get; set; }
        public FloatValueModel Saturation { get; set; }
        public FloatValueModel Value { get; set; }
        public FloatValueModel Alpha { get; set; }

        public GenericValueModel<ModifierMode> Mode { get; set; }

    }
}
