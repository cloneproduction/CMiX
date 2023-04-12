// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Modifiers;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class RandomUV : ObservableObject, IBeatModifiable, ITextureFilter
    {
        public RandomUV(RandomUVModel randomUVModel, CompositionService compositionService)
        {
            this.ID = randomUVModel.ID;
            this.Name = randomUVModel.Name;
            isExpanded = true;

            Visible = new BooleanValue(randomUVModel.Visible, compositionService);

            Easing = new Easing(randomUVModel.EasingModel, compositionService);
            BeatModifier = new BeatModifier(randomUVModel.BeatModifierModel, compositionService);
            SamplerState = new SamplerState(randomUVModel.SamplerState, compositionService);
            RandomizeLocation = new BooleanValue(randomUVModel.RandomizeLocation, compositionService);
            Location = new Vector2(randomUVModel.Location, compositionService);
            RandomizeScale = new BooleanValue(randomUVModel.RandomizeScale, compositionService);
            Uniform = new FloatValue(randomUVModel.Uniform, compositionService);
            Scale = new Vector2(randomUVModel.Scale, compositionService);
            RandomizeRotation = new BooleanValue(randomUVModel.RandomizeScale, compositionService);
            Rotation = new FloatValue(randomUVModel.Rotation, compositionService);
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
