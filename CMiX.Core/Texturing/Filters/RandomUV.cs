// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Sampling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class RandomUV : ObservableObject, IBeatModifiable, ITextureModifier
    {
        public RandomUV()
        {
            Visible = new BooleanValue(true);
            BeatModifier = new BeatModifier();
            Easing = new Easing();
            RandomizeLocation = new BooleanValue(true);
            Location = new Vector2(0.0f, 0.0f);
            RandomizeScale = new BooleanValue(true);
            Scale = new Vector2(1.0f, 1.0f);
            Uniform = new FloatValue(0.0f);
            RandomizeRotation = new BooleanValue(true);
            Rotation = new FloatValue(0.0f);
            SamplerState = new SamplerState();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public BooleanValue RandomizeLocation { get; set; }
        public Vector2 Location { get; set; }
        public BooleanValue RandomizeScale { get; set; }
        public Vector2 Scale { get; set; }
        public FloatValue Uniform { get; set; }
        public BooleanValue RandomizeRotation { get; set; }
        public FloatValue Rotation { get; set; }
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
