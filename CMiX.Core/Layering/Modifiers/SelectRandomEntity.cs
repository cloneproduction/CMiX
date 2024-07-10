// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Layering.Modifiers
{
    public class SelectRandomEntity : ObservableObject, IBeatModifiable, IPrefab
    {
        public SelectRandomEntity(PrefabManager beatModifierManager,
                                  PrefabService prefabService)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
    }
}
