// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class RandomXY : ObservableObject, IBeatModifiable, ITransformModifier
    {
        public RandomXY(RandomXYModel randomXYModel)
        {
            this.ID = randomXYModel.ID;
            this.Name = randomXYModel.Name;
            IsExpanded = true;

            Counter = new Counter(randomXYModel.CounterModel);
            Visible = new ToggleButton(randomXYModel.Visible);

            Easing = new Easing(randomXYModel.EasingModel);
            BeatModifier = new BeatModifier(randomXYModel.BeatModifierModel);
            Mode = new ComboBox<ModifierMode>(randomXYModel.Mode);

            RandomizeLocation = new ToggleButton(randomXYModel.RandomizeLocation);
            RandomizeLocation.IsChecked = true;
            LocationX = new Slider(nameof(LocationX), randomXYModel.LocationX);
            LocationY = new Slider(nameof(LocationY), randomXYModel.LocationY);

            RandomizeScale = new ToggleButton(randomXYModel.RandomizeScale);
            RandomizeScale.IsChecked = true;
            Uniform = new Slider(nameof(Uniform), randomXYModel.Uniform);
            ScaleX = new Slider(nameof(ScaleX), randomXYModel.ScaleX);
            ScaleY = new Slider(nameof(ScaleY), randomXYModel.ScaleY);

            RandomizeRotation = new ToggleButton(randomXYModel.RandomizeScale);
            RandomizeRotation.IsChecked = true;
            Rotation = new Slider(nameof(Rotation), randomXYModel.Rotation);

            Spread = new ToggleButton(randomXYModel.Spread);
        }


        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }


        public Guid ID { get; set; }
        public TransformModifierNames Name { get; set; }
        public ComboBox<ModifierMode> Mode { get; set; }

        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public Counter Counter { get; set; }
        public ToggleButton Spread { get; set; }


        public ToggleButton RandomizeLocation { get; set; }
        public Slider LocationX { get; set; }
        public Slider LocationY { get; set; }

        public ToggleButton RandomizeScale { get; set; }
        public Slider ScaleX { get; set; }
        public Slider ScaleY { get; set; }
        public Slider Uniform { get; set; }

        public ToggleButton RandomizeRotation { get; set; }
        public Slider Rotation { get; set; }


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


        public void SetMasterBeat(MasterBeat masterBeat)
        {
            BeatModifier.SetMasterBeat(masterBeat);
        }

        public void Dispose()
        {
            BeatModifier.Dispose();
        }


        public void SetViewModel(IModel model)
        {
            RandomXYModel randomXYModel = model as RandomXYModel;
            this.ID = randomXYModel.ID;
            this.Name = randomXYModel.Name;
            //this.SelectedModifierType = ModifierMode.AsGroup;

            this.Mode.SetViewModel(randomXYModel.Mode);
            this.Visible.SetViewModel(randomXYModel.Visible);
            this.Spread.SetViewModel(randomXYModel.Spread);

            this.BeatModifier.SetViewModel(randomXYModel.BeatModifierModel);
            this.Counter.SetViewModel(randomXYModel.CounterModel);
            this.Easing.SetViewModel(randomXYModel.EasingModel);

            this.LocationX.SetViewModel(randomXYModel.LocationX);
            this.LocationY.SetViewModel(randomXYModel.LocationY);
            this.RandomizeLocation.SetViewModel(randomXYModel.RandomizeLocation);

            this.ScaleX.SetViewModel(randomXYModel.LocationX);
            this.ScaleY.SetViewModel(randomXYModel.LocationY);
            this.Uniform.SetViewModel(randomXYModel.Uniform);
            this.RandomizeScale.SetViewModel(randomXYModel.RandomizeLocation);

            this.Rotation.SetViewModel(randomXYModel.Rotation);

            this.RandomizeRotation.SetViewModel(randomXYModel.RandomizeLocation);
        }

        public IModel GetModel()
        {
            RandomXYModel model = new RandomXYModel();
            model.ID = this.ID;
            model.Name = this.Name;

            model.Mode = (ComboBoxModel<ModifierMode>)this.Mode.GetModel();

            model.Visible = (ToggleButtonModel)this.Visible.GetModel();
            model.Spread = (ToggleButtonModel)this.Spread.GetModel();

            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.CounterModel = (CounterModel)this.Counter.GetModel();
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
            return model;
        }
    }
}
