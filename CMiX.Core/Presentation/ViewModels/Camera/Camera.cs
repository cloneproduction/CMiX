// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Camera : ObservableObject, IPrefab
    {
        public Camera(CameraModel cameraModel, CompositionService compositionService)
        {
            this.ID = cameraModel.ID;

            Name = cameraModel.Name;
            FOV = new Slider(nameof(FOV), cameraModel.FOV);
            Distance = new Slider(nameof(Distance), cameraModel.Distance);

            Yaw = new Slider(nameof(Yaw), cameraModel.Yaw);
            Pitch = new Slider(nameof(Pitch), cameraModel.Pitch);

            TargetX = new Slider(nameof(TargetX), cameraModel.TargetX);
            TargetY = new Slider(nameof(TargetY), cameraModel.TargetY);
            TargetZ = new Slider(nameof(TargetZ), cameraModel.TargetZ);

            NearClip = new Slider(nameof(NearClip), cameraModel.NearClip);
            FarClip = new Slider(nameof(FarClip), cameraModel.FarClip);

            Projection = new ToggleButton(cameraModel.Projection);

            CameraTransformModifierManager = new ModifierManager(cameraModel.CameraTransformModifierManager, new CameraTransformModifierFactory(compositionService));
        }


        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }
        public ModifierManager CameraTransformModifierManager { get; set; }



        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }


        public Slider FOV { get; set; }
        public Slider Distance { get; set; }
        public Slider Yaw { get; set; }
        public Slider Pitch { get; set; }
        public Slider TargetX { get; set; }
        public Slider TargetY { get; set; }
        public Slider TargetZ { get; set; }
        public Slider NearClip { get; set; }
        public Slider FarClip { get; set; }
        public ToggleButton Projection { get; set; }


        public void SetViewModel(IModel model)
        {
            CameraModel cameraModel = model as CameraModel;
            ID = cameraModel.ID;

            FOV.SetViewModel(cameraModel.FOV);
            Distance.SetViewModel(cameraModel.Distance);
            CameraTransformModifierManager.SetViewModel(cameraModel.CameraTransformModifierManager);
            Yaw.SetViewModel(cameraModel.Yaw);
            Pitch.SetViewModel(cameraModel.Pitch);

            TargetX.SetViewModel(cameraModel.TargetX);
            TargetY.SetViewModel(cameraModel.TargetY);
            TargetZ.SetViewModel(cameraModel.TargetZ);

            NearClip.SetViewModel(cameraModel.NearClip);
            FarClip.SetViewModel(cameraModel.FarClip);

            Projection.SetViewModel(cameraModel.Projection);
        }

        public IModel GetModel()
        {
            CameraModel cameraModel = new CameraModel();
            cameraModel.ID = this.ID;
            cameraModel.CameraTransformModifierManager = (ModifierManagerModel)CameraTransformModifierManager.GetModel();
            cameraModel.FOV = (SliderModel)FOV.GetModel();
            cameraModel.Distance = (SliderModel)Distance.GetModel();

            cameraModel.Yaw = (SliderModel)Yaw.GetModel();
            cameraModel.Pitch = (SliderModel)Pitch.GetModel();

            cameraModel.TargetX = (SliderModel)TargetX.GetModel();
            cameraModel.TargetY = (SliderModel)TargetY.GetModel();
            cameraModel.TargetZ = (SliderModel)TargetZ.GetModel();

            cameraModel.NearClip = (SliderModel)NearClip.GetModel();
            cameraModel.FarClip = (SliderModel)FarClip.GetModel();

            cameraModel.Projection = (ToggleButtonModel)Projection.GetModel();

            return cameraModel;
        }
    }
}
