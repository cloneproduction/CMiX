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
    public partial class RandomRotation : ObservableObject, IBeatModifiable, IPrefab, ISpreadableModifier
    {
        public RandomRotation(PrefabManager beatModifierManager,
                              PrefabService prefabService, 
                              ModifierModeSelector modifierModeSelector, 
                              BeatModifier beatModifier, 
                              Easing easing, 
                              Vector3 rotation)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            BeatModifier = beatModifier;
            Easing = easing;
            Rotation = rotation;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public Vector3 Rotation { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
