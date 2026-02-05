// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class Flip : ObservableObject, IModifier, IBeatModifiable
    {
        public Flip(PrefabService prefabService,
                    PrefabManager beatModifierManager,
                    DirectionXYZ directionXYZ)
        {
            PrefabService = prefabService;
            BeatModifierManager = beatModifierManager;
            DirectionXYZ = directionXYZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
