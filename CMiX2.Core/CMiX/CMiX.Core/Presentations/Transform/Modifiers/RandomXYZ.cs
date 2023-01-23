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
    public class RandomXYZ : ObservableObject, IBeatModifiable, ITransformModifier
    {
        public RandomXYZ(RandomXYZModel randomXYZModel, CompositionService compositionService)
        {
            this.ID = randomXYZModel.ID;
            this.Name = randomXYZModel.Name;

            Counter = new IntegerValue(randomXYZModel.CounterModel);
            Visible = new BooleanValue(randomXYZModel.Visible);

            Easing = new Easing(randomXYZModel.EasingModel);
            BeatModifier = new BeatModifier(randomXYZModel.BeatModifierModel, compositionService);

            Mode = new GenericValue<ModifierMode>(randomXYZModel.Mode);

            RandomizeLocation = new BooleanValue(randomXYZModel.RandomizeLocation);
            RandomizeLocation.Value = true;
            Location = new Vector3(randomXYZModel.Location);

            RandomizeScale = new BooleanValue(randomXYZModel.RandomizeScale);
            RandomizeScale.Value = true;
            Scale = new Vector3(randomXYZModel.Scale);

            RandomizeRotation = new BooleanValue(randomXYZModel.RandomizeScale);
            RandomizeRotation.Value = true;
            Rotation = new Vector3(randomXYZModel.Rotation);

            Spread = new BooleanValue(randomXYZModel.Spread);

            IsExpanded = true;
        }

        public BooleanValue Visible { get; set; }


        public Guid ID { get; set; }
        public TransformModifierNames Name { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }


        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public IntegerValue Counter { get; set; }
        public BooleanValue Spread { get; set; }


        public BooleanValue RandomizeLocation { get; set; }
        public Vector3 Location{ get; set; }

        public BooleanValue RandomizeScale { get; set; }
        public Vector3 Scale { get; set; }

        public BooleanValue RandomizeRotation { get; set; }
        public Vector3 Rotation { get; set; }



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

            this.Location.SetViewModel(randomXYZModel.Location);
            this.RandomizeLocation.SetViewModel(randomXYZModel.RandomizeLocation);

            this.Scale.SetViewModel(randomXYZModel.Scale);
            this.RandomizeScale.SetViewModel(randomXYZModel.RandomizeLocation);

            this.Rotation.SetViewModel(randomXYZModel.Rotation);
            this.RandomizeRotation.SetViewModel(randomXYZModel.RandomizeLocation);
        }

        public IModel GetModel()
        {
            RandomXYZModel model = new RandomXYZModel();
            model.ID = this.ID;
            model.Name = this.Name;

            model.Mode = (GenericValueModel<ModifierMode>)this.Mode.GetModel();

            model.Visible = (BooleanValueModel)this.Visible.GetModel();
            model.Spread = (BooleanValueModel)this.Spread.GetModel();

            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.CounterModel = (IntegerValueModel)this.Counter.GetModel();
            model.EasingModel = (EasingModel)this.Easing.GetModel();

            model.Location = (Vector3Model)this.Location.GetModel();
            model.RandomizeLocation = (BooleanValueModel)this.RandomizeLocation.GetModel();

            model.Scale = (Vector3Model)this.Scale.GetModel();
            model.RandomizeScale = (BooleanValueModel)this.RandomizeLocation.GetModel();

            model.Rotation = (Vector3Model)this.Rotation.GetModel();
            model.RandomizeRotation = (BooleanValueModel)this.RandomizeLocation.GetModel();
            return model;
        }
    }
}
