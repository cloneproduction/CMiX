// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class CameraLFOModel : IModifierModel
    {
        public CameraLFOModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            Visible = new ToggleButtonModel(true);
            Yaw = new ToggleButtonModel(false);
            Pitch = new ToggleButtonModel(false);
            Zoom = new ToggleButtonModel(false);
            PingPong = new ToggleButtonModel(false);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            From = new SliderModel(0.0f);
            To = new SliderModel(1.0f);
            Axis = new ComboBoxModel<CameraAxis>(CameraAxis.Zoom);
        }

        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public ToggleButtonModel Yaw { get; set; }
        public ToggleButtonModel Pitch { get; set; }
        public ToggleButtonModel Zoom { get; set; }

        public ToggleButtonModel PingPong { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public SliderModel From { get; set; }
        public SliderModel To { get; set; }
        public ComboBoxModel<CameraAxis> Axis { get; set; }

    }
}
