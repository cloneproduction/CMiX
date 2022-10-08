// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class LFOModel : IModifierModel
    {
        public LFOModel()
        {
            Name = "LFO";
            this.ID = Guid.NewGuid();
            Enabled = true;

            Mode = new ComboBoxModel<ModifierMode>(ModifierMode.PerInstance);

            Visible = new ToggleButtonModel(true);
            BeatModifier = new BeatModifierModel();

            XAxis = new ToggleButtonModel();
            YAxis = new ToggleButtonModel();
            ZAxis = new ToggleButtonModel();

            PingPong = new ToggleButtonModel();

            TransformType = new ComboBoxModel<TransformType>(Presentation.ViewModels.TransformType.Translate);
            Easing = new EasingModel();

            From = new SliderModel(0.0f);
            To = new SliderModel(1.0f);
        }

        public string Name { get; set; }
        public ToggleButtonModel PingPong { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public BeatModifierModel BeatModifier { get; set; }

        public ToggleButtonModel XAxis { get; set; }
        public ToggleButtonModel YAxis { get; set; }
        public ToggleButtonModel ZAxis { get; set; }
        public ComboBoxModel<TransformType> TransformType { get; set; }
        public SliderModel From { get; set; }
        public SliderModel To { get; set; }
        public ComboBoxModel<ModifierMode> Mode { get; internal set; }
        public EasingModel Easing { get; internal set; }
    }
}
