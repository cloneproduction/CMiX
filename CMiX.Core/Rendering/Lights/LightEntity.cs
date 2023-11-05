// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Lights
{
    public partial class LightEntity : ObservableObject, IPrefab
    {
        public LightEntity(PrefabService prefabService, LightSettings lightSettings, ModifierManager modifierManager)
        {
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

            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visibility { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }


        public GenericValue<LightType> LightTypeSelector { get; set; }
        public ColorValue LightColor { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public FloatValue Radius { get; set; }
        public FloatValue Angle { get; set; }
        public FloatValue Softness { get; set; }
        public FloatValue Intensity { get; set; }


        public ModifierManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool settingsIsExpanded;

        [ObservableProperty]
        private bool modifierIsExpanded;
    }
}
