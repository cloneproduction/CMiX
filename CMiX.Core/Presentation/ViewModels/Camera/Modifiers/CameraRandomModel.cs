// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class CameraRandomModel : IModifierModel
    {
        public CameraRandomModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            Visible = new ToggleButtonModel(true);
            PingPong = new ToggleButtonModel(false);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            Width = new SliderModel(0.0f);
            To = new SliderModel(1.0f);
            Axis = new ComboBoxModel<CameraAxis>(CameraAxis.Zoom);
        }

        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public ToggleButtonModel Visible { get; set; }

        public ToggleButtonModel PingPong { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public SliderModel Width { get; set; }
        public SliderModel To { get; set; }
        public ComboBoxModel<CameraAxis> Axis { get; set; }

    }
}
