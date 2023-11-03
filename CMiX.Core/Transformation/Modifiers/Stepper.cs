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
        public Stepper(ModifierModeSelector modifierModeSelector, IntegerValue stepCount, BooleanValue visible, BeatModifier beatModifier)
        {
            ModifierModeSelector = new ModifierModeSelector();
            StepCount = stepCount;// new IntegerValue(4);
            Visible = visible; // new BooleanValue(true);
            BeatModifier = beatModifier; // new BeatModifier();
            PingPong = new BooleanValue(false);
            TransformType = new GenericValue<TransformType>();
            DirectionXYZ = new DirectionXYZ();
            Easing = new Easing();
            From = new FloatValue(0.0f);
            To = new FloatValue(1.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
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
        private bool isExpanded = true;
    }
}
