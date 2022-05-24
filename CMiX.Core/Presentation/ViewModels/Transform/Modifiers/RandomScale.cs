// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class RandomScale : ObservableObject, IBeatModifiable, ITransformModifier
    {
        public RandomScale(RandomScaleModel randomScaleModel)
        {
            this.ID = randomScaleModel.ID;
            this.Name = randomScaleModel.Name;

            Counter = new Counter(randomScaleModel.CounterModel);
            Visible = new ToggleButton(randomScaleModel.Visible);

            Easing = new Easing(randomScaleModel.EasingModel);
            BeatModifier = new BeatModifier(randomScaleModel.BeatModifierModel);
            Mode = new ComboBox<ModifierMode>(randomScaleModel.Mode);

            ScaleX = new Slider(nameof(ScaleX), randomScaleModel.ScaleX);
            ScaleY = new Slider(nameof(ScaleY), randomScaleModel.ScaleY);
            ScaleZ = new Slider(nameof(ScaleZ), randomScaleModel.ScaleZ);

            UniformXYZ = new Slider(nameof(UniformXYZ), randomScaleModel.UniformXYZ);

            Spread = new ToggleButton(randomScaleModel.Spread);
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


        public Slider ScaleX { get; set; }
        public Slider ScaleY { get; set; }
        public Slider ScaleZ { get; set; }
        public Slider UniformXYZ { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
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
            RandomScaleModel randomScaleModel = model as RandomScaleModel;
            this.ID = randomScaleModel.ID;
            this.Name = randomScaleModel.Name;

            this.Mode.SetViewModel(randomScaleModel.Mode);
            this.Visible.SetViewModel(randomScaleModel.Visible);
            this.Spread.SetViewModel(randomScaleModel.Spread);

            this.BeatModifier.SetViewModel(randomScaleModel.BeatModifierModel);
            this.Counter.SetViewModel(randomScaleModel.CounterModel);
            this.Easing.SetViewModel(randomScaleModel.EasingModel);

            this.ScaleX.SetViewModel(randomScaleModel.ScaleX);
            this.ScaleY.SetViewModel(randomScaleModel.ScaleY);
            this.ScaleZ.SetViewModel(randomScaleModel.ScaleZ);
            this.UniformXYZ.SetViewModel(randomScaleModel.UniformXYZ);
        }

        public IModel GetModel()
        {
            RandomScaleModel model = new RandomScaleModel();
            model.ID = this.ID;
            model.Name = this.Name;
            
            model.Mode = (ComboBoxModel<ModifierMode>)this.Mode.GetModel();

            model.Visible = (ToggleButtonModel)this.Visible.GetModel();
            model.Spread = (ToggleButtonModel)this.Spread.GetModel();

            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.CounterModel = (CounterModel)this.Counter.GetModel();
            model.EasingModel = (EasingModel)this.Easing.GetModel();

            model.ScaleX = (SliderModel)this.ScaleX.GetModel();
            model.ScaleY = (SliderModel)this.ScaleY.GetModel();
            model.ScaleZ = (SliderModel)this.ScaleZ.GetModel();

            model.UniformXYZ = (SliderModel)this.UniformXYZ.GetModel();

            return model;
        }
    }
}
