// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class RandomScale : ObservableObject, IBeatModifiable, IPrefab, ISpreadableModifier
    {
        public RandomScale(PrefabService prefabService, 
                           BeatModifier beatModifier, 
                           Easing easing, 
                           ModifierModeSelector modifierModeSelector, 
                           Vector3 scale, 
                           GenericValue<float> uniformXYZ)
        {
            PrefabService = prefabService;
            BeatModifier = beatModifier;
            Easing = easing;
            ModifierModeSelector = modifierModeSelector;

            Scale = scale;
            UniformXYZ = uniformXYZ; // new GenericValue<float>(0.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public Vector3 Scale { get; set; }
        public GenericValue<float> UniformXYZ { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public PrefabService PrefabService { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
