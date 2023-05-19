// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public class CameraLFO : IModifier, IBeatModifiable
    {
        public CameraLFO(CameraLFOModel lfoModel)
        {
            ID = lfoModel.ID;
            Visible = new BooleanValue(lfoModel.Visible);
            BeatModifier = new BeatModifier(lfoModel.BeatModifier);
            Yaw = new BooleanValue(lfoModel.Yaw);
            Pitch = new BooleanValue(lfoModel.Pitch);
            Zoom = new BooleanValue(lfoModel.Zoom);
            PingPong = new BooleanValue(lfoModel.PingPong);
            Axis = new GenericValue<CameraAxis>(lfoModel.Axis);
            Easing = new Easing(lfoModel.Easing);
            From = new FloatValue(lfoModel.From);
            To = new FloatValue(lfoModel.To);
        }


        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public BooleanValue Yaw { get; set; }
        public BooleanValue Pitch { get; set; }
        public BooleanValue Zoom { get; set; }
        public BooleanValue PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public Easing Easing { get; set; }
        public FloatValue From { get; set; }
        public FloatValue To { get; set; }


        public void Dispose()
        {

        }
    }
}
