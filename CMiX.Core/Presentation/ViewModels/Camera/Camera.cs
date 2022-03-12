// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Camera : ObservableObject, IControl
    {
        public Camera(CameraModel cameraModel)
        {
            this.ID = cameraModel.ID;

            FOV = new Slider(nameof(FOV), cameraModel.FOV);
            Distance = new Slider(nameof(Distance), cameraModel.Distance);

            Yaw = new Slider(nameof(Yaw), cameraModel.Yaw);
            Pitch = new Slider(nameof(Pitch), cameraModel.Pitch);

            TargetX = new Slider(nameof(TargetX), cameraModel.TargetX);
            TargetY = new Slider(nameof(TargetY), cameraModel.TargetY);
            TargetZ = new Slider(nameof(TargetZ), cameraModel.TargetZ);
        }


        public Guid ID { get; set; }

        public Slider FOV { get; set; }
        public Slider Distance { get; set; }
        public Slider Yaw { get; set; }
        public Slider Pitch { get; set; }
        public Slider TargetX { get; set; }
        public Slider TargetY { get; set; }
        public Slider TargetZ { get; set; }


        public void SetViewModel(IModel model)
        {
            CameraModel cameraModel = model as CameraModel;
            this.ID = cameraModel.ID;

            this.FOV.SetViewModel(cameraModel.FOV);
            this.Distance.SetViewModel(cameraModel.Distance);

            this.Yaw.SetViewModel(cameraModel.Yaw);
            this.Pitch.SetViewModel(cameraModel.Pitch);

            this.TargetX.SetViewModel(cameraModel.TargetX);
            this.TargetY.SetViewModel(cameraModel.TargetY);
            this.TargetZ.SetViewModel(cameraModel.TargetZ);
        }

        public IModel GetModel()
        {
            CameraModel cameraModel = new CameraModel();
            cameraModel.ID = this.ID;

            cameraModel.FOV = (SliderModel)FOV.GetModel();
            cameraModel.Distance = (SliderModel)Distance.GetModel();

            cameraModel.Yaw = (SliderModel)Yaw.GetModel();
            cameraModel.Pitch = (SliderModel)Pitch.GetModel();

            cameraModel.TargetX = (SliderModel)TargetX.GetModel();
            cameraModel.TargetY = (SliderModel)TargetY.GetModel();
            cameraModel.TargetZ = (SliderModel)TargetZ.GetModel();
            return cameraModel;
        }
    }
}
