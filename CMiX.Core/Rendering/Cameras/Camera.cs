// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras
{
    public class Camera : ObservableObject, IPrefab, IModifiable
    {
        public Camera(CameraModel cameraModel)
        {
            ID = cameraModel.ID;
            IsSelected = new BooleanValue(cameraModel.IsSelected);
            IsRenaming = new BooleanValue(cameraModel.IsRenaming);
            Name = new StringValue(cameraModel.Name);
            FOV = new FloatValue(cameraModel.FOV);
            Distance = new FloatValue(cameraModel.Distance);
            Yaw = new FloatValue(cameraModel.Yaw);
            Pitch = new FloatValue(cameraModel.Pitch);
            Target = new Vector3(cameraModel.Target);
            NearClip = new FloatValue(cameraModel.NearClip);
            FarClip = new FloatValue(cameraModel.FarClip);
            Projection = new BooleanValue(cameraModel.Projection);
            ModifierManager = new ModifierManager(cameraModel.CameraTransformModifierManager, new CameraTransformModifierFactory());
        }

        public Guid ID { get; set; }
        public ModifierManager ModifierManager { get; set; }
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
