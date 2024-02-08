// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LFOUV : ObservableObject, ITextureModifier, IBeatModifiable
    {
        public LFOUV(
            GenericValue<bool> visible, 
            BeatModifier beatModifier, 
            Easing easing, 
            ModifierModeSelector modifierModeSelector, 
            GenericValue<bool> pingPong, 
            GenericValue<bool> xAxis, 
            GenericValue<bool> yAxis, 
            GenericValue<bool> zAxis, 
            GenericValue<float> from,
            GenericValue<float> to,
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

            From = from; // new GenericValue<float>(0.0f);
            To = to; // new GenericValue<float>(1.0f);
            SamplerState = samplerState;
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public BeatModifier BeatModifier { get; set; }
        public GenericValue<bool> Visible { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<bool> XAxis { get; set; }
        public GenericValue<bool> YAxis { get; set; }
        public GenericValue<bool> ZAxis { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
