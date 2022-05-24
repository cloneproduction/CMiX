// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

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
            CounterModel = new CounterModel(1);
            Width = new SliderModel();
            Phase = new SliderModel();
            DirectionXYZModel = new DirectionXYZModel();
            Mode = new ComboBoxModel<ModifierMode>();
            Mode.Selection = ModifierMode.ToSpread;
            TransformTypeSelector = new ComboBoxModel<TransformType>(TransformType.Translate);
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
        public ComboBoxModel<ModifierMode> Mode { get; internal set; }
        public ComboBoxModel<TransformType> TransformTypeSelector { get; internal set; }
    }
}
