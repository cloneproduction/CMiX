// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class RandomScaleModel : IModifierModel
    {
        public RandomScaleModel()
        {
            this.ID = Guid.NewGuid();
            Name = TransformModifierNames.RandomXY;
            Mode = new ComboBoxModel<ModifierMode>(ModifierMode.ToSpread);

            Visible = new ToggleButtonModel(true);

            BeatModifierModel = new BeatModifierModel();
            CounterModel = new CounterModel(1);
            EasingModel = new EasingModel();

            ScaleX = new SliderModel();
            ScaleY = new SliderModel();
            ScaleZ = new SliderModel();
            UniformXYZ = new SliderModel();

            Spread = new ToggleButtonModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }


        public ComboBoxModel<ModifierMode> Mode { get; set; }
        public ToggleButtonModel Visible { get; set; }

        public EasingModel EasingModel { get; set; }
        public CounterModel CounterModel { get; set; }


        public ToggleButtonModel RandomizeScale { get; set; }
        public SliderModel ScaleX { get; set; }
        public SliderModel ScaleY { get; set; }
        public SliderModel ScaleZ { get; set; }


        public BeatModifierModel BeatModifierModel { get; set; }
        public TransformModifierNames Name { get; set; }

        public int Count { get; set; }
        public ToggleButtonModel Spread { get; internal set; }
        public SliderModel UniformXYZ { get; internal set; }
    }
}
