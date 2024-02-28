// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public class Composition : IPrefab
    {
        public Composition(PrefabService prefabService, 
                           MasterBeat masterBeat,
                           ReorderablePrefabManager prefabManager, 
                           ReorderablePrefabManager modifierManager, 
                           OutputSettings outputSettings)
        {
            ID = prefabService.ID;

            PrefabService = prefabService;
            Name = prefabService.Name;
            IsSelected = prefabService.IsSelected;
            IsRenaming = prefabService.IsRenaming;
            Visibility = prefabService.Visibility;
            MasterBeat = masterBeat;
            OutputSettings = outputSettings;
            LayerManager = prefabManager;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }

        public GenericValue<string> Name { get; set; }
        public GenericValue<bool> IsSelected { get; set; }
        public GenericValue<bool> IsRenaming { get; set; }
        public GenericValue<bool> Visibility { get; set; }

        public PrefabManager LayerManager { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public MasterBeat MasterBeat { get; set; }
    }
}
