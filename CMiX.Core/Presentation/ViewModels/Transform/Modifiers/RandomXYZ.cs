// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class RandomXYZ : ObservableObject, IBeatModifiable, ITransformModifier
    {
        public RandomXYZ(RandomXYZModel randomXYZModel, CompositionService compositionService)
        {
            this.ID = randomXYZModel.ID;
            this.Name = randomXYZModel.Name;

            Counter = new Counter(randomXYZModel.CounterModel);
            Visible = new ToggleButton(randomXYZModel.Visible);

            Easing = new Easing(randomXYZModel.EasingModel);
            BeatModifier = new BeatModifier(randomXYZModel.BeatModifierModel, compositionService);

            Mode = new ComboBox<ModifierMode>(randomXYZModel.Mode);

            RandomizeLocation = new ToggleButton(randomXYZModel.RandomizeLocation);
            RandomizeLocation.IsChecked = true;
            LocationX = new Slider(nameof(LocationX), randomXYZModel.LocationX);
            LocationY = new Slider(nameof(LocationY), randomXYZModel.LocationY);
            LocationZ = new Slider(nameof(LocationZ), randomXYZModel.LocationZ);

            RandomizeScale = new ToggleButton(randomXYZModel.RandomizeScale);
            RandomizeScale.IsChecked = true;
            ScaleX = new Slider(nameof(ScaleX), randomXYZModel.ScaleX);
            ScaleY = new Slider(nameof(ScaleY), randomXYZModel.ScaleY);
            ScaleZ = new Slider(nameof(ScaleZ), randomXYZModel.ScaleZ);

            RandomizeRotation = new ToggleButton(randomXYZModel.RandomizeScale);
            RandomizeRotation.IsChecked = true;
            RotationX = new Slider(nameof(RotationX), randomXYZModel.RotationX);
            RotationY = new Slider(nameof(RotationY), randomXYZModel.RotationY);
            RotationZ = new Slider(nameof(RotationZ), randomXYZModel.RotationZ);

            Spread = new ToggleButton(randomXYZModel.Spread);

            IsExpanded = true;
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
        public Slider LocationZ { get; set; }

        public ToggleButton RandomizeScale { get; set; }
        public Slider ScaleX { get; set; }
        public Slider ScaleY { get; set; }
        public Slider ScaleZ { get; set; }

        public ToggleButton RandomizeRotation { get; set; }
        public Slider RotationX { get; set; }
        public Slider RotationY { get; set; }
        public Slider RotationZ { get; set; }


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
            RandomXYZModel randomXYZModel = model as RandomXYZModel;
            this.ID = randomXYZModel.ID;
            this.Name = randomXYZModel.Name;
            //this.SelectedModifierType = ModifierMode.AsGroup;

            this.Mode.SetViewModel(randomXYZModel.Mode);
            this.Visible.SetViewModel(randomXYZModel.Visible);
            this.Spread.SetViewModel(randomXYZModel.Spread);

            this.BeatModifier.SetViewModel(randomXYZModel.BeatModifierModel);
            this.Counter.SetViewModel(randomXYZModel.CounterModel);
            this.Easing.SetViewModel(randomXYZModel.EasingModel);

            this.LocationX.SetViewModel(randomXYZModel.LocationX);
            this.LocationY.SetViewModel(randomXYZModel.LocationY);
            this.LocationZ.SetViewModel(randomXYZModel.LocationZ);
            this.RandomizeLocation.SetViewModel(randomXYZModel.RandomizeLocation);

            this.ScaleX.SetViewModel(randomXYZModel.LocationX);
            this.ScaleY.SetViewModel(randomXYZModel.LocationY);
            this.ScaleZ.SetViewModel(randomXYZModel.LocationZ);
            this.RandomizeScale.SetViewModel(randomXYZModel.RandomizeLocation);

            this.RotationX.SetViewModel(randomXYZModel.LocationX);
            this.RotationY.SetViewModel(randomXYZModel.LocationY);
            this.RotationZ.SetViewModel(randomXYZModel.LocationZ);
            this.RandomizeRotation.SetViewModel(randomXYZModel.RandomizeLocation);
        }

        public IModel GetModel()
        {
            RandomXYZModel model = new RandomXYZModel();
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
            model.LocationZ = (SliderModel)this.LocationZ.GetModel();
            model.RandomizeLocation = (ToggleButtonModel)this.RandomizeLocation.GetModel();

            model.ScaleX = (SliderModel)this.ScaleX.GetModel();
            model.ScaleY = (SliderModel)this.ScaleY.GetModel();
            model.ScaleZ = (SliderModel)this.ScaleZ.GetModel();
            model.RandomizeScale = (ToggleButtonModel)this.RandomizeLocation.GetModel();

            model.RotationX = (SliderModel)this.RotationX.GetModel();
            model.RotationY = (SliderModel)this.RotationY.GetModel();
            model.RotationZ = (SliderModel)this.RotationZ.GetModel();
            model.RandomizeRotation = (ToggleButtonModel)this.RandomizeLocation.GetModel();
            return model;
        }
    }
}
