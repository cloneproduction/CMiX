// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Rendering.Cameras
{
    public partial class Camera : ObservableObject, IPrefab, IModifiable, IDisposable
    {
        public Camera(PrefabService prefabService, 
                      CameraSettings settings,
                      PrefabManager modifierManager)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Settings = settings;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public CameraSettings Settings { get; set; }
        public PrefabManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool modifierManagerIsExpanded = true;

        [ObservableProperty]
        private bool settingsIsExpanded = true;

        public IControlModel ToModel() => new CameraModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Settings = (CameraSettingsModel)Settings.ToModel(),
            ModifierManager = (PrefabManagerModel)ModifierManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (CameraModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Settings.FromModel(m.Settings);

            LoadManager(ModifierManager, m.ModifierManager);
        }

        public void Dispose()
        {
            ModifierManager.Dispose();
        }
    }
}
