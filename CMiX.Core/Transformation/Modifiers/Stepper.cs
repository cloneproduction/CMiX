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
    public partial class Stepper : ObservableObject, IBeatModifiable, IPrefab, ISpreadableModifier
    {
        public Stepper(PrefabManager beatModifierManager,
                       ModifierModeSelector modifierModeSelector, 
                       GenericValue<int> stepCount, 
                       PrefabService prefabService, 
                       BeatModifier beatModifier,
                       GenericValue<bool> pingPong,
                       GenericValue<TransformType> transformType,
                       DirectionXYZ directionXYZ,
                       Easing easing,
                       GenericValue<float> from,
                       GenericValue<float> to)
        {
            BeatModifierManager = beatModifierManager;
            ModifierModeSelector = modifierModeSelector;
            StepCount = stepCount;
            PrefabService = prefabService;
            BeatModifier = beatModifier;
            PingPong = pingPong;
            TransformType = transformType;
            DirectionXYZ = directionXYZ;
            Easing = easing;
            From = from;
            To = to;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<int> StepCount { get; set; }
        public GenericValue<bool> Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
