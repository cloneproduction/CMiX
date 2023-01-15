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
    public class RandomScale : ObservableObject, IBeatModifiable, ITransformModifier
    {
        public RandomScale(RandomScaleModel randomScaleModel, CompositionService compositionService)
        {
            this.ID = randomScaleModel.ID;
            this.Name = randomScaleModel.Name;

            Counter = new IntegerValue(randomScaleModel.CounterModel);
            Visible = new BooleanValue(randomScaleModel.Visible);

            Easing = new Easing(randomScaleModel.EasingModel);
            BeatModifier = new BeatModifier(randomScaleModel.BeatModifierModel, compositionService);

            Mode = new GenericValue<ModifierMode>(randomScaleModel.Mode);

            Scale = new Vector3(randomScaleModel.Scale);
            UniformXYZ = new FloatValue(randomScaleModel.UniformXYZ);

            Spread = new BooleanValue(randomScaleModel.Spread);

            IsExpanded = true;
        }


        public bool Enabled { get; set; }
        public BooleanValue Visible { get; set; }


        public Guid ID { get; set; }
        public TransformModifierNames Name { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public IntegerValue Counter { get; set; }
        public BooleanValue Spread { get; set; }


        public Vector3 Scale { get; set; }
        public FloatValue UniformXYZ { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public void Dispose()
        {
            BeatModifier.Dispose();
        }


        public void SetViewModel(IModel model)
        {
            RandomScaleModel randomScaleModel = model as RandomScaleModel;
            this.ID = randomScaleModel.ID;
            this.Name = randomScaleModel.Name;

            this.Mode.SetViewModel(randomScaleModel.Mode);
            this.Visible.SetViewModel(randomScaleModel.Visible);
            this.Spread.SetViewModel(randomScaleModel.Spread);

            this.BeatModifier.SetViewModel(randomScaleModel.BeatModifierModel);
            this.Counter.SetViewModel(randomScaleModel.CounterModel);
            this.Easing.SetViewModel(randomScaleModel.EasingModel);

            this.Scale.SetViewModel(randomScaleModel.Scale);
            this.UniformXYZ.SetViewModel(randomScaleModel.UniformXYZ);
        }

        public IModel GetModel()
        {
            RandomScaleModel model = new RandomScaleModel();
            model.ID = this.ID;
            model.Name = this.Name;
            
            model.Mode = (GenericValueModel<ModifierMode>)this.Mode.GetModel();

            model.Visible = (BooleanValueModel)this.Visible.GetModel();
            model.Spread = (BooleanValueModel)this.Spread.GetModel();
            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.CounterModel = (IntegerValueModel)this.Counter.GetModel();
            model.EasingModel = (EasingModel)this.Easing.GetModel();
            model.Scale = (Vector3Model)this.Scale.GetModel();
            model.UniformXYZ = (FloatValueModel)this.UniformXYZ.GetModel();

            return model;
        }
    }
}
