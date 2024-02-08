// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class RandomUV : ObservableObject, IBeatModifiable, ITextureModifier
    {
        public RandomUV(GenericValue<bool> visible, 
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
            Visible = visible;
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
        public GenericValue<bool> Visible { get; set; }
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
