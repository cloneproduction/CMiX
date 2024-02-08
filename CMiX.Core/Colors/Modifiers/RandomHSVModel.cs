// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Colors.Modifiers
{
    public class RandomHSVModel : IModifierModel
    {
        public RandomHSVModel()
        {
            ID = Guid.NewGuid();
            Hue = new GenericValueModel<float>();
            Saturation = new GenericValueModel<float>();
            Value = new GenericValueModel<float>();
            Alpha = new GenericValueModel<float>();
            Visible = new GenericValueModel<bool>(true);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.PerInstance);
        }

        public Guid ID { get; set; }
        public Vector3Model HSV { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<float> Hue { get; set; }
        public GenericValueModel<float> Saturation { get; set; }
        public GenericValueModel<float> Value { get; set; }
        public GenericValueModel<float> Alpha { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }

    }
}
