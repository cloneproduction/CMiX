// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class RandomUV : ObservableObject, IPrefab, IBeatModifiable, ITextureModifier
    {
        public RandomUV(PrefabManager beatModifierManager,
                        PrefabService prefabService, 
                        BeatModifier beatModifier,
                        Easing easing,
                        GenericValue<bool> randomizeLocation,
                        Vector2 location,
                        GenericValue<bool> randomizeScale,
                        Vector2 scale,
                        GenericValue<float> uniform,
                        GenericValue<bool> randomizeRotation,
                        GenericValue<float> rotation,
                        SamplerState samplerState)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            BeatModifier = beatModifier;
            Easing = easing;
            RandomizeLocation = randomizeLocation;
            Location = location;
            RandomizeScale = randomizeScale;
            Scale = scale;
            Uniform = uniform;
            RandomizeRotation = randomizeRotation;
            Rotation = rotation;
            SamplerState = samplerState;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<bool> RandomizeLocation { get; set; }
        public Vector2 Location { get; set; }
        public GenericValue<bool> RandomizeScale { get; set; }
        public Vector2 Scale { get; set; }
        public GenericValue<float> Uniform { get; set; }
        public GenericValue<bool> RandomizeRotation { get; set; }
        public GenericValue<float> Rotation { get; set; }
        public SamplerState SamplerState { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

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
