// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras
{
    public partial class Camera : ObservableObject, IPrefab, IModifiable
    {
        public Camera(PrefabService prefabService, 
                      CameraSettings settings, 
                      ReorderablePrefabManager modifierManager)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Settings = settings;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public CameraSettings Settings { get; set; }
        public PrefabManagerBase ModifierManager { get; set; }

        [ObservableProperty]
        bool isExpanded = false;
    }
}
