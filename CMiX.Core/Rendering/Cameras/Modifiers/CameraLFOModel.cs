// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public class CameraLFOModel : IModifierModel
    {
        public CameraLFOModel()
        {
            ID = Guid.NewGuid();

            Visible = new GenericValueModel<bool>(true);
            Yaw = new GenericValueModel<bool>(false);
            Pitch = new GenericValueModel<bool>(false);
            Zoom = new GenericValueModel<bool>(false);
            PingPong = new GenericValueModel<bool>(false);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            From = new GenericValueModel<float>(0.0f);
            To = new GenericValueModel<float>(1.0f);
            Axis = new GenericValueModel<CameraAxis>(CameraAxis.Zoom);
        }

        public Guid ID { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
        public GenericValueModel<bool> Yaw { get; set; }
        public GenericValueModel<bool> Pitch { get; set; }
        public GenericValueModel<bool> Zoom { get; set; }
        public GenericValueModel<bool> PingPong { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<float> From { get; set; }
        public GenericValueModel<float> To { get; set; }
        public GenericValueModel<CameraAxis> Axis { get; set; }
    }
}
