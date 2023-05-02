// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.Modifier;
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

            //Mode = new GenericValue<ModifierMode>(stepperModel.Mode);

            //Visible = new BooleanValue(stepperModel.Visible);
            //BeatModifier = new BeatModifier(stepperModel.BeatModifier, compositionService);

            //XAxis = new BooleanValue(stepperModel.XAxis);
            //YAxis = new BooleanValue(stepperModel.YAxis);
            //ZAxis = new BooleanValue(stepperModel.ZAxis);

            //PingPong = new BooleanValue(stepperModel.PingPong);
            //TransformType = new GenericValue<TransformType>(stepperModel.TransformType);
            //Easing = new Easing(stepperModel.Easing);

            //From = new FloatValue(stepperModel.From);
            //To = new FloatValue(stepperModel.To);
            //StepCount = new IntegerValue(stepperModel.StepCount);

            IsExpanded = true;
        }


        public Guid ID { get; set; }
        public bool Enabled { get; set; }

        public IntegerValue StepCount { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }

        public BooleanValue PingPong { get; set; }

        public BooleanValue XAxis { get; set; }
        public BooleanValue YAxis { get; set; }
        public BooleanValue ZAxis { get; set; }


        public GenericValue<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }

        public FloatValue From { get; set; }
        public FloatValue To { get; set; }


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

            //stepperModel.Visible = (BooleanValueModel)Visible.GetModel();
            //stepperModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            //stepperModel.PingPong = (BooleanValueModel)PingPong.GetModel();

            //stepperModel.XAxis = (BooleanValueModel)XAxis.GetModel();
            //stepperModel.YAxis = (BooleanValueModel)YAxis.GetModel();
            //stepperModel.ZAxis = (BooleanValueModel)ZAxis.GetModel();

            //stepperModel.TransformType = (GenericValueModel<TransformType>)TransformType.GetModel();
            //stepperModel.Mode = (GenericValueModel<ModifierMode>)Mode.GetModel();

            //stepperModel.Easing = (EasingModel)Easing.GetModel();

            //stepperModel.From = (FloatValueModel)From.GetModel();
            //stepperModel.To = (FloatValueModel)To.GetModel();
            //stepperModel.StepCount = (IntegerValueModel)StepCount.GetModel();

            return stepperModel;
        }

        public void SetViewModel(IModel model)
        {
            StepperModel stepperModel = model as StepperModel;
            ID = stepperModel.ID;
            Enabled = stepperModel.Enabled;

            //Mode.SetViewModel(stepperModel.Mode);
            //Visible.SetViewModel(stepperModel.Visible);
            //BeatModifier.SetViewModel(stepperModel.BeatModifier);
            //PingPong.SetViewModel(stepperModel.PingPong);

            //XAxis.SetViewModel(stepperModel.XAxis);
            //YAxis.SetViewModel(stepperModel.YAxis);
            //ZAxis.SetViewModel(stepperModel.ZAxis);

            //TransformType.SetViewModel(stepperModel.TransformType);
            //Easing.SetViewModel(stepperModel.Easing);

            //From.SetViewModel(stepperModel.From);
            //To.SetViewModel(stepperModel.To);
            //StepCount.SetViewModel(stepperModel.StepCount);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
