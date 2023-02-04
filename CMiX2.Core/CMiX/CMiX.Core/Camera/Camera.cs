// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.BaseControl;
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

            Target = new Vector3(cameraModel.Target);

            NearClip = new FloatValue(cameraModel.NearClip);
            FarClip = new FloatValue(cameraModel.FarClip);

            Projection = new BooleanValue(cameraModel.Projection);

            CameraTransformModifierManager = new ModifierManager(cameraModel.CameraTransformModifierManager, new CameraTransformModifierFactory(compositionService), compositionService);
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
        public Vector3 Target { get; set; }
        public FloatValue NearClip { get; set; }
        public FloatValue FarClip { get; set; }
        public BooleanValue Projection { get; set; }

    }
}
