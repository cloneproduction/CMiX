// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class Stepper : ObservableObject, IBeatModifiable, ITransformModifier, IEase
    {
        public Stepper(StepperModel stepperModel)
        {
            ID = stepperModel.ID;
            Mode = new GenericValue<ModifierMode>(stepperModel.Mode);
            Visible = new BooleanValue(stepperModel.Visible);
            BeatModifier = new BeatModifier(stepperModel.BeatModifier);
            XAxis = new BooleanValue(stepperModel.XAxis);
            YAxis = new BooleanValue(stepperModel.YAxis);
            ZAxis = new BooleanValue(stepperModel.ZAxis);
            PingPong = new BooleanValue(stepperModel.PingPong);
            TransformType = new GenericValue<TransformType>(stepperModel.TransformType);
            Easing = new Easing(stepperModel.Easing);
            From = new FloatValue(stepperModel.From);
            To = new FloatValue(stepperModel.To);
            StepCount = new IntegerValue(stepperModel.StepCount);
            isExpanded = true;
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

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
