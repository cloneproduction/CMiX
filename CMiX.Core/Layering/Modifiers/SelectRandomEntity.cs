// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Layering.Modifiers
{
    public class SelectRandomEntity : ObservableObject, IControl, IBeatModifiable, IPrefab
    {
        public SelectRandomEntity(PrefabManager beatModifierManager,
                                  PrefabService prefabService,
                                  BeatModifier beatModifier,
                                  Easing easing)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            BeatModifier = beatModifier;
            Easing = easing;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public Easing Easing { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
    }
}
