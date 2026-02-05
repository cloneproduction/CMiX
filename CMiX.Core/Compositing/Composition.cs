// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Composition : ObservableObject, IControl, IPrefab, ITextureModifiable, IModifiable
    {
        public Composition(PrefabService prefabService, 
                           MasterBeat masterBeat,
                           PrefabManager prefabManager,
                           PrefabManager textureModifierManager, 
                           PrefabManager modifierManager,
                           OutputSettings outputSettings)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            MasterBeat = masterBeat;
            OutputSettings = outputSettings;
            LayerManager = prefabManager;
            ModifierManager = modifierManager;

            TextureModifierManager = textureModifierManager;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager LayerManager { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public MasterBeat MasterBeat { get; set; }
        public PrefabManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool textureModifierIsExpanded = true;

        [ObservableProperty]
        private bool outputSettingsIsExpanded = true;
    }
}
