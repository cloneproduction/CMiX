// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class AnimParameterModel : IModel
    {
        public AnimParameterModel()
        {
            this.ID = Guid.NewGuid();
            BeatModifierModel = new BeatModifierModel();
            EasingModel = new EasingModel();
            RangeModel = new RangeModel();
            AnimModeModel = new AnimModeModel();
            Width = new SliderModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel Width { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public ModeType SelectedModeType { get; set; }
        public EasingModel EasingModel { get; set; }
        public RangeModel RangeModel { get; set; }
        public BeatModifierModel BeatModifierModel { get; set; }
        public IAnimModeModel AnimModeModel { get; set; }
    }
}
