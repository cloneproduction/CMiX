// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Stepper : ObservableObject, IControl, IBeatModifiable, ITransformModifier, IDisposable
    {
        public Stepper(StepperModel stepperModel, CompositionService compositionService)
        {
            ID = stepperModel.ID;
            Enabled = stepperModel.Enabled;

            Mode = new ComboBox<ModifierMode>(stepperModel.Mode);

            Visible = new ToggleButton(stepperModel.Visible);
            BeatModifier = new BeatModifier(stepperModel.BeatModifier, compositionService);

            XAxis = new ToggleButton(stepperModel.XAxis);
            YAxis = new ToggleButton(stepperModel.YAxis);
            ZAxis = new ToggleButton(stepperModel.ZAxis);

            PingPong = new ToggleButton(stepperModel.PingPong);
            TransformType = new ComboBox<TransformType>(stepperModel.TransformType);
            Easing = new Easing(stepperModel.Easing);

            From = new Slider(nameof(From), stepperModel.From);
            To = new Slider(nameof(To), stepperModel.To);
            StepCount = new Counter(stepperModel.StepCount);

            IsExpanded = true;
        }


        public Guid ID { get; set; }
        public bool Enabled { get; set; }

        public Counter StepCount { get; set; }
        public ComboBox<ModifierMode> Mode { get; set; }

        public ToggleButton Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }

        public ToggleButton PingPong { get; set; }

        public ToggleButton XAxis { get; set; }
        public ToggleButton YAxis { get; set; }
        public ToggleButton ZAxis { get; set; }


        public ComboBox<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }

        public Slider From { get; set; }
        public Slider To { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public IModel GetModel()
        {
            StepperModel stepperModel = new StepperModel();

            stepperModel.ID = this.ID;
            stepperModel.Enabled = Enabled;

            stepperModel.Visible = (ToggleButtonModel)Visible.GetModel();
            stepperModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            stepperModel.PingPong = (ToggleButtonModel)PingPong.GetModel();

            stepperModel.XAxis = (ToggleButtonModel)XAxis.GetModel();
            stepperModel.YAxis = (ToggleButtonModel)YAxis.GetModel();
            stepperModel.ZAxis = (ToggleButtonModel)ZAxis.GetModel();

            stepperModel.TransformType = (ComboBoxModel<TransformType>)TransformType.GetModel();
            stepperModel.Mode = (ComboBoxModel<ModifierMode>)Mode.GetModel();

            stepperModel.Easing = (EasingModel)Easing.GetModel();

            stepperModel.From = (SliderModel)From.GetModel();
            stepperModel.To = (SliderModel)To.GetModel();
            stepperModel.StepCount = (CounterModel)StepCount.GetModel();

            return stepperModel;
        }

        public void SetViewModel(IModel model)
        {
            StepperModel stepperModel = model as StepperModel;
            ID = stepperModel.ID;
            Enabled = stepperModel.Enabled;

            Mode.SetViewModel(stepperModel.Mode);
            Visible.SetViewModel(stepperModel.Visible);
            BeatModifier.SetViewModel(stepperModel.BeatModifier);
            PingPong.SetViewModel(stepperModel.PingPong);

            XAxis.SetViewModel(stepperModel.XAxis);
            YAxis.SetViewModel(stepperModel.YAxis);
            ZAxis.SetViewModel(stepperModel.ZAxis);

            TransformType.SetViewModel(stepperModel.TransformType);
            Easing.SetViewModel(stepperModel.Easing);

            From.SetViewModel(stepperModel.From);
            To.SetViewModel(stepperModel.To);
            StepCount.SetViewModel(stepperModel.StepCount);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
