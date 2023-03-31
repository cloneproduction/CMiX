// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Animation;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Presentations.Modifiers.Camera
{
    public class CameraLFOModel : IModifierModel
    {
        public CameraLFOModel()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValueModel(true);
            Yaw = new BooleanValueModel(false);
            Pitch = new BooleanValueModel(false);
            Zoom = new BooleanValueModel(false);
            PingPong = new BooleanValueModel(false);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            From = new FloatValueModel(0.0f);
            To = new FloatValueModel(1.0f);
            Axis = new GenericValueModel<CameraAxis>(CameraAxis.Zoom);
        }

        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public BooleanValueModel Yaw { get; set; }
        public BooleanValueModel Pitch { get; set; }
        public BooleanValueModel Zoom { get; set; }

        public BooleanValueModel PingPong { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public FloatValueModel From { get; set; }
        public FloatValueModel To { get; set; }
        public GenericValueModel<CameraAxis> Axis { get; set; }

    }
}
