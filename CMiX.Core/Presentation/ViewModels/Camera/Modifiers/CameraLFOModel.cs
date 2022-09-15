// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class CameraLFOModel : IModel
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
            TransformType = new ComboBoxModel<CameraTransformType>(CameraTransformType.Pitch);
        }
        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public ToggleButtonModel Visible { get; internal set; }
        public ToggleButtonModel Yaw { get; internal set; }
        public ToggleButtonModel Pitch { get; internal set; }
        public ToggleButtonModel Zoom { get; internal set; }

        public ToggleButtonModel PingPong { get; internal set; }
        public BeatModifierModel BeatModifier { get; internal set; }
        public EasingModel Easing { get; internal set; }
        public SliderModel From { get; internal set; }
        public SliderModel To { get; internal set; }
        public ComboBoxModel<CameraTransformType> TransformType { get; internal set; }

    }
}
