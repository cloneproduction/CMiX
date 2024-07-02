// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Colors.Modifiers
{
    public partial class RandomHSV : ObservableObject, IControl, IBeatModifiable, IPrefab, ISpreadableModifier
    {
        public RandomHSV(PrefabManager beatModifierManager,
                         PrefabService prefabService, 
                         BeatModifier beatModifier, 
                         Easing easing, 
                         ModifierModeSelector modifierModeSelector, 
                         GenericValue<float> hue, 
                         GenericValue<float> saturation, 
                         GenericValue<float> value, 
                         GenericValue<float> alpha)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            BeatModifier = beatModifier;
            Easing = easing;
            ModifierModeSelector = modifierModeSelector;
            Hue = hue;
            Saturation = saturation;
            Value = value;
            Alpha = alpha;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Saturation { get; set; }
        public GenericValue<float> Value { get; set; }
        public GenericValue<float> Alpha { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
