// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class GaussianXYZ : ObservableObject, IControl, IBeatModifiable, IPrefab, ISpreadableModifier
    {
        public GaussianXYZ(PrefabService prefabService,
                           BeatModifier beatModifier,
                           Easing easing,
                           ModifierModeSelector modifierModeSelector,
                           GenericValue<bool> randomizeLocation,
                           Vector3 location,
                           GenericValue<bool> randomizeScale,
                           Vector3 scale,
                           GenericValue<bool> randomizeRotation,
                           Vector3 rotation)
        {
            PrefabService = prefabService;
            BeatModifier = beatModifier;
            Easing = easing;
            ModifierModeSelector = modifierModeSelector;

            RandomizeLocation = randomizeLocation;
            Location = location;
            RandomizeScale = randomizeScale;
            Scale = scale;
            RandomizeRotation = randomizeLocation;
            Rotation = rotation;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<bool> RandomizeLocation { get; set; }
        public Vector3 Location { get; set; }
        public GenericValue<bool> RandomizeScale { get; set; }
        public Vector3 Scale { get; set; }
        public GenericValue<bool> RandomizeRotation { get; set; }
        public Vector3 Rotation { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;

        [ObservableProperty]
        private bool randomizeLocationIsExpanded;

        [ObservableProperty]
        private bool randomizeScaleIsExpanded;

        [ObservableProperty]
        private bool randomizeRotationIsExpanded;
    }
}
