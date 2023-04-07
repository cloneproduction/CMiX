// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public class Camera : ObservableObject, IPrefab
    {
        public Camera(CameraModel cameraModel, CompositionService compositionService)
        {
            ID = cameraModel.ID;
            Name = new StringValue(cameraModel.Name, compositionService);
            FOV = new FloatValue(cameraModel.FOV, compositionService);
            Distance = new FloatValue(cameraModel.Distance, compositionService);
            Yaw = new FloatValue(cameraModel.Yaw, compositionService);
            Pitch = new FloatValue(cameraModel.Pitch, compositionService);
            Target = new Vector3(cameraModel.Target, compositionService);
            NearClip = new FloatValue(cameraModel.NearClip, compositionService);
            FarClip = new FloatValue(cameraModel.FarClip, compositionService);
            Projection = new BooleanValue(cameraModel.Projection, compositionService);
            CameraTransformModifierManager = new ModifierManager(cameraModel.CameraTransformModifierManager, new CameraTransformModifierFactory(compositionService), compositionService);
        }

        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }
        public ModifierManager CameraTransformModifierManager { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
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
