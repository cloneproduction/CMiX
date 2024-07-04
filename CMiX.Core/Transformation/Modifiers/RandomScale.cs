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
    public partial class RandomScale : ObservableObject, IBeatModifiable, IPrefab, ISpreadableModifier
    {
        public RandomScale(PrefabManager beatModifierManager,
                           PrefabService prefabService, 
                           ModifierModeSelector modifierModeSelector, 
                           Vector3 scale, 
                           GenericValue<float> uniformXYZ)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            Scale = scale;
            UniformXYZ = uniformXYZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Vector3 Scale { get; set; }
        public GenericValue<float> UniformXYZ { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
