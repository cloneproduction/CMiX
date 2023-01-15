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
            FOV = new FloatValue(cameraModel.FOV);
            Distance = new FloatValue(cameraModel.Distance);

            Yaw = new FloatValue(cameraModel.Yaw);
            Pitch = new FloatValue(cameraModel.Pitch);

            TargetX = new FloatValue(cameraModel.TargetX);
            TargetY = new FloatValue(cameraModel.TargetY);
            TargetZ = new FloatValue(cameraModel.TargetZ);

            NearClip = new FloatValue(cameraModel.NearClip);
            FarClip = new FloatValue(cameraModel.FarClip);

            Projection = new BooleanValue(cameraModel.Projection);

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


        public FloatValue FOV { get; set; }
        public FloatValue Distance { get; set; }
        public FloatValue Yaw { get; set; }
        public FloatValue Pitch { get; set; }
        public FloatValue TargetX { get; set; }
        public FloatValue TargetY { get; set; }
        public FloatValue TargetZ { get; set; }
        public FloatValue NearClip { get; set; }
        public FloatValue FarClip { get; set; }
        public BooleanValue Projection { get; set; }


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
            cameraModel.FOV = (FloatValueModel)FOV.GetModel();
            cameraModel.Distance = (FloatValueModel)Distance.GetModel();

            cameraModel.Yaw = (FloatValueModel)Yaw.GetModel();
            cameraModel.Pitch = (FloatValueModel)Pitch.GetModel();

            cameraModel.TargetX = (FloatValueModel)TargetX.GetModel();
            cameraModel.TargetY = (FloatValueModel)TargetY.GetModel();
            cameraModel.TargetZ = (FloatValueModel)TargetZ.GetModel();

            cameraModel.NearClip = (FloatValueModel)NearClip.GetModel();
            cameraModel.FarClip = (FloatValueModel)FarClip.GetModel();

            cameraModel.Projection = (BooleanValueModel)Projection.GetModel();

            return cameraModel;
        }
    }
}
