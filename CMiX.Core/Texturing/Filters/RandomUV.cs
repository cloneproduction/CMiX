// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Transformation.Modifiers;
using CMiX.Core.ViewModels;
using CMiX.Core.Texturing.Sampling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class RandomUV : ObservableObject, IBeatModifiable, ITextureFilter, IEase
    {
        public RandomUV(RandomUVModel randomUVModel)
        {
            ID = randomUVModel.ID;
            Name = randomUVModel.Name;
            isExpanded = true;

            Visible = new BooleanValue(randomUVModel.Visible);

            Easing = new Easing(randomUVModel.EasingModel);
            BeatModifier = new BeatModifier(randomUVModel.BeatModifierModel);
            SamplerState = new SamplerState(randomUVModel.SamplerState);
            RandomizeLocation = new BooleanValue(randomUVModel.RandomizeLocation);
            Location = new Vector2(randomUVModel.Location);
            RandomizeScale = new BooleanValue(randomUVModel.RandomizeScale);
            Uniform = new FloatValue(randomUVModel.Uniform);
            Scale = new Vector2(randomUVModel.Scale);
            RandomizeRotation = new BooleanValue(randomUVModel.RandomizeScale);
            Rotation = new FloatValue(randomUVModel.Rotation);
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public TextureFilterName Name { get; set; }
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
        private bool isExpanded;

        [ObservableProperty]
        private bool randomizeLocationIsExpanded;

        [ObservableProperty]
        private bool randomizeScaleIsExpanded;

        [ObservableProperty]
        private bool randomizeRotationIsExpanded;


        private ModifierMode _selectedModifierType;
        public ModifierMode SelectedModifierType
        {
            get => _selectedModifierType;
            set => SetProperty(ref _selectedModifierType, value);
        }

        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
