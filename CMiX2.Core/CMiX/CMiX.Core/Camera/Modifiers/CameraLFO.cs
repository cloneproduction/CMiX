// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels
{
    public class CameraLFO : ICameraModifier, IBeatModifiable
    {
        public CameraLFO(CameraLFOModel lfoModel, CompositionService compositionService)
        {
            ID = lfoModel.ID;

            Visible = new BooleanValue(lfoModel.Visible);
            BeatModifier = new BeatModifier(lfoModel.BeatModifier, compositionService);

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


        public IModel GetModel()
        {
            CameraLFOModel cameraLFOModel = new CameraLFOModel();

            cameraLFOModel.ID = ID;
            cameraLFOModel.Visible = (BooleanValueModel)Visible.GetModel();
            cameraLFOModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            cameraLFOModel.Yaw = (BooleanValueModel)Yaw.GetModel();
            cameraLFOModel.Pitch = (BooleanValueModel)Pitch.GetModel();
            cameraLFOModel.Zoom = (BooleanValueModel)Zoom.GetModel();
            cameraLFOModel.PingPong = (BooleanValueModel)PingPong.GetModel();
            cameraLFOModel.Axis = (GenericValueModel<CameraAxis>)Axis.GetModel();
            cameraLFOModel.Easing = (EasingModel)Easing.GetModel();
            cameraLFOModel.From = (FloatValueModel)From.GetModel();
            cameraLFOModel.To = (FloatValueModel)To.GetModel();

            return cameraLFOModel;
        }

        public void SetViewModel(IModel model)
        {
            CameraLFOModel cameraLFOModel = model as CameraLFOModel;

            ID = cameraLFOModel.ID;
            Visible.SetViewModel(cameraLFOModel.Visible);
            BeatModifier.SetViewModel(cameraLFOModel.BeatModifier);
            Yaw.SetViewModel(cameraLFOModel.Yaw);
            Pitch.SetViewModel(cameraLFOModel.Pitch);
            Zoom.SetViewModel(cameraLFOModel.Zoom);
            PingPong.SetViewModel(cameraLFOModel.PingPong);
            Axis.SetViewModel(cameraLFOModel.Axis);
            Easing.SetViewModel(cameraLFOModel.Easing);
            From.SetViewModel(cameraLFOModel.From);
            To.SetViewModel(cameraLFOModel.To);
        }

        public void Dispose()
        {

        }
    }
}
