// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Lights
{
    public partial class LightEntity : ObservableObject, IPrefab
    {
        public LightEntity(PrefabService prefabService, LightSettings lightSettings, ReorderablePrefabManager prefabManager)
        {
            PrefabService = prefabService;
            Name = prefabService.Name;
            IsRenaming = prefabService.IsRenaming;
            IsSelected = prefabService.IsSelected;
            Visibility = prefabService.Visibility;

            LightColor = lightSettings.LightColor;
            Position = lightSettings.Position;
            Target = lightSettings.Target;
            Radius = lightSettings.Radius;
            Angle = lightSettings.Angle;
            Softness = lightSettings.Softness;
            Intensity = lightSettings.Intensity;
            LightTypeSelector = lightSettings.LightTypeSelector;

            ModifierManager = prefabManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> Visibility { get; set; }
        public GenericValue<string> Name { get; set; }
        public GenericValue<bool> IsRenaming { get; set; }
        public GenericValue<bool> IsSelected { get; set; }
        public GenericValue<LightType> LightTypeSelector { get; set; }
        public GenericValue<string> LightColor { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public GenericValue<float> Radius { get; set; }
        public GenericValue<float> Angle { get; set; }
        public GenericValue<float> Softness { get; set; }
        public GenericValue<float> Intensity { get; set; }


        public ReorderablePrefabManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool settingsIsExpanded;

        [ObservableProperty]
        private bool modifierIsExpanded;
    }
}
