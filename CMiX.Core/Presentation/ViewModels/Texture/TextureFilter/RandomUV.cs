// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
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

            Visible = new ToggleButton(randomUVModel.Visible);

            Easing = new Easing(randomUVModel.EasingModel);
            BeatModifier = new BeatModifier(randomUVModel.BeatModifierModel, compositionService);
            SamplerState = new SamplerState(randomUVModel.SamplerState, compositionService);
            RandomizeLocation = new ToggleButton(randomUVModel.RandomizeLocation);
            RandomizeLocation.IsChecked = true;
            LocationX = new Slider(nameof(LocationX), randomUVModel.LocationX);
            LocationY = new Slider(nameof(LocationY), randomUVModel.LocationY);

            RandomizeScale = new ToggleButton(randomUVModel.RandomizeScale);
            RandomizeScale.IsChecked = true;
            Uniform = new Slider(nameof(Uniform), randomUVModel.Uniform);
            ScaleX = new Slider(nameof(ScaleX), randomUVModel.ScaleX);
            ScaleY = new Slider(nameof(ScaleY), randomUVModel.ScaleY);

            RandomizeRotation = new ToggleButton(randomUVModel.RandomizeScale);
            RandomizeRotation.IsChecked = true;
            Rotation = new Slider(nameof(Rotation), randomUVModel.Rotation);
        }


        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }


        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }

        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }


        public ToggleButton RandomizeLocation { get; set; }
        public Slider LocationX { get; set; }
        public Slider LocationY { get; set; }

        public ToggleButton RandomizeScale { get; set; }
        public Slider ScaleX { get; set; }
        public Slider ScaleY { get; set; }
        public Slider Uniform { get; set; }

        public ToggleButton RandomizeRotation { get; set; }
        public Slider Rotation { get; set; }

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
            //this.SelectedModifierType = ModifierMode.AsGroup;


            this.Visible.SetViewModel(randomUVModel.Visible);

            this.BeatModifier.SetViewModel(randomUVModel.BeatModifierModel);
            this.Easing.SetViewModel(randomUVModel.EasingModel);

            this.LocationX.SetViewModel(randomUVModel.LocationX);
            this.LocationY.SetViewModel(randomUVModel.LocationY);
            this.RandomizeLocation.SetViewModel(randomUVModel.RandomizeLocation);

            this.ScaleX.SetViewModel(randomUVModel.LocationX);
            this.ScaleY.SetViewModel(randomUVModel.LocationY);
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

            model.Visible = (ToggleButtonModel)this.Visible.GetModel();

            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.EasingModel = (EasingModel)this.Easing.GetModel();

            model.LocationX = (SliderModel)this.LocationX.GetModel();
            model.LocationY = (SliderModel)this.LocationY.GetModel();

            model.RandomizeLocation = (ToggleButtonModel)this.RandomizeLocation.GetModel();

            model.ScaleX = (SliderModel)this.ScaleX.GetModel();
            model.ScaleY = (SliderModel)this.ScaleY.GetModel();
            model.Uniform = (SliderModel)this.Uniform.GetModel();
            model.RandomizeScale = (ToggleButtonModel)this.RandomizeLocation.GetModel();

            model.Rotation = (SliderModel)this.Rotation.GetModel();

            model.RandomizeRotation = (ToggleButtonModel)this.RandomizeLocation.GetModel();

            model.SamplerState = (SamplerStateModel)this.SamplerState.GetModel();
            return model;
        }
    }
}
