// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras
{
    public partial class Camera : ObservableObject, IPrefab, IModifiable
    {
        public Camera(PrefabService prefabService, CameraSettings cameraSettings, ReorderablePrefabManager modifierManager)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;

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

        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> Visibility { get; set; }
        public GenericValue<bool> IsSelected { get; set; }
        public GenericValue<bool> IsRenaming { get; set; }
        public GenericValue<string> Name { get; set; }

        public GenericValue<float> FOV { get; set; }
        public GenericValue<float> Distance { get; set; }
        public GenericValue<float> Yaw { get; set; }
        public GenericValue<float> Pitch { get; set; }
        public Vector3 Target { get; set; }
        public GenericValue<float> NearClip { get; set; }
        public GenericValue<float> FarClip { get; set; }
        public GenericValue<bool> Projection { get; set; }

        public ReorderablePrefabManager ModifierManager { get; set; }

        [ObservableProperty]
        bool isExpanded = false;
    }
}
