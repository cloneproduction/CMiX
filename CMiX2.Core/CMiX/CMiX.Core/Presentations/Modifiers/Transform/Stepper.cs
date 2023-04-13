// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public class Stepper : ObservableObject, IControl, IBeatModifiable, ITransformModifier, IDisposable
    {
        public Stepper(StepperModel stepperModel, CompositionService compositionService)
        {
            ID = stepperModel.ID;

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
            var stepperModel = new StepperModel();

            stepperModel.ID = ID;

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

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
