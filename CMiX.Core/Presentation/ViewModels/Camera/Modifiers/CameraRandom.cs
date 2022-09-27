// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels
{
    public class CameraRandom : ICameraModifier, IBeatModifiable
    {
        public CameraRandom(CameraRandomModel randomModel, CompositionService compositionService)
        {
            ID = randomModel.ID;
            Enabled = randomModel.Enabled;

            Visible = new ToggleButton(randomModel.Visible);
            BeatModifier = new BeatModifier(randomModel.BeatModifier, compositionService);

            PingPong = new ToggleButton(randomModel.PingPong);
            Axis = new ComboBox<CameraAxis>(randomModel.Axis);
            Easing = new Easing(randomModel.Easing);

            From = new Slider(nameof(From), randomModel.From);
            To = new Slider(nameof(To), randomModel.To);
        }


        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }


        public BeatModifier BeatModifier { get; set; }
        public ToggleButton PingPong { get; set; }
        public ComboBox<CameraAxis> Axis { get; set; }
        public Easing Easing { get; set; }
        public Slider From { get; set; }
        public Slider To { get; set; }


        public IModel GetModel()
        {
            CameraRandomModel cameraRandomModel = new CameraRandomModel();

            cameraRandomModel.ID = ID;
            cameraRandomModel.Enabled = Enabled;
            cameraRandomModel.Visible = (ToggleButtonModel)Visible.GetModel();
            cameraRandomModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            cameraRandomModel.PingPong = (ToggleButtonModel)PingPong.GetModel();
            cameraRandomModel.Axis = (ComboBoxModel<CameraAxis>)Axis.GetModel();
            cameraRandomModel.Easing = (EasingModel)Easing.GetModel();
            cameraRandomModel.From = (SliderModel)From.GetModel();
            cameraRandomModel.To = (SliderModel)To.GetModel();

            return cameraRandomModel;
        }

        public void SetViewModel(IModel model)
        {
            CameraRandomModel cameraRandomModel = model as CameraRandomModel;

            ID = cameraRandomModel.ID;
            Visible.SetViewModel(cameraRandomModel.Visible);
            BeatModifier.SetViewModel(cameraRandomModel.BeatModifier);
            PingPong.SetViewModel(cameraRandomModel.PingPong);
            Axis.SetViewModel(cameraRandomModel.Axis);
            Easing.SetViewModel(cameraRandomModel.Easing);
            From.SetViewModel(cameraRandomModel.From);
            To.SetViewModel(cameraRandomModel.To);
        }

        public void Dispose()
        {

        }
    }
}
