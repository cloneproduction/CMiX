// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class LinearXYZModel : IModifierModel
    {
        public LinearXYZModel()
        {
            this.ID = Guid.NewGuid();
            Name = TransformModifierNames.LinearXYZ;

            Visible = new ToggleButtonModel(true);
            BeatModifierModel = new BeatModifierModel();
            CounterModel = new CounterModel();
            Width = new SliderModel();
            Phase = new SliderModel();
            DirectionXYZModel = new DirectionXYZModel();
        }

        public TransformModifierNames Name { get; set; }
        public Guid ID { get; set; }
        public int Count { get; set; }
        public bool Enabled { get; set; }

        public ToggleButtonModel Visible { get; set; }
        public BeatModifierModel BeatModifierModel { get; set; }
        public CounterModel CounterModel { get; set; }
        public SliderModel Width { get; set; }
        public DirectionXYZModel DirectionXYZModel { get; set; }
        public SliderModel Phase { get; set; }

    }
}
