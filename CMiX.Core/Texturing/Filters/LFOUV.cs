// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Sampling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LFOUV : ObservableObject, ITextureModifier, IBeatModifiable
    {
        public LFOUV(
            BooleanValue visible, 
            BeatModifier beatModifier, 
            Easing easing, 
            ModifierModeSelector modifierModeSelector, 
            BooleanValue pingPong, 
            BooleanValue xAxis, 
            BooleanValue yAxis, 
            BooleanValue zAxis, 
            FloatValue from,
            FloatValue to,
            SamplerState samplerState)
        {
            Visible = visible;
            BeatModifier = beatModifier;
            Easing = easing;
            ModifierModeSelector = modifierModeSelector;

            PingPong = pingPong;
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;

            From = from; // new FloatValue(0.0f);
            To = to; // new FloatValue(1.0f);
            SamplerState = samplerState;
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public BeatModifier BeatModifier { get; set; }
        public BooleanValue Visible { get; set; }
        public BooleanValue PingPong { get; set; }
        public BooleanValue XAxis { get; set; }
        public BooleanValue YAxis { get; set; }
        public BooleanValue ZAxis { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Easing Easing { get; set; }
        public FloatValue From { get; set; }
        public FloatValue To { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
