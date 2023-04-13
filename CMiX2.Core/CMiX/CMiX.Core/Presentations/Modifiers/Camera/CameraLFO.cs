// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Modifiers.Camera
{
    public class CameraLFO : ICameraModifier, IBeatModifiable
    {
        public CameraLFO(CameraLFOModel lfoModel, CompositionService compositionService)
        {
            ID = lfoModel.ID;

            Visible = new BooleanValue(lfoModel.Visible, compositionService);
            BeatModifier = new BeatModifier(lfoModel.BeatModifier, compositionService);

            Yaw = new BooleanValue(lfoModel.Yaw, compositionService);
            Pitch = new BooleanValue(lfoModel.Pitch, compositionService);
            Zoom = new BooleanValue(lfoModel.Zoom, compositionService);

            PingPong = new BooleanValue(lfoModel.PingPong, compositionService);
            Axis = new GenericValue<CameraAxis>(lfoModel.Axis, compositionService);
            Easing = new Easing(lfoModel.Easing, compositionService);

            From = new FloatValue(lfoModel.From, compositionService);
            To = new FloatValue(lfoModel.To, compositionService);
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
