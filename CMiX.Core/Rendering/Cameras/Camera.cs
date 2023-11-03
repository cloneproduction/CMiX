// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Collections;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras
{
    public partial class Camera : ObservableObject, IPrefab, IModifiable
    {
        public Camera(PrefabService prefabService, CameraSettings cameraSettings, ModifierManager modifierManager)
        {
            ID = prefabService.ID;
            IsSelected = prefabService.IsSelected;
            IsRenaming = prefabService.IsRenaming;
            Name = prefabService.Name;
            Visibility = prefabService.Visibility;

            FOV = cameraSettings.FOV;
            Distance = cameraSettings.Distance;
            Yaw = cameraSettings.Yaw;
            Pitch = cameraSettings.Pitch;
            Target = cameraSettings.Target;
            NearClip = cameraSettings.NearClip;
            FarClip = cameraSettings.FarClip;
            Projection = cameraSettings.Projection;

            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public BooleanValue Visibility { get; set; }
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

        public ModifierManager ModifierManager { get; set; }

        [ObservableProperty]
        bool isExpanded = false;
    }
}
