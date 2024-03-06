// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public class Composition : IControl, IPrefab
    {
        public Composition(PrefabService prefabService, 
                           MasterBeat masterBeat,
                           PrefabManager prefabManager,
                           PrefabManager modifierManager, 
                           OutputSettings outputSettings)
        {
            ID = prefabService.ID;

            PrefabService = prefabService;
            MasterBeat = masterBeat;
            OutputSettings = outputSettings;
            LayerManager = prefabManager;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }
        public PrefabManager LayerManager { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public MasterBeat MasterBeat { get; set; }
    }
}
