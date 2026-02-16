// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Lights
{
    public partial class LightEntity : ObservableObject, IPrefab, IModifiable
    {
        public LightEntity(PrefabService prefabService,
                           LightSettings settings,
                           PrefabManager modifierManager)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Settings = settings;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public LightSettings Settings { get; set; }
        public PrefabManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool modifierManagerIsExpanded = true;

        [ObservableProperty]
        private bool settingsIsExpanded = true;
    }
}
