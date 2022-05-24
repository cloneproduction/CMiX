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
            Enabled = true;

            Hue = new SliderModel();
            Saturation = new SliderModel();
            Value = new SliderModel();
            Alpha = new SliderModel();

            Visible = new ToggleButtonModel(true);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            Mode = new ComboBoxModel<ModifierMode>(ModifierMode.PerInstance);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public VectorXYZModel HSV { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public SliderModel Hue { get; set; }
        public SliderModel Saturation { get; set; }
        public SliderModel Value { get; set; }
        public SliderModel Alpha { get; set; }

        public ComboBoxModel<ModifierMode> Mode { get; set; }

    }
}
