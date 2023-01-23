// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class RandomUV : ObservableObject, IBeatModifiable, ITextureFilter
    {
        public RandomUV(RandomUVModel randomUVModel, CompositionService compositionService)
        {
            this.ID = randomUVModel.ID;
            this.Name = randomUVModel.Name;
            IsExpanded = true;

            Visible = new BooleanValue(randomUVModel.Visible);

            Easing = new Easing(randomUVModel.EasingModel);
            BeatModifier = new BeatModifier(randomUVModel.BeatModifierModel, compositionService);
            SamplerState = new SamplerState(randomUVModel.SamplerState, compositionService);
            RandomizeLocation = new BooleanValue(randomUVModel.RandomizeLocation);
            Location = new Vector2(randomUVModel.Location);

            RandomizeScale = new BooleanValue(randomUVModel.RandomizeScale);
            Uniform = new FloatValue(randomUVModel.Uniform);
            Scale = new Vector2(randomUVModel.Scale);

            RandomizeRotation = new BooleanValue(randomUVModel.RandomizeScale);
            Rotation = new FloatValue(randomUVModel.Rotation);
        }


        public BooleanValue Visible { get; set; }


        public Guid ID { get; set; }
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


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _randomizeLocationIsExpanded;
        public bool RandomizeLocationIsExpanded
        {
            get => _randomizeLocationIsExpanded;
            set => SetProperty(ref _randomizeLocationIsExpanded, value);
        }

        private bool _randomizeScaleIsExpanded;
        public bool RandomizeScaleIsExpanded
        {
            get => _randomizeScaleIsExpanded;
            set => SetProperty(ref _randomizeScaleIsExpanded, value);
        }

        private bool _randomizeRotationIsExpanded;
        public bool RandomizeRotationIsExpanded
        {
            get => _randomizeRotationIsExpanded;
            set => SetProperty(ref _randomizeRotationIsExpanded, value);
        }

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


        public void SetViewModel(IModel model)
        {
            RandomUVModel randomUVModel = model as RandomUVModel;
            this.ID = randomUVModel.ID;
            this.Name = randomUVModel.Name;
 
            this.Visible.SetViewModel(randomUVModel.Visible);

            this.BeatModifier.SetViewModel(randomUVModel.BeatModifierModel);
            this.Easing.SetViewModel(randomUVModel.EasingModel);

            this.Location.SetViewModel(randomUVModel.Location);
            this.RandomizeLocation.SetViewModel(randomUVModel.RandomizeLocation);

            this.Scale.SetViewModel(randomUVModel.Scale);
            this.Uniform.SetViewModel(randomUVModel.Uniform);
            this.RandomizeScale.SetViewModel(randomUVModel.RandomizeLocation);

            this.Rotation.SetViewModel(randomUVModel.Rotation);

            this.RandomizeRotation.SetViewModel(randomUVModel.RandomizeLocation);

            this.SamplerState.SetViewModel(randomUVModel.SamplerState);
        }

        public IModel GetModel()
        {
            RandomUVModel model = new RandomUVModel();
            model.ID = this.ID;
            model.Name = this.Name;

            model.Visible = (BooleanValueModel)this.Visible.GetModel();

            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.EasingModel = (EasingModel)this.Easing.GetModel();

            model.Location = (Vector2Model)this.Location.GetModel();

            model.RandomizeLocation = (BooleanValueModel)this.RandomizeLocation.GetModel();

            model.Scale = (Vector2Model)this.Scale.GetModel();
            model.Uniform = (FloatValueModel)this.Uniform.GetModel();
            model.RandomizeScale = (BooleanValueModel)this.RandomizeLocation.GetModel();

            model.Rotation = (FloatValueModel)this.Rotation.GetModel();

            model.RandomizeRotation = (BooleanValueModel)this.RandomizeLocation.GetModel();

            model.SamplerState = (SamplerStateModel)this.SamplerState.GetModel();
            return model;
        }
    }
}
