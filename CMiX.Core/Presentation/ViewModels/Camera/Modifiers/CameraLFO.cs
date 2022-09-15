// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels
{
    public class CameraLFO : ICameraModifier
    {
        public CameraLFO(CameraLFOModel lfoModel, CompositionService compositionService)
        {
            ID = lfoModel.ID;
            Enabled = lfoModel.Enabled;

            Visible = new ToggleButton(lfoModel.Visible);
            BeatModifier = new BeatModifier(lfoModel.BeatModifier, compositionService);

            Yaw = new ToggleButton(lfoModel.Yaw);
            Pitch = new ToggleButton(lfoModel.Pitch);
            Zoom = new ToggleButton(lfoModel.Zoom);

            PingPong = new ToggleButton(lfoModel.PingPong);
            TransformType = new ComboBox<CameraTransformType>(lfoModel.TransformType);
            Easing = new Easing(lfoModel.Easing);

            From = new Slider(nameof(From), lfoModel.From);
            To = new Slider(nameof(To), lfoModel.To);
        }


        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }


        public BeatModifier BeatModifier { get; set; }
        public ToggleButton Yaw { get; set; }
        public ToggleButton Pitch { get; set; }
        public ToggleButton Zoom { get; set; }
        public ToggleButton PingPong { get; set; }
        public ComboBox<CameraTransformType> TransformType { get; set; }
        public Easing Easing { get; set; }
        public Slider From { get; set; }
        public Slider To { get; set; }


        public IModel GetModel()
        {
            CameraLFOModel cameraLFOModel = new CameraLFOModel();

            cameraLFOModel.ID = ID;
            cameraLFOModel.Enabled = Enabled;
            cameraLFOModel.Visible = (ToggleButtonModel)Visible.GetModel();
            cameraLFOModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            cameraLFOModel.Yaw = (ToggleButtonModel)Yaw.GetModel();
            cameraLFOModel.Pitch = (ToggleButtonModel)Pitch.GetModel();
            cameraLFOModel.Zoom = (ToggleButtonModel)Zoom.GetModel();
            cameraLFOModel.PingPong = (ToggleButtonModel)PingPong.GetModel();
            cameraLFOModel.TransformType = (ComboBoxModel<CameraTransformType>)TransformType.GetModel();
            cameraLFOModel.Easing = (EasingModel)Easing.GetModel();
            cameraLFOModel.From = (SliderModel)From.GetModel();
            cameraLFOModel.To = (SliderModel)To.GetModel();

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
            TransformType.SetViewModel(cameraLFOModel.TransformType);
            Easing.SetViewModel(cameraLFOModel.Easing);
            From.SetViewModel(cameraLFOModel.From);
            To.SetViewModel(cameraLFOModel.To);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
