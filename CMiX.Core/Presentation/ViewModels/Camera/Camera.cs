// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Camera : ObservableObject, IControl
    {
        public Camera(MasterBeat beat, CameraModel cameraModel)
        {
            this.ID = cameraModel.ID;
            Rotation = ((CameraRotation)0).ToString();
            LookAt = ((CameraLookAt)0).ToString();
            View = ((CameraView)0).ToString();

            BeatModifier = new BeatModifier(new BeatModifierModel());
            FOV = new Slider(nameof(FOV), cameraModel.FOV);
            Zoom = new Slider(nameof(Zoom), cameraModel.Zoom);
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Slider FOV { get; set; }
        public Slider Zoom { get; set; }


        private string _rotation;
        public string Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        private string _lookAt;
        public string LookAt
        {
            get => _lookAt;
            set => SetProperty(ref _lookAt, value);
        }

        private string _view;
        public string View
        {
            get => _view;
            set => SetProperty(ref _view, value);
        }


        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }


        public void SetViewModel(IModel model)
        {
            CameraModel cameraModel = model as CameraModel;
            this.ID = cameraModel.ID;
            this.Rotation = cameraModel.Rotation;
            this.LookAt = cameraModel.LookAt;
            this.View = cameraModel.View;
            this.BeatModifier.SetViewModel(cameraModel.BeatModifierModel);
            this.FOV.SetViewModel(cameraModel.FOV);
            this.Zoom.SetViewModel(cameraModel.Zoom);
        }

        public IModel GetModel()
        {
            CameraModel cameraModel = new CameraModel();
            cameraModel.ID = this.ID;
            cameraModel.Rotation = this.Rotation;
            cameraModel.LookAt = this.LookAt;
            cameraModel.View = this.View;
            return cameraModel;
        }
    }
}
