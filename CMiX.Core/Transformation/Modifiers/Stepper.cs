// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class Stepper : ObservableObject, IBeatModifiable, IModifier, ISpreadable
    {
        public Stepper(StepperModel stepperModel)
        {
            ID = stepperModel.ID;
            ModifierModeSelector = new ModifierModeSelector(stepperModel.ModifierModeSelector);
            Visible = new BooleanValue(stepperModel.Visible);
            BeatModifier = new BeatModifier(stepperModel.BeatModifier);
            DirectionXYZ = new DirectionXYZ(stepperModel.DirectionXYZ);

            PingPong = new BooleanValue(stepperModel.PingPong);
            TransformType = new GenericValue<TransformType>(stepperModel.TransformType);
            Easing = new Easing(stepperModel.Easing);
            From = new FloatValue(stepperModel.From);
            To = new FloatValue(stepperModel.To);
            StepCount = new IntegerValue(stepperModel.StepCount);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public IntegerValue StepCount { get; set; }
        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public BooleanValue PingPong { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }
        public FloatValue From { get; set; }
        public FloatValue To { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
