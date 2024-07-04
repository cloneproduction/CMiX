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
    public partial class RandomPosition : ObservableObject, IPrefab, IBeatModifiable, ISpreadableModifier
    {
        public RandomPosition(PrefabManager beatModifierManager,
                              PrefabService prefabService, 
                              ModifierModeSelector modifierModeSelector, 
                              Vector3 location)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            Location = location;
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Vector3 Location { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
