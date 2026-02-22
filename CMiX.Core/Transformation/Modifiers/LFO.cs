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
    public partial class LFO : ObservableObject, ISpreadableModifier, IBeatModifiable
    {
        public LFO(PrefabManager beatModifierManager,
                   PrefabService prefabService,
                   GenericValue<bool> pingPong,
                   GenericValue<float> randomizePhase,
                   DirectionXYZ directionXYZ,
                   GenericValue<TransformType> transformType,
                   GenericValue<float> from,
                   GenericValue<float> to,
                   ModifierModeSelector modifierModeSelector)
        {
            BeatModifierManager = beatModifierManager;
            RandomizePhase = randomizePhase;
            ModifierModeSelector = modifierModeSelector;
            PrefabService = prefabService;
            PingPong = pingPong;
            DirectionXYZ = directionXYZ;
            TransformType = transformType;
            From = from;
            To = to;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<float> RandomizePhase { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
